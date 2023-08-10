using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class PlayerPage : Page
    {
        public PlayerViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.PlayerVM;

        public PlayerPage()
        {
            Debug.WriteLine("PlayerPage created");
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
