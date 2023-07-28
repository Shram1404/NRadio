using System;

using NRadio.ViewModels;

using Windows.UI.Xaml.Controls;

namespace NRadio.Views
{
    public sealed partial class LogInPage : Page
    {
        public LogInViewModel ViewModel { get; } = new LogInViewModel();

        public LogInPage()
        {
            InitializeComponent();
        }
    }
}
