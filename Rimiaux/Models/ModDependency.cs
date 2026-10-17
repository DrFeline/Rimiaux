using System;
using System.Collections.Generic;
using System.Text;

namespace Rimiaux.Models
{
    public class ModDependency
    {
        public string PackageId { get; init; } = "";
        public string DisplayName { get; init; } = "";
        public string? SteamWorkshopUrl { get; init; }
        public string? DownloadUrl { get; init; }
        public List<string> AlternativePackageIds { get; init; } = new List<string>();

        public string? Url => SteamWorkshopUrl ?? DownloadUrl;

    }
}
