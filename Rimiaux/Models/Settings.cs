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

        [JsonIgnore]
        public string? DataPath => GamePath != null? Path.Combine(GamePath, "Data") : null;

        // ModsConfig.xml is located in this Config folder
        [JsonIgnore]
        public string? ModsConfigPath => ConfigPath != null ? Path.Combine(ConfigPath, "ModsConfig.xml") : null;

    }
}
