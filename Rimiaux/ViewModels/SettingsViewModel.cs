using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rimiaux.Models;
using Rimiaux.Services;

namespace Rimiaux.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string? GamePath { get; set; }

    [ObservableProperty]
    public partial string? ConfigPath { get; set; }

    [ObservableProperty]
    public partial string? SteamWorkshopPath { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    public event Action<bool>? CloseRequested;

    public SettingsViewModel()
    {
        Settings current = SettingsService.Settings;
        GamePath = current.GamePath;
        ConfigPath = current.ConfigPath;
        SteamWorkshopPath = current.SteamWorkshopPath;
    }

    [RelayCommand]
    private void Save()
    {
        var settings = new Settings
        {
            GamePath = Clean(GamePath),
            ConfigPath = Clean(ConfigPath),
            SteamWorkshopPath = Clean(SteamWorkshopPath),
        };

        try
        {
            SettingsService.Save(settings);
            CloseRequested?.Invoke(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorMessage = $"Could not save settings: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(false);

    private static string? Clean(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;
        return path.Trim().Trim('"');
    }
}