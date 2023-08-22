using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class RecordingPage : Page
    {
        RecordingViewModel ViewModel = ((App)Application.Current).ViewModelLocator.RecordingVM;
        public RecordingPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
