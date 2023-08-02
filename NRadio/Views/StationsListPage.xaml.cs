using System;
using System.Diagnostics;
using NRadio.Core.Helpers;
using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NRadio.Views
{
    public sealed partial class StationsListPage : Page
    {
        public StationsListViewModel ViewModel { get; set; } = ((App)Application.Current).ViewModelLocator.StationsListVM;

        public StationsListPage()
        {
            Debug.WriteLine("StationsListPage created");
            InitializeComponent();
        }

        ~StationsListPage()
        {
            Debug.WriteLine("StationsListPage destroyed");
        }
    }
}
