using System;

using NRadio.ViewModels;

using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class PlayerPage : Page
    {
        public PlayerViewModel ViewModel { get; } = new PlayerViewModel();

        public PlayerPage()
        {
            InitializeComponent();
        }
    }
}
