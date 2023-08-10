using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Services;
using NRadio.Views;
using System;
using System.Collections.Generic;
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
        private readonly List<RadioStation> allStations = RadioStationsContainer.AllStations;
        private List<RadioStation> stations;

        public BrowseViewModel()
        {
            GoToCommand = new AsyncRelayCommand<BrowseBy>(GoToSortedListPage);
        }

        public ICommand GoToCommand { get; private set; }
        public Type EnumType { get => typeof(BrowseBy); }
        public string Name { get; set; }

        public List<RadioStation> Stations
        {
            get => stations;
            private set => SetProperty(ref stations, value);
        }

        private async Task GoToSortedListPage(BrowseBy sortBy)
        {
            switch (sortBy)
            {
                case BrowseBy.Premium:
                    if (true)  // TODO: Change to real premium check
                    {
                        Stations = new List<RadioStation>(RadioStationsContainer.PremiumStations);
                    }
                    else
                    {
                        await ShowPremiumDialog();
                    }

                    break;
                case BrowseBy.Local:
                    Stations = new List<RadioStation>(allStations.Where(s => s.CountryCode == "UA")); // TODO: Change to real locale
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
                    // TODO: Add subpage with different locations and languages
            }

            ((App)Application.Current).ViewModelLocator.StationsListVM.LoadData(Stations);
            NavigationService.Navigate<StationsListPage>();
        }

        private async Task ShowPremiumDialog()
        {
            var loader = new ResourceLoader();
            string title = loader.GetString("Premium_NotActive/Title");
            string content = loader.GetString("Premium_NotActive/Content");
            string closeButtonText = loader.GetString("Premium_NotActive/CloseButtonText");

            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText
            };

            await dialog.ShowAsync();
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
