using System;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;

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
    
}