using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CoaLoad.Helpers;
using CoaLoad.ViewModels;
using DialogHostAvalonia;

namespace CoaLoad.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        private void CustomInstance_OnDialogClosing(object? sender, DialogClosingEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Parameter?.ToString()))
            {
                InstanceComboBox.SelectedIndex = 0;
            }
            Console.WriteLine($"Dialog host value: {e.Parameter ?? string.Empty}");
            // Set the last value of InstanceOptions to the entered value
            SettingsViewModel.InstanceOptions[SettingsViewModel.InstanceOptions.Count - 1] = "Custom: " + e.Parameter?.ToString() ?? string.Empty;
            InstanceComboBox.SelectedIndex = SettingsViewModel.InstanceOptions.Count - 1;
            
            
        }


        private async void SaveSettingsButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var newSettings = new AppSettings.SettingsValues
            {
                Instance = SettingsViewModel._selectedInstance,
                VideoQuality = SettingsViewModel._selectedVideoQuality
            };
            var res = await AppSettings.SaveSettings(newSettings);
            if (res)
            {
                SaveButton.Foreground = Brushes.LawnGreen;
                SaveButtonTextBox.Text = "Saved!";
                await Task.Delay(500);
                Content = new MainView();
            }
        }

        private void HomeButtonClicked(object? sender, RoutedEventArgs e)
        {
            Content = new MainView();
        }
    }
}