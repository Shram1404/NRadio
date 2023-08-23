using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Models;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.Views;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        private readonly List<RadioStation> allStations = RadioStationsContainer.AllStations;
        private List<RadioStation> stations;

        public BrowseViewModel()
        {
            GoToCommand = new AsyncRelayCommand<BrowseBy>(GoToSortedListPage);
        }

        public ICommand GoToCommand { get; private set; }
        public Type EnumType => typeof(BrowseBy);
        public string Name { get; set; }

        public List<RadioStation> Stations
        {
            get => stations;
            private set => SetProperty(ref stations, value);
        }

        private async Task GoToSortedListPage(BrowseBy sortBy)
        {
            if (allStations != null && allStations.Count > 0)
            {
                switch (sortBy)
                {
                    case BrowseBy.Premium:
                        var purchaseProvider = ((App)Application.Current).purchaseProvider;
                        if (await purchaseProvider.CheckIfUserHasPremiumAsync())
                        {
                            Stations = new List<RadioStation>(RadioStationsContainer.PremiumStations);
                        }
                        else
                        {
                            await DialogService.PremiumNotActiveDialogAsync();
                        }

                        break;
                    case BrowseBy.Local:
                        Stations = new List<RadioStation>(allStations.Where(s => s.CountryCode.ToUpper() == LocationService.GetCountryCode()));
                        break;
                    case BrowseBy.Recent:
                        Stations = RadioStationsContainer.RecentStations;
                        break;
                    case BrowseBy.Favorites:
                        Stations = RadioStationsContainer.FavoriteStations;
                        break;
                    case BrowseBy.Trending:
                        Stations = new List<RadioStation>(allStations.Where(s => s.Tags.Contains("trending")));
                        break;
                    case BrowseBy.Music:
                        Stations = new List<RadioStation>(allStations.Where(s => s.Tags.Contains("music")));
                        break;
                    case BrowseBy.Sports:
                        Stations = new List<RadioStation>(allStations.Where(s => s.Tags.Contains("sport")));
                        break;
                    case BrowseBy.NewsAndTalk:
                        Stations = new List<RadioStation>(allStations.Where(s => s.Tags.Contains("news")
                        || s.Tags.Contains("talk")));
                        break;
                    case BrowseBy.Podcasts:
                        Stations = new List<RadioStation>(allStations.Where(s => s.Tags.Contains("podcast")));
                        break;
                }

            ((App)Application.Current).ViewModelLocator.StationsListVM.LoadData(Stations);
                NavigationService.Navigate<StationsListPage>();
            }
            else
            {
                await RadioStationsLoader.ShowUpdateStationsMessageAsync();
            }
        }

        public enum BrowseBy
        {
            Premium,
            Local,
            Recent,
            Favorites,
            Trending,
            Music,
            Sports,
            NewsAndTalk,
            Podcasts,
            Location,
            Language,
        }
    }
}
