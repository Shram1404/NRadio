using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; set; } = new SettingsViewModel();

        public SettingsPage()
        {
            Debug.WriteLine("SettingsPage created");
            InitializeComponent();
        }
    }
}
