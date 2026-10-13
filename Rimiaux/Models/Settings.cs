using System.IO;
using System.Text.Json.Serialization;

namespace Rimiaux.Models
{
    public class Settings
    {
        public string? GamePath { get; set; }

        public string? ConfigPath { get; set; }

        public string? SteamWorkshopPath { get; set; }


        [JsonIgnore]
        public string? LocalModsPath => GamePath != null ? Path.Combine(GamePath, "Mods") : null;

        // Folder for core + DLCs
        [JsonIgnore]
        public string? DataPath => GamePath != null? Path.Combine(GamePath, "Data") : null;

        // ModsConfig.xml
        [JsonIgnore]
        public string? ModsConfigPath => ConfigPath != null ? Path.Combine(ConfigPath, "ModsConfig.xml") : null;

    }
}
