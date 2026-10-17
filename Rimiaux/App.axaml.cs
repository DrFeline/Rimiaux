using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Rimiaux.Services;
using Rimiaux.ViewModels;
using Rimiaux.Views;
using System;
using System.IO;

namespace Rimiaux;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                AppPaths.Load();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                base.OnFrameworkInitializationCompleted();
                return;
            }

            SettingsService.Load();

            desktop.MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}