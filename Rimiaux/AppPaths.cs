using System;
using System.IO;

namespace Rimiaux
{
    public static class AppPaths
    {
        public static string DataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rimiaux");
        public static string SettingsFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Rimiaux");
        public static string SettingsFile => Path.Combine(SettingsFolder, "settings.json");

        public static void Load()
        {
            Directory.CreateDirectory(DataFolder);
            Directory.CreateDirectory(SettingsFolder);
        }

    }
}
