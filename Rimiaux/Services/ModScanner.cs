using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Avalonia.Dialogs.Internal;
using Rimiaux.Models;

namespace Rimiaux.Services
{
    // There is still a lot to do. Check TODOs
    public class ModScanner
    {
        public List<string> Errors { get; } = new();

        public List<ModInfo> ScanAll(Settings settings)
        {
            List<ModInfo> mods = new List<ModInfo>();

            //ScanFolder(settings.DataPath, ModSource.Official, mods);
            ScanFolder(settings.LocalModsPath, ModSource.Local, mods);
            ScanFolder(settings.SteamWorkshopPath, ModSource.Workshop, mods);

            return mods;
        }

        private void ScanFolder(string? root, ModSource source, List<ModInfo> mods)
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                return;

            foreach(string modFolder in Directory.EnumerateDirectories(root))
            {
                ModInfo? mod = ReadMod(modFolder, source);
                if (mod != null)
                    mods.Add(mod);
            }
            
        }

        public ModInfo? ReadMod(string modFolder, ModSource source)
        {
            string aboutPath = Path.Combine(modFolder, "About", "About.xml");
            if (!File.Exists(aboutPath))
                return null;

            XElement? root;
            try
            {
                root = XDocument.Load(aboutPath).Root;
            }
            catch (Exception ex) when (ex is XmlException or IOException)
            {
                //Errors.Add(aboutPath + ": " + ex.Message);
                Errors.Add($"{aboutPath}: {ex.Message}");
                return null;
            }
            if (root == null)
                return null;

            string? name = ((string?)root.Element("name"));
            string? packageId = ((string?)root.Element("packageId"));
            string? author = ((string?)root.Element("author"));

            // clunky but I want to keep intent explicit or something
            if (string.IsNullOrEmpty(name))
                Errors.Add($"{aboutPath}: no name");
            if (string.IsNullOrEmpty(packageId))
                Errors.Add($"{aboutPath}: no packageId");
            if (string.IsNullOrEmpty(author))
                Errors.Add($"{aboutPath}: no author");
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(packageId) || string.IsNullOrEmpty(author))
                return null;

            return new ModInfo
            {
                PackageId = packageId.ToLowerInvariant(),
                Name = (string?)root.Element("name") ?? Path.GetFileName(modFolder),
                Author = author /* TODO: only returns first element of author!!! there can be multiple (list) */,
                FolderPath = modFolder,
                Source = source,
                SupportedVersions = root.Elements("supportedVersions").Select(x => (string)x).ToList(),
                Dependencies = root.Elements("modDependencies").Elements("li")
                    .Select(li => (string?)li.Element("packageId"))
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => id!.ToLowerInvariant())
                    .ToList(),
                // need to implement loadBefore and loadAfter
            };
        }
    }
}
