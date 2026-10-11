using System;
using System.IO;

namespace Rimiaux
{
    public static class AppPaths
    {
        public static string DataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rimiaux");
        public static string SettingsFile => Path.Combine(DataFolder, "settings.json");

        // runs once the first time the class is accessed, ensures the data folder exists
        static AppPaths() => Directory.CreateDirectory(DataFolder);
    }
}
