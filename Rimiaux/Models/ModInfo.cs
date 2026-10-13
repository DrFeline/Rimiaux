using System.Collections.Generic;

namespace Rimiaux.Models
{
    public enum ModSource { Official, Local, Workshop }

    public class ModInfo
    {
        public required string PackageId { get; init; } // should always be lowercase
        public required string Name { get; init; }
        public List<string> Authors { get; init; } = new();
        public required string Description { get; init; }
        public required string FolderPath { get; init; }
        public string? ModVersion { get; init; }
        public List<string> SupportedVersions { get; init; } = new();
        public string? Url { get; init; }
        public ModSource Source { get; init; }


        // stores packageIds (lowercase!!)
        public List<string> Dependencies { get; init; } = new();
        public List<string> LoadBefore { get; init; } = new();
        public List<string> LoadAfter { get; init; } = new();
        public List<string> IncompatibleWith { get; init; } = new();
        public List<string> ForceLoadBefore { get; init; } = new();
        public List<string> ForceLoadAfter { get; init; } = new();

    }
}
