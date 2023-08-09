using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.MainVM;

        public MainPage()
        {
            Debug.WriteLine("MainPage created");
            InitializeComponent();
        }
    }
}
