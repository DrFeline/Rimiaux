using Avalonia.Controls;
using Avalonia.Interactivity;
using Rimiaux.ViewModels;

namespace Rimiaux.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
    }

    private async void OpenSettings_Click(object? sender, RoutedEventArgs e)
    {
        var window = new SettingsWindow
        {
            DataContext = new SettingsViewModel()
        };

        bool saved = await window.ShowDialog<bool>(this);

        if (saved && DataContext is MainViewModel vm)
            vm.Status = "Settings saved. Press refresh to rescan.";
    }

}