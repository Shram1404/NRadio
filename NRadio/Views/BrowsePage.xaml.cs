using System.Diagnostics;
using NRadio.ViewModels;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class BrowsePage : Page
    {
        public BrowseViewModel ViewModel { get; } = new BrowseViewModel();

        public BrowsePage()
        {
            Debug.WriteLine("BrowsePage created");
            InitializeComponent();
        }
    }
}
