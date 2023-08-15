using NRadio.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.ShellVM;

        public ShellPage()
        {
            Debug.WriteLine("ShellPage created");
            InitializeComponent();
            DataContext = ViewModel;
            Debug.WriteLine("ShellPage DataContext - " + DataContext);
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }
    }
}
