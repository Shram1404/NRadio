using Microsoft.Toolkit.Uwp.UI.Animations;

using NRadio.Services;
using NRadio.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NRadio.Views
{
    public sealed partial class StationDetailPage : Page
    {
        public StationDetailViewModel ViewModel { get; } = ((App)Application.Current).ViewModelLocator.StationDetailVM;

        public StationDetailPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) // TODO: Change to behaviors
        {
            base.OnNavigatedTo(e);
            this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
            if (e.Parameter is string name)
            {
                ViewModel.Initialize(name);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(ViewModel.CurrentStation);
            }
        }
    }
}
