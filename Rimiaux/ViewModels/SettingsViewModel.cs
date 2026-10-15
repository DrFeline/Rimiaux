using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Rimiaux.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        public partial string GamePath { get; set; }

        [ObservableProperty]
        public partial string WorkshopPath { get; set; }

        [ObservableProperty]
        public partial string ConfigPath { get; set; }

        public SettingsViewModel(string gamePath, string workshopPath, string configPath)
        {
            GamePath = gamePath;
            WorkshopPath = workshopPath;
            ConfigPath = configPath;
        }

        public SettingsViewModel() : this("", "", "") { }
    }
}
