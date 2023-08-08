using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace NRadio.Views
{
    public sealed partial class MiniPlayerPage : Page
    {
        PlayerViewModel ViewModel = ((App)Application.Current).ViewModelLocator.PlayerVM;
        public MiniPlayerPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
