using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Avalonia.Dialogs.Internal;
using Rimiaux.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace Rimiaux.Services
{
    public class ModScanner
    {
        
        public List<ModInfo> ScanAll(Settings settings)
        {
            List<ModInfo> mods = new List<ModInfo>();

            ScanFolder(settings.DataPath, ModSource.Official, mods);
            ScanFolder(settings.LocalModsPath, ModSource.Local, mods);
            ScanFolder(settings.SteamWorkshopPath, ModSource.Workshop, mods);

            return mods;
        }

        private void ScanFolder(string? root, ModSource source, List<ModInfo> mods)
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                return;

            foreach(string modFolderPath in Directory.EnumerateDirectories(root))
            {
                ModInfo? mod = ReadMod(modFolderPath, source);
                if (mod != null)
                    mods.Add(mod);
            }
            
        }

        public ModInfo? ReadMod(string modFolderPath, ModSource source)
        {
            string aboutPath = Path.Combine(modFolderPath, "About", "About.xml");
            if (!File.Exists(aboutPath))
                return null;

            XElement? root;
            try
            {
                root = XDocument.Load(aboutPath).Root;
            }
            catch (Exception ex) when (ex is XmlException or IOException)
            {
                return null;
            }
            if (root == null)
                return null;

            string name = getNameHelper(root, modFolderPath, source);

            return new ModInfo
            {
                PackageId = packageIdHelper(root, name),
                Name = name,
                Authors = getAuthorsHelper(root),
                Description = (string?)root.Element("description") ?? "No description provided.",
                FolderPath = modFolderPath,
                ModVersion = (string?)root.Element("modVersion"),
                SupportedVersions = root.Elements("supportedVersions").Select(x => (string)x).ToList(),
                Url = (string?)root.Element("url"),
                Source = source,

                Dependencies = getListedPackageIds(root, "modDependencies"),
                LoadBefore = getListedPackageIds(root, "loadBefore"),
                LoadAfter = getListedPackageIds(root, "loadAfter"),
                IncompatibleWith = getListedPackageIds(root, "incompatibleWith"),
                ForceLoadBefore = getListedPackageIds(root, "forceLoadBefore"),
                ForceLoadAfter = getListedPackageIds(root, "forceLoadAfter")
            };
        }

        private static string packageIdHelper(XElement root, string name)
        {
            string? packageId = (string?)root.Element("packageId");
            if (!string.IsNullOrEmpty(packageId))
                return packageId.ToLowerInvariant();

            string author = (string?)root.Element("author") ?? "Anonymous";
            string description = (string?)root.Element("description") ?? "No description provided.";

            string hash = "none";
            if (!string.IsNullOrEmpty(description))
            {
                hash = stableStringHash(description).ToString().Replace("-", "");
                hash = hash.Substring(0, Math.Min(3, hash.Length));
            }

            return (convertToAscii(author + hash) + "." + convertToAscii(name)).ToLowerInvariant();
        }

        private static string getNameHelper(XElement root, string modFolderPath, ModSource source)
        {
            string? name = ((string?)root.Element("name"));
            if (!string.IsNullOrEmpty(name))
                return name;

            if (source == ModSource.Workshop)
                return "Workshop mod " + Path.GetFileName(modFolderPath);

            return Path.GetFileName(modFolderPath);
        }

        private static List<string> getAuthorsHelper(XElement root)
        {
            List<string> authors = new List<string>();
            string? author = (string?)root.Element("author");
            if (!string.IsNullOrEmpty(author))
                authors.Add(author);

            authors.AddRange(root.Descendants("authors").Elements("li").Select(li => li.Value).Where(s => s.Length > 0));

            authors = authors.Distinct().ToList();

            if (authors.Count == 0)
                authors.Add("Anonymous");

            return authors;
        }

        private static List<string> getListedPackageIds(XElement root, string element)
        {
            XElement? container = root?.Element(element);
            if (container == null)
                return new List<string>();

            return container.Elements("li")
                .Select(li => li.HasElements ? (string?)li.Element("packageId"): li.Value)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!.Trim()).ToList();
        }

        private static string convertToAscii(string part)
        {
            StringBuilder sb = new StringBuilder(part.Length);
            foreach (char ch in part)
            {
                char c = ch;
                if (!char.IsLetterOrDigit(c) || c >= '\u0080')
                    c = (char)(c % 25 + 'A');
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static int stableStringHash(string str)
        {
            unchecked
            {
                int num = 23;
                foreach (char c in str)
                    num = num * 31 + c;
                return num;
            }
        }

    }
}
