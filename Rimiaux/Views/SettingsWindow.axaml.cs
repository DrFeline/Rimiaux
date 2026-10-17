using System;
using Avalonia.Controls;
using Rimiaux.ViewModels;

namespace Rimiaux.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is SettingsViewModel vm)
            vm.CloseRequested += saved => Close(saved);
    }
}