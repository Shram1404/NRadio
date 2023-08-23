using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    public class SearchViewModel : ObservableObject
    {
        private string searchText;
        private UserControl stationsListUserControl;

        public SearchViewModel()
        {
            System.Diagnostics.Debug.WriteLine("SearchViewModel created");
            PropertyChanged += OnSearchTextPropertyChanged;
        }

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        public UserControl StationsListUserControl
        {
            get => stationsListUserControl;
            set => SetProperty(ref stationsListUserControl, value);
        }

        private async void OnSearchTextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchText))
            {
                await Search(SearchText);
            }
        }

        private async Task Search(string searchBy)
        {
            var stations = await Task.Run(() =>
                RadioStationsContainer.AllStations
                    .Where(s => s.Name.IndexOf(searchBy, StringComparison.OrdinalIgnoreCase) >= 0
                             || s.Tags.IndexOf(searchBy, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList());

            if (StationsListUserControl == null)
            {
                StationsListUserControl = new StationsListPage();
            }

            ((App)Application.Current).ViewModelLocator.StationsListVM.LoadData(new List<RadioStation>(stations));
        }
    }
}