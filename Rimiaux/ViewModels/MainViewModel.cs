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
    public partial string Status { get; set; } = "Set your folders and press Scan.";

    public ObservableCollection<ModInfo> Mods { get; } = new();
    public ObservableCollection<string> Errors { get; } = new();

    [RelayCommand]
    private async Task ScanAsync()
    {
        _settings.GamePath = string.IsNullOrWhiteSpace(GamePath) ? null : GamePath.Trim();
        _settings.SteamWorkshopPath = string.IsNullOrWhiteSpace(WorkshopPath) ? null : WorkshopPath.Trim();
        _settings.ConfigPath = string.IsNullOrWhiteSpace(ConfigPath) ? null : ConfigPath.Trim();
        SettingsService.Save(_settings);

        Status = "Scanning...";
        ModScanner scanner = new ModScanner();

        List<ModInfo> mods = await Task.Run(() => scanner.ScanAll(_settings));

        Mods.Clear();
        foreach (ModInfo mod in mods.OrderBy(m => m.Name))
            Mods.Add(mod);

        Errors.Clear();
        foreach (string error in scanner.Errors)
            Errors.Add(error);

        Status = $"Found {Mods.Count} mods, {Errors.Count} problems.";
    }
}
