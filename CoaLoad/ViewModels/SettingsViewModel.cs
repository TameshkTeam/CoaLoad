using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CoaLoad.Helpers;
using ReactiveUI;

namespace CoaLoad.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        public static ObservableCollection<string> InstanceOptions { get; } = new()
        {
            "https://instance1.cobalt.tools",
            "https://instance2.cobalt.tools",
            "https://instance3.cobalt.tools",
            "Custom Instance"
        };

        public static ObservableCollection<string> VideoQualityOptions { get; } = new()
        {
            "144p",
            "240p",
            "360p",
            "480p",
            "720p",
            "1080p",
            "1440p",
            "4k",
            "8k+"
        };

        private static string? _selectedInstance;
        public string? SelectedInstance
        {
            get => _selectedInstance;
            set
            {
                if (value == InstanceOptions[InstanceOptions.Count - 1])
                {
                    OpenCustomInstanceDialog();
                }
                else
                {
                    this.RaiseAndSetIfChanged(ref _selectedInstance, value);
                }
            }
        }
        
        
        private string? _customInstanceUrl;
        public string? CustomInstanceUrl
        {
            get => _customInstanceUrl;
            set => this.RaiseAndSetIfChanged(ref _customInstanceUrl, value);
        }

        private static string? _selectedVideoQuality;
        public string? SelectedVideoQuality
        {
            get => _selectedVideoQuality;
            set => this.RaiseAndSetIfChanged(ref _selectedVideoQuality, value);
        }

        public SettingsViewModel()
        {
            // Initialize the settings on construction
            _ = InitializeSettings();
        }

        private async Task InitializeSettings()
        {
            // Load the settings from the JSON file
            var loadedSettings = await AppSettings.LoadSettings();
            Console.WriteLine("Loaded settings: "+loadedSettings.Instance + " - " + loadedSettings.VideoQuality);
            // Apply the loaded settings to the ViewModel properties
            SelectedInstance = loadedSettings.Instance;

            SelectedVideoQuality = VideoQualityOptions.Contains(loadedSettings.VideoQuality)
                ? loadedSettings.VideoQuality
                : VideoQualityOptions[0]; // Default to the first option
        }

        private void OpenCustomInstanceDialog()
        {
            DialogHostAvalonia.DialogHost.Show(this, "CustomInstanceDialogHost");
        }
    }
}
