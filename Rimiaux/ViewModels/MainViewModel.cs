using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rimiaux.Models;
using Rimiaux.Services;

namespace Rimiaux.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    public partial ModMetaData? SelectedMod { get; set; }

    [ObservableProperty]
    public partial string Status { get; set; } = "Press refresh to scan folders.";

    [ObservableProperty]
    public partial ObservableCollection<ModMetaData> InactiveModList { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<ModMetaData> ActiveModList { get; set; } = new();

    
    [RelayCommand]
    private async Task RefreshAsync()
    {
        Status = "Scanning...";

        await ModService.RefreshModsAsync();

        IOrderedEnumerable<ModMetaData> sorted = ModService.AllMods
            .OrderBy(m => m.Source)
            .ThenBy(m => m.DisplayName, StringComparer.CurrentCultureIgnoreCase);

        InactiveModList.Clear();
        foreach (ModMetaData mod in sorted)
            InactiveModList.Add(mod);

        Status = $"Found {InactiveModList.Count} mods.";
    }

}
