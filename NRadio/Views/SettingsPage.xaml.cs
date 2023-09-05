using System.Diagnostics;
using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; set; } = ((App)Application.Current).ViewModelLocator.SettingsVM;

        public SettingsPage()
        {
            Debug.WriteLine("SettingsPage created");
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
