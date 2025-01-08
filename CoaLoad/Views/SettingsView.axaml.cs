using Avalonia.Controls;
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
            System.Console.WriteLine($"Dialog host value: {e.Parameter ?? string.Empty}");
            InstanceComboBox.SelectedIndex = 0;
        }
    }
}