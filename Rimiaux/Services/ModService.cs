using CommunityToolkit.Mvvm.ComponentModel;
using Rimiaux.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rimiaux.Services
{
    public static class ModService
    {
        private static readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

        public static IReadOnlyDictionary<string, IReadOnlyList<ModMetaData>> Mods => Volatile.Read(ref _mods);
        private static IReadOnlyDictionary<string, IReadOnlyList<ModMetaData>> _mods = new Dictionary<string, IReadOnlyList<ModMetaData>>();
        
        public static IEnumerable<ModMetaData> AllMods => Mods.Values.SelectMany(list => list);

        public static async Task RefreshModsAsync()
        {
            await _gate.WaitAsync();
            
            try
            {
                IReadOnlyDictionary<string, IReadOnlyList<ModMetaData>> scanned = await Task.Run(() => ModScanner.ScanAll());
                Volatile.Write(ref _mods, scanned);
            }
            finally
            {
                _gate.Release();
            }

        }

    }

}
