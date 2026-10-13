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
    private readonly Settings _settings = SettingsService.Load();
    
    [ObservableProperty]
    public partial string GamePath { get; set; } = "";

    [ObservableProperty]
    public partial string WorkshopPath { get; set; } = "";

    [ObservableProperty]
    public partial string ConfigPath { get; set; } = "";

    [ObservableProperty]
    public partial ModInfo? SelectedMod { get; set; }
    public MainViewModel()
    {
        GamePath = _settings.GamePath ?? "";
        WorkshopPath = _settings.SteamWorkshopPath ?? "";
        ConfigPath = _settings.ConfigPath ?? "";
    }

    [ObservableProperty]
    public partial string Status { get; set; } = "Press refresh to scan folders.";

    [ObservableProperty]
    public partial ObservableCollection<ModInfo> Mods { get; set; } = new();

    [RelayCommand]
    private async Task ScanAsync()
    {
        _settings.GamePath = string.IsNullOrWhiteSpace(GamePath) ? null : GamePath.Trim();
        _settings.SteamWorkshopPath = string.IsNullOrWhiteSpace(WorkshopPath) ? null : WorkshopPath.Trim();
        _settings.ConfigPath = string.IsNullOrWhiteSpace(ConfigPath) ? null : ConfigPath.Trim();
        SettingsService.Save(_settings);

        Status = "Scanning...";

        try
        {
            List<ModInfo> mods = await Task.Run(() =>
            {
                ModIndex modIndex = new ModIndex();
                modIndex.PopulateIndex(_settings);
                
                return modIndex.Mods.Values
                    .SelectMany(entry => entry.AllInstances)
                    .OrderBy(mod => mod.Name, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(mod => mod.Source)
                    .ToList();
            });

            Mods = new ObservableCollection<ModInfo>(mods);
            Status = $"Found {Mods.Count} mods.";

        }
        catch (Exception ex)
        {
            Status = $"Scan failed: {ex.Message}";
        }
    }
}
