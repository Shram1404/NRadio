using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class StationsListPage : Page
    {
        public StationsListViewModel ViewModel { get; set; } = ((App)Application.Current).ViewModelLocator.StationsListVM;

        public StationsListPage()
        {
            Debug.WriteLine("StationsListPage created");
            InitializeComponent();
        }

        ~StationsListPage()
        {
            Debug.WriteLine("StationsListPage destroyed");
        }
    }
}
