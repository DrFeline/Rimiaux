using Avalonia.Controls;
using Avalonia.Interactivity;
using Rimiaux.ViewModels;

namespace Rimiaux.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenSettings(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        var settingsVm = new SettingsViewModel(vm.GamePath, vm.WorkshopPath, vm.ConfigPath);
        var dialog = new SettingsWindow { DataContext = settingsVm };

        bool saved = await dialog.ShowDialog<bool>(this);
        if (saved)
            vm.ApplySettings(settingsVm.GamePath, settingsVm.WorkshopPath, settingsVm.ConfigPath);
    }
}