using System;
using System.Diagnostics;
using NRadio.Core.Helpers;
using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NRadio.Views
{
    public sealed partial class PlayerPage : Page
    {
        public PlayerViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.PlayerVM;

        public PlayerPage()
        {
            Debug.WriteLine("PlayerPage created");
            InitializeComponent();
            DataContext = ViewModel;
        }

    }
}
