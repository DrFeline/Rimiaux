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
    private readonly Settings _settings = SettingsService.Load();
    
    [ObservableProperty]
    public partial string GamePath { get; set; } = "";

    [ObservableProperty]
    public partial string WorkshopPath { get; set; } = "";

    [ObservableProperty]
    public partial string ConfigPath { get; set; } = "";

    public MainViewModel()
    {
        GamePath = _settings.GamePath ?? "";
        WorkshopPath = _settings.SteamWorkshopPath ?? "";
        ConfigPath = _settings.ConfigPath ?? "";
    }

    [ObservableProperty]
    public partial string Status { get; set; } = "Press refresh to scan folders.";

    public ObservableCollection<ModInfo> Mods { get; } = new();

    [RelayCommand]
    private async Task ScanAsync()
    {
        _settings.GamePath = string.IsNullOrWhiteSpace(GamePath) ? null : GamePath.Trim();
        _settings.SteamWorkshopPath = string.IsNullOrWhiteSpace(WorkshopPath) ? null : WorkshopPath.Trim();
        _settings.ConfigPath = string.IsNullOrWhiteSpace(ConfigPath) ? null : ConfigPath.Trim();
        SettingsService.Save(_settings);

        Status = "Scanning...";
        ModIndex modIndex = new ModIndex();
        
        await Task.Run(() => modIndex.PopulateIndex(_settings));

        IEnumerable<ModInfo> mods = modIndex._mods.Values
            .Select(entry => entry.Local ?? entry.Workshop)
            .OfType<ModInfo>()
            .OrderBy(mod => mod.Name);

        Mods.Clear();
        foreach (ModInfo mod in mods)
            Mods.Add(mod);

        Status = $"Found {Mods.Count} mods.";
    }
}
