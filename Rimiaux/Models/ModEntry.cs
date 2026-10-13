using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rimiaux.Models
{
    public class ModEntry
    {
        public string PackageId { get; init; }
        public ModInfo? Local { get; set; } // official or mods folder
        public ModInfo? Workshop { get; set; }

        public ModEntry(ModInfo mod, ModSource source)
        {
            if (source == ModSource.Local || source == ModSource.Official)
                this.Local = mod;
            this.Workshop = mod;
            this.PackageId = mod.PackageId;
        }

        public ModEntry(ModInfo local, ModInfo workshop)
        {
            this.Local = local;
            this.Workshop = workshop;
            this.PackageId = local.PackageId;
        }

    }
}
