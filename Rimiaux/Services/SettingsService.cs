using System.IO;
using System.Text.Json;
using Rimiaux.Models;

namespace Rimiaux.Services
{
    public static class SettingsService
    {
        public static Settings Load()
        {
            try
            {
                if (File.Exists(AppPaths.SettingsFile))
                    return JsonSerializer.Deserialize<Settings>(File.ReadAllText(AppPaths.SettingsFile)) ?? new Settings();
            }
            catch (System.Exception ex) when (ex is JsonException or IOException) { }
            return new Settings();
        }

        public static void Save(Settings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AppPaths.SettingsFile, json);
        }
    }
}
