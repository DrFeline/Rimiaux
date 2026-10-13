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

        public ModEntry(ModInfo mod)
        {
            this.PackageId = mod.PackageId;
            if (mod.Source == ModSource.Local || mod.Source == ModSource.Official)
                this.Local = mod;
            else
                this.Workshop = mod;
        }

        public ModEntry(ModInfo local, ModInfo workshop)
        {
            this.Local = local;
            this.Workshop = workshop;
            this.PackageId = local.PackageId;
        }

        public IEnumerable<ModInfo> AllInstances
        {
            get
            {
                if (Local != null)
                    yield return Local;
                if (Workshop != null)
                    yield return Workshop;
            }
        }

    }
}
