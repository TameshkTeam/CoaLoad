using System;
using System.Linq;
using System.Security.Cryptography;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CoaLoad.Helpers;
using CoaLoad.ViewModels;

namespace CoaLoad.Views;

public partial class MainView : UserControl
{
    private DispatcherTimer _notificationTimer;
    private Brush _originalTextBoxBorderBrush;

    public MainView()
    {
        InitializeComponent();
        _notificationTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };
        _notificationTimer.Tick += NotificationTimer_Tick;
    }

    private async void DownloadButtonClicked(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(UrlInputBox.Text)) // Check if TextBox is empty
        {
            _originalTextBoxBorderBrush = UrlInputBox.BorderBrush as Brush;
            UrlInputBox.BorderBrush = Brushes.IndianRed;
            ShowNotification("Please paste a link first.", 2000);
        }
        else
        {
            var settings = await AppSettings.LoadSettings();
            var filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filename = $"{new Random().Next(10000, 99999)}.mp4";
            await DownloadFromCobalt.Download(settings.Instance, settings.VideoQuality, UrlInputBox.Text, filepath + "/" + filename);
            
        }
    }

    private void ShowNotification(string message, int duration)
    {
        UrlInputBox.BorderBrush = Brushes.IndianRed;
        NotificationMessage.Text = message;
        NotificationProgressBar.Maximum = duration;
        NotificationProgressBar.Value = duration;
        NotificationPopup.IsOpen = true;

        // Calculate the offset to ensure the popup stays within the window
        _notificationTimer.Stop();
        _notificationTimer.Start();
    }


    private void NotificationTimer_Tick(object sender, EventArgs e)
    {
        NotificationProgressBar.Value -= _notificationTimer.Interval.TotalMilliseconds;

        if (NotificationProgressBar.Value <= 0)
        {
            _notificationTimer.Stop();
            UrlInputBox.BorderBrush = _originalTextBoxBorderBrush;
            NotificationPopup.IsOpen = false;
        }
    }


    private void AutoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AutoButton.IsChecked == false)
        {
            AutoButton.IsChecked = true; // Prevent deselecting the button
            return;
        }

        MuteButton.IsChecked = false;
        AudioButton.IsChecked = false;
        AutoIcon.Fill = new SolidColorBrush(Colors.Black);
        AudioIcon.Fill = new SolidColorBrush(Colors.White);
        MuteIcon.Fill = new SolidColorBrush(Colors.White);
    }

    private void AudioButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AudioButton.IsChecked == false)
        {
            AudioButton.IsChecked = true; // Prevent deselecting the button
            return;
        }

        AutoButton.IsChecked = false;
        MuteButton.IsChecked = false;
        AutoIcon.Fill = new SolidColorBrush(Colors.White);
        AudioIcon.Fill = new SolidColorBrush(Colors.Black);
        MuteIcon.Fill = new SolidColorBrush(Colors.White);
    }

    private void MuteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (MuteButton.IsChecked == false)
        {
            MuteButton.IsChecked = true; // Prevent deselecting the button
            return;
        }

        AutoButton.IsChecked = false;
        AudioButton.IsChecked = false;
        AutoIcon.Fill = new SolidColorBrush(Colors.White);
        AudioIcon.Fill = new SolidColorBrush(Colors.White);
        MuteIcon.Fill = new SolidColorBrush(Colors.Black);
    }

    private async void ClipboardButtonClicked(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        UrlInputBox.Text = await clipboard!.GetTextAsync();
    }

    private void SettingsButton_OnClick(object? sender, RoutedEventArgs e)
    {

            this.Content = new SettingsView();
        
    }
}