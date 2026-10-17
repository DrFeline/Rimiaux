using System;
using System.IO;
using System.Text.Json;
using Rimiaux.Models;

namespace Rimiaux.Services
{
    public static class SettingsService
    {
        public static Settings Settings { get; private set; } = new();

        public static void Load()
        {
            if (!File.Exists(AppPaths.SettingsFile))
            {
                Settings = new Settings();
                return;
            }

            try
            {
                string json = File.ReadAllText(AppPaths.SettingsFile);
                Settings = JsonSerializer.Deserialize<Settings>(json, JsonOptions) ?? new Settings();
            }
            catch (System.Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                // TODO: record corrupted settings file warning
                Settings = new Settings();
                BackUpCorruptFile();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // TODO: Record settings file access warning
                Settings = new Settings();
            }
        }

        public static void Save(Settings settings)
        {
            string json = JsonSerializer.Serialize(settings, JsonOptions);

            Directory.CreateDirectory(Path.GetDirectoryName(AppPaths.SettingsFile)!);

            string tmp = AppPaths.SettingsFile + ".tmp";
            File.WriteAllText(tmp, json);
            File.Move(tmp, AppPaths.SettingsFile, overwrite: true);

            Settings = settings;
        }

        private static void BackUpCorruptFile()
        {
            try
            {
                string backup = $"{AppPaths.SettingsFile}.corrupt-{DateTime.Now:yyyyMMdd-HHmmss}";
                File.Move(AppPaths.SettingsFile, backup);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // TODO: record failed to backup corrupted settings file warning
            }
        }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

    }
}
