using System.Collections.Generic;

namespace Rimiaux.Models
{
    public enum ModSource { Official, Local, Workshop }

    public class ModInfo
    {
        public required string PackageId { get; init; } // should always be lowercase
        public required string Name { get; init; }
        public string? Author { get; init; }
        public required string FolderPath { get; init; }
        public ModSource Source { get; init; }
        public List<string> SupportedVersions { get; init; } = new();

        // stores packageIds (lowercase!!)
        public List<string> Dependencies { get; init; } = new();
        public List<string> LoadBefore { get; init; } = new();
        public List<string> LoadAfter { get; init; } = new();

    }
}
