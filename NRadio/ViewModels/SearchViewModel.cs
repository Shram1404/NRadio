using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using NRadio.Views;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using NRadio.Core.Models;
using NRadio.Core.Helpers;

namespace NRadio.ViewModels
{
    public class SearchViewModel : ObservableObject
    {
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                OnSearch(value);
            }
        }

        private StationsListPage _stationsListPage;

        private UserControl _stationsListUserControl;
        public UserControl StationsListUserControl
        {
            get => _stationsListUserControl;
            set => SetProperty(ref _stationsListUserControl, value);
        }

        private async void OnSearch(string searchBy)
        {
            ObservableCollection<RadioStation> stations = new ObservableCollection<RadioStation>(RadioStationsContainer.AllStations.
                Where(s => s.Name.ToLower().Contains(searchBy.ToLower())
                || s.Tags.ToLower().Contains(searchBy.ToLower())));


            if (_stationsListPage == null)
                _stationsListPage = new StationsListPage();

            await ((App)Application.Current).ViewModelLocator.StationsListVM.LoadDataAsync(stations);
            StationsListUserControl = _stationsListPage;
        }

        public SearchViewModel()
        {
            System.Diagnostics.Debug.WriteLine("SearchViewModel created");
        }
    }
}
