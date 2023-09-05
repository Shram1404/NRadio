using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class LogInPage : Page
    {
        public LogInViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.LogInVM;

        public LogInPage()
        {
            InitializeComponent();
        }
    }
}
