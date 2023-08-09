using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Services;
using NRadio.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        public ICommand GoToCommand { get; private set; }

        private readonly ObservableCollection<RadioStation> allStations = RadioStationsContainer.AllStations;

        private ObservableCollection<RadioStation> stations;
        public ObservableCollection<RadioStation> Stations
        {
            get => stations;
            private set => SetProperty(ref stations, value);
        }

        public BrowseViewModel()
        {
            GoToCommand = new AsyncRelayCommand<SortByCategory>(GoToListPage);
        }

        public string Name { get; set; }

        private async Task GoToListPage(SortByCategory sortBy)
        {
            System.Diagnostics.Debug.WriteLine("StationsListViewModel created");

            switch (sortBy)
            {
                case SortByCategory.Local:
                    Stations = new ObservableCollection<RadioStation>(allStations.Where(s => s.CountryCode == "UA")); //TODO: Change to current locale
                    break;
                case SortByCategory.Favorites:
                    Stations = RadioStationsContainer.FavoriteStations;
                    break;
                case SortByCategory.Recent:
                    Stations = RadioStationsContainer.RecentStations;
                    break;
                case SortByCategory.Sports:
                    Stations = new ObservableCollection<RadioStation>(allStations.Where(s => s.Tags.Contains("sport")));
                    break;
                case SortByCategory.News:
                    Stations = new ObservableCollection<RadioStation>(allStations.Where(s => s.Tags.Contains("news")));
                    break;
                case SortByCategory.Premium:
                    if (false) // TODO: Check if premium
                    {
                        Stations = new ObservableCollection<RadioStation>(RadioStationsContainer.PremiumStations);
                    }
                    else
                    {
                        await ShowPremiumDialog();
                        return;
                    }
                    break;
                default:
                    Stations = new ObservableCollection<RadioStation>(allStations);
                    break;
            }

            await ((App)Application.Current).ViewModelLocator.StationsListVM.LoadDataAsync(Stations);

            NavigationService.Navigate<StationsListPage>();
        }


        private async Task ShowPremiumDialog()
        {
            var loader = new ResourceLoader();
            var title = loader.GetString("Premium_NotActive/Title");
            var content = loader.GetString("Premium_NotActive/Content");
            var closeButtonText = loader.GetString("Premium_NotActive/CloseButtonText");

            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText
            };
        }

        private enum SortByCategory
        {
            Local,
            Favorites,
            Recent,
            Sports,
            News,
            Premium,
        }
    }
}
