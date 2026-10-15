using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Rimiaux.ViewModels;

namespace Rimiaux.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private SettingsViewModel? Vm => DataContext as SettingsViewModel;

    private async Task<string?> PickFolderAsync(string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
    }

    private async void BrowseGamePath(object? sender, RoutedEventArgs e)
    {
        if (Vm is null) return;
        if (await PickFolderAsync("Select RimWorld folder") is { } path)
            Vm.GamePath = path;
    }

    private async void BrowseWorkshopPath(object? sender, RoutedEventArgs e)
    {
        if (Vm is null) return;
        if (await PickFolderAsync("Select Workshop folder") is { } path)
            Vm.WorkshopPath = path;
    }

    private async void BrowseConfigPath(object? sender, RoutedEventArgs e)
    {
        if (Vm is null) return;
        if (await PickFolderAsync("Select config folder") is { } path)
            Vm.ConfigPath = path;
    }

    private void Save(object? sender, RoutedEventArgs e) => Close(true);
    private void Cancel(object? sender, RoutedEventArgs e) => Close(false);
}