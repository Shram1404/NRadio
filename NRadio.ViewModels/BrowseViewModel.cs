using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Purchase;
using NRadio.Core.Services;
using NRadio.Helpers;
using NRadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NRadio.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        private readonly ViewModelLocator vml;

        private readonly List<RadioStation> allStations = RadioStationsContainer.AllStations;
        private List<RadioStation> stations;

        public BrowseViewModel(IServiceProvider serviceProvider)
        {
            GoToCommand = new AsyncRelayCommand<BrowseBy>(GoToSortedListPage);

            vml = serviceProvider.GetService<ViewModelLocator>();
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
                        var purchaseProvider = PurchaseService.PurchaseProvider;
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

                vml.StationsListVM.LoadData(Stations);
                NavigationService.Navigate(NavigationTarget.Target.StationsListPage);
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
