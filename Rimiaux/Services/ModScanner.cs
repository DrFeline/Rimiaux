using Rimiaux.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Rimiaux.Services
{
    public static partial class ModScanner
    {
        public static IReadOnlyDictionary<string, IReadOnlyList<ModMetaData>> ScanAll()
        {
            Dictionary<string, List<ModMetaData>> mods = new Dictionary<string, List<ModMetaData>>();

            ScanFolder(SettingsService.Settings.DataPath, ModSource.Official, mods);
            ScanFolder(SettingsService.Settings.LocalModsPath, ModSource.Local, mods);
            ScanFolder(SettingsService.Settings.SteamWorkshopPath, ModSource.Workshop, mods);

            return mods.ToDictionary(
                kv => kv.Key,
                kv => (IReadOnlyList<ModMetaData>)kv.Value.AsReadOnly());
        }

        private static void ScanFolder(string? root, ModSource source, Dictionary<string, List<ModMetaData>> mods)
        {
            if (!Directory.Exists(root))
                return;

            foreach (string modFolderPath in Directory.EnumerateDirectories(root))
            {
                ModMetaData mod = ReadMod(modFolderPath, source);
                Add(mod, mods);
            }
        }

        private static void Add(ModMetaData mod, Dictionary<string, List<ModMetaData>> mods)
        {
            if (!mods.TryGetValue(mod.PackageId, out List<ModMetaData>? entry))
            {
                mods[mod.PackageId] = new List<ModMetaData> { mod };
                return;
            }

            if (entry.Any(m => m.Source == mod.Source))
            {
                // TODO: record a duplicate warning
            }

            int index = entry.FindIndex(m => m.Source > mod.Source);
            entry.Insert(index == -1 ? entry.Count : index, mod);

        }

        private static ModMetaData ReadMod(string modFolderPath, ModSource source)
        {
            XElement? root = null;
            string? aboutPath = FindAboutXml(modFolderPath);
            bool aboutValidation = false;

            if (aboutPath != null)
            {
                try
                {
                    root = XDocument.Load(aboutPath).Root;
                    aboutValidation = root != null;
                }
                catch (Exception ex) when (ex is XmlException or IOException or UnauthorizedAccessException)
                {
                    // TODO: record a malformed about.xml warning
                    root = null;
                }
            }

            string displayName = GetDisplayNameHelper(root, modFolderPath, source);

            return new ModMetaData
            {
                PackageId = GetPackageIdHelper(root, displayName),
                Name = XmlStr(root, "name"),
                DisplayName = displayName,
                Authors = GetAuthorsHelper(root),
                Description = XmlStr(root, "description"),
                FolderPath = modFolderPath,
                ModVersion = XmlStr(root, "modVersion"),
                SupportedVersions = root?.Element("supportedVersions")?.Elements("li").Select(li => li.Value.Trim()).ToList() ?? new(),
                Url = XmlStr(root, "url"),
                Source = source,
                ValidAboutXml = aboutValidation,

                Dependencies = GetDependencies(root?.Element("modDependencies")),
                LoadBefore = GetStringList(root, "loadBefore"),
                LoadAfter = GetStringList(root, "loadAfter"),
                IncompatibleWith = GetStringList(root, "incompatibleWith"),
                ForceLoadBefore = GetStringList(root, "forceLoadBefore"),
                ForceLoadAfter = GetStringList(root, "forceLoadAfter")
            };
        }

        

        private static string GetPackageIdHelper(XElement? root, string name)
        {
            string? packageId = XmlStr(root, "packageId");
            if (!string.IsNullOrEmpty(packageId))
                return packageId.ToLowerInvariant();

            string author = XmlStr(root, "author") ?? "Anonymous";
            string description = XmlStr(root, "description") ?? "No description provided.";

            string hash = "none";
            if (!string.IsNullOrEmpty(description))
            {
                hash = StableStringHash(description).ToString(CultureInfo.InvariantCulture).Replace("-", "");
                hash = hash.Substring(0, Math.Min(3, hash.Length));
            }

            return (ConvertToAscii(author + hash) + "." + ConvertToAscii(name)).ToLowerInvariant();
        }

        private static string GetDisplayNameHelper(XElement? root, string modFolderPath, ModSource source)
        {
            string? name = XmlStr(root, "name");

            if (!string.IsNullOrEmpty(name))
                return name;

            if (source == ModSource.Workshop)
                return "Workshop mod " + Path.GetFileName(modFolderPath);

            return Path.GetFileName(modFolderPath);
        }

        private static List<string> GetAuthorsHelper(XElement? root)
        {
            List<string> authors = new List<string>();
            if (root == null)
                return authors;

            string? author = XmlStr(root, "author");

            if (!string.IsNullOrEmpty(author))
                authors.Add(author);

            authors.AddRange(root.Element("authors")?.Elements("li")
                .Select(li => li.Value)
                .Where(s => s.Length > 0) ?? Enumerable.Empty<string>());

            return authors.Distinct().ToList();
        }

        private static List<ModDependency> GetDependencies(XElement? container)
        {
            if (container == null)
                return new List<ModDependency>();

            return container.Elements("li").Select(li =>
            {
                string packageId = (XmlStr(li, "packageId") ?? "").ToLowerInvariant();
                if (packageId.Length == 0)
                    return null;
                string displayName = XmlStr(li, "displayName") ?? "";
                string? steamUrl = XmlStr(li, "steamWorkshopUrl");
                string? downloadUrl = XmlStr(li, "downloadUrl");

                bool valid = packageId.Length > 0
                          && PackageIdFormat.IsMatch(packageId)
                          && displayName.Length > 0
                          && (!string.IsNullOrEmpty(steamUrl) || !string.IsNullOrEmpty(downloadUrl) || packageId.Contains("ludeon"));

                if (!valid)
                    return null;

                return new ModDependency
                {
                    PackageId = packageId,
                    DisplayName = displayName,
                    SteamWorkshopUrl = steamUrl,
                    DownloadUrl = downloadUrl,
                    AlternativePackageIds = GetStringList(li, "alternativePackageIds")
                };
            }).OfType<ModDependency>().ToList();
        }

        private static List<string> GetStringList(XElement? root, string element)
        {
            return root?.Element(element)?.Elements("li")
                .Select(li => li.Value.Trim().ToLowerInvariant())
                .Where(id => id.Length > 0)
                .ToList() ?? new List<string>();
        }

        private static string? FindAboutXml(string modFolderPath)
        {
            string? aboutDir = Directory.EnumerateDirectories(modFolderPath, "About", IgnoreCase).FirstOrDefault();
            if (aboutDir == null) return null;

            return Directory.EnumerateFiles(aboutDir, "About.xml", IgnoreCase).FirstOrDefault();
        }

        private static readonly EnumerationOptions IgnoreCase = new()
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            RecurseSubdirectories = false
        };

        private static string? XmlStr(XElement? root, string name)
        {
            string? v = (string?)root?.Element(name);
            return v?.Replace("\\n", "\n");
        }

        private static string ConvertToAscii(string part)
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

        private static int StableStringHash(string str)
        {
            unchecked
            {
                int num = 23;
                foreach (char c in str)
                    num = num * 31 + c;
                return num;
            }
        }

        [GeneratedRegex(@"(?=.{1,60}$)^(?!\.)(?=.*?[.])(?!.*([.])\1+)[a-zA-Z0-9.]{1,}[a-zA-Z0-9]{1}$")]
        private static partial Regex PackageIdFormat { get; }

        









    }
}
