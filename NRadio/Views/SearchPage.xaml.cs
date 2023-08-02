using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class SearchPage : Page
    {
        SearchViewModel ViewModel = new SearchViewModel();

        public SearchPage()
        {
            Debug.WriteLine("SearchPage created");
            this.InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
