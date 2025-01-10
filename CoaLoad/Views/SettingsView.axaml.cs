using System;
using System.ComponentModel.DataAnnotations;
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
            if (e.Parameter == null)
            {
                Console.WriteLine("is null");
                InstanceComboBox.SelectedIndex = 0;
                return;
            }
            if (!Uri.TryCreate(e.Parameter.ToString(), UriKind.Absolute, out _))
            {
                InstanceComboBox.BorderBrush = Brushes.Red;
            }
            InstanceComboBox.PlaceholderText = e.Parameter.ToString();
            Console.WriteLine($"Dialog host value: {e.Parameter}");
            var vmSettings = (SettingsViewModel)DataContext;
            vmSettings.SelectedInstance = e.Parameter.ToString();
        }

        private async void SaveSettingsButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var vmSettings = (SettingsViewModel)DataContext;
            var newSettings = new AppSettings.SettingsValues
            {
                Instance = SettingsViewModel._selectedInstance,
                VideoQuality = vmSettings.SelectedVideoQuality
            };
            var res = await AppSettings.SaveSettings(newSettings);
            if (res)
            {
                Console.WriteLine("Settings saved: " + newSettings.Instance + " - " + newSettings.VideoQuality);
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