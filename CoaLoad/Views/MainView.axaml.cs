using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace CoaLoad.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void DownloadButtonClicked(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Download button clicked");
        
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


}