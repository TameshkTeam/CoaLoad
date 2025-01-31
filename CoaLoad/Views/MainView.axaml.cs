using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using CoaLoad.Helpers;


namespace CoaLoad.Views;
// TODO: Get download type
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
        if (string.IsNullOrEmpty(UrlInputBox.Text))
        {
            _originalTextBoxBorderBrush = UrlInputBox.BorderBrush as Brush;
            UrlInputBox.BorderBrush = Brushes.IndianRed;
            ShowNotification("Please paste a link first.", 2000);
        }
        else
        {
            DownloadButton.IsEnabled = false;
            DownloadButtonText.Text = "Please wait...";
            var settings = await AppSettings.LoadSettings();
            var filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            if (!Directory.Exists(filepath + "/CoaLoad")) Directory.CreateDirectory(filepath + "/CoaLoad");
            var filename = $"{new Random().Next(10000, 99999)}.mp4";
            var downloadUrl = await DownloadFromCobalt.GetDownloadUrl(settings.Instance, UrlInputBox.Text);
            if (string.IsNullOrEmpty(downloadUrl))
            {
                Console.WriteLine(downloadUrl);
                ShowNotification("Failed to download the video.", 2000);

                DownloadButton.IsEnabled = true;
                DownloadButtonText.Text = "Download";
            }
            else
            {
                var progress = new Progress<double>(sizeInMb =>
                {
                    // Update your ProgressBar here
                    UrlInputBox.BorderBrush = Brushes.GreenYellow;
                    NotificationMessage.Text = sizeInMb.ToString() + " MB Downloaded";
                    NotificationMessage.TextAlignment = TextAlignment.Center;
                    //NotificationProgressBar.Maximum = 50;
                    NotificationProgressBar.Value = sizeInMb;
                    NotificationPopup.IsOpen = true;
                });
                DownloadButton.IsEnabled = false;
                DownloadButtonText.Text = "Downloading...";
                await DownloadFromCobalt.DownloadFile(downloadUrl, filepath + "/CoaLoad/" + filename, progress);
                DownloadButton.IsEnabled = true;
                DownloadButtonText.Text = "Download";
                NotificationPopup.IsOpen = false;
                ShowNotification("Download completed.", 2000);
            }
        }
    }


    private void ShowNotification(string message, int duration)
    {
        UrlInputBox.BorderBrush = Brushes.IndianRed;
        NotificationMessage.Text = message;
        NotificationProgressBar.Maximum = duration;
        NotificationProgressBar.Value = duration;
        NotificationPopup.IsOpen = true;

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
        Content = new SettingsView();
    }
}