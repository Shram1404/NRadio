using System;

using NRadio.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class ListDetailsPage : Page
    {
        public ListDetailsViewModel ViewModel { get; } = new ListDetailsViewModel();

        public ListDetailsPage()
        {
            InitializeComponent();
            Loaded += ListDetailsPage_Loaded;
            System.Diagnostics.Debug.WriteLine("ListDetailedView created");
        }

        private async void ListDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
        }
    }
}
