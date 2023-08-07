using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;
using NRadio.Services;
using NRadio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    public class BrowseViewModel : ObservableObject
    {
        public ICommand GoToCommand { get; private set; }

        private readonly ObservableCollection<RadioStation> _allStations = RadioStationsContainer.AllStations;

        private ObservableCollection<RadioStation> _stations;
        public ObservableCollection<RadioStation> Stations
        {
            get => _stations;
            private set => SetProperty(ref _stations, value);
        }

        public BrowseViewModel()
        {
            GoToCommand = new RelayCommand<string>(GoToPage);
        }

        public string Name { get; set; }

        private async void GoToPage(string sortBy)
        {
            System.Diagnostics.Debug.WriteLine("StationsListViewModel created");

            if (sortBy == "All") Stations = _allStations; // TODO: Remove in future
            else if (sortBy == "Local") Stations = new ObservableCollection<RadioStation>(_allStations.Where(s => s.CountryCode == "UA")); //TODO: Change to current locale
            else if (sortBy == "Favorites") Stations = RadioStationsContainer.FavoriteStations;
            else if (sortBy == "Recents") Stations = RadioStationsContainer.RecentStations;
            else if (sortBy == "Sports") Stations = new ObservableCollection<RadioStation>(_allStations.Where(s => s.Tags.Contains("sport")));
            else if (sortBy == "News") Stations = new ObservableCollection<RadioStation>(_allStations.Where(s => s.Tags.Contains("news")));
            else if (sortBy == "Premium")
            {
                if (false) // TODO: Check if premium
                {
                    Stations = new ObservableCollection<RadioStation>(RadioStationsContainer.PremiumStations);
                }
                else
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


                    await dialog.ShowAsync();
                    return;
                }
            }
            else Stations = new ObservableCollection<RadioStation>(_allStations);

            await ((App)Application.Current).ViewModelLocator.StationsListVM.LoadDataAsync(Stations);

            NavigationService.Navigate<StationsListPage>();
        }

    }
}
