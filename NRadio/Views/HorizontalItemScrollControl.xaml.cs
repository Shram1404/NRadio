using NRadio.ViewModels;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class HorizontalItemScrollControl : UserControl
    {
        HorizontalItemScrollViewModel ViewModel = new HorizontalItemScrollViewModel();
        public HorizontalItemScrollControl()
        {
            InitializeComponent();
        }
    }
}
