using System;
using System.Collections.ObjectModel;
using System.Reactive;
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

        

        public static string _selectedInstance;
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

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public static string SelectedVideoQuality { get; set; }


        public SettingsViewModel()
        {
            SaveCommand = ReactiveCommand.Create(SaveSettings);
        }

        private void SaveSettings()
        {
            Console.WriteLine($"Selected Instance: {_selectedInstance}");
        }

        private void AcceptCustomInstance()
        {
            SelectedInstance = CustomInstanceUrl;
            DialogHostAvalonia.DialogHost.Close("CustomInstanceDialogHost");
        }

        private void CloseDialog()
        {
            DialogHostAvalonia.DialogHost.Close("CustomInstanceDialogHost");
        }

        private void OpenCustomInstanceDialog()
        {
            DialogHostAvalonia.DialogHost.Show(new SettingsViewModel(), "CustomInstanceDialogHost");
        }
    }
}
