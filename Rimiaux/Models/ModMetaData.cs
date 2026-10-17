using System.Collections.Generic;

namespace Rimiaux.Models
{
    public enum ModSource { Official, Local, Workshop }

    public class ModMetaData
    {
        public required string PackageId { get; init; } // should always be lowercase
        public string? Name { get; init; }
        public required string DisplayName { get; init; }
        public List<string> Authors { get; init; } = new();
        public string? Description { get; init; }
        public required string FolderPath { get; init; }
        public string? ModVersion { get; init; }
        public List<string> SupportedVersions { get; init; } = new();
        public string? Url { get; init; }
        public required ModSource Source { get; init; }
        public string? PreviewImagePath { get; init; }
        public required bool ValidAboutXml { get; init; }

        
        // stores packageIds (lowercase!!)
        public List<ModDependency> Dependencies { get; init; } = new();
        public List<string> LoadBefore { get; init; } = new();
        public List<string> LoadAfter { get; init; } = new();
        public List<string> IncompatibleWith { get; init; } = new();
        public List<string> ForceLoadBefore { get; init; } = new();
        public List<string> ForceLoadAfter { get; init; } = new();

    }
}
