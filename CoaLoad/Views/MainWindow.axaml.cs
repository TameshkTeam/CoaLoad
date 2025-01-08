using System;
using Avalonia;
using Avalonia.Controls;
using CoaLoad.Helpers;

namespace CoaLoad.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.Opened += (sender, args) =>
        {
            CenterWindowOnDesktop();
        };
        InitializeComponent();
        AppSettings.SetupSettingsDb(TopLevel.GetTopLevel(this).StorageProvider);
    }

    private void CenterWindowOnDesktop()
    {
        if (Screens.Primary is { } primaryScreen)
        {
            var screenBounds = primaryScreen.Bounds;
            var windowSize = this.Bounds.Size;

            var centeredPosition = new PixelPoint(
                (int)((screenBounds.Width - windowSize.Width) / 2),
                (int)((screenBounds.Height - windowSize.Height) / 2)
            );

            this.Position = centeredPosition;
        }
    }
}