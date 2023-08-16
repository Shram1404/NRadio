using System.Diagnostics;
using NRadio.ViewModels;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class SearchPage : Page
    {
        SearchViewModel ViewModel = new SearchViewModel();

        public SearchPage()
        {
            Debug.WriteLine("SearchPage created");
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
