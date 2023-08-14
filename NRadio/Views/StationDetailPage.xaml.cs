using NRadio.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class StationDetailPage : Page
    {
        public StationDetailViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.StationDetailVM;

        public StationDetailPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
