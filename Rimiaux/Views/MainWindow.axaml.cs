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
}