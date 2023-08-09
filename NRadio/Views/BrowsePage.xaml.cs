using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class BrowsePage : Page
    {
        public BrowseViewModel ViewModel { get; set; } = new BrowseViewModel();

        public BrowsePage()
        {
            Debug.WriteLine("BrowsePage created");
            this.InitializeComponent();
        }
    }
}
