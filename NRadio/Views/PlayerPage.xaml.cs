using System;
using NRadio.Core.Helpers;
using NRadio.ViewModels;

using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class PlayerPage : Page
    {
        public PlayerViewModel ViewModel { get; } = new PlayerViewModel(RadioStationsContainer.AllStations, 0);

        public PlayerPage()
        {
            InitializeComponent();
            Loaded += PlayerPage_Loaded;
        }

        private void PlayerPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
 
        }
    }
}
