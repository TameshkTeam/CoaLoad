using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CoaLoad.ViewModels;

namespace CoaLoad.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Parent is Window window)
        {
            window.Content = new MainView { DataContext = new MainViewModel() };
        }    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
    }

}