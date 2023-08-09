﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            var task = Task.Run(() => RadioStationsContainer.AllStations
            .Where(s => s.Name.IndexOf(searchBy, StringComparison.OrdinalIgnoreCase) >= 0
            || s.Tags.IndexOf(searchBy, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList());
            var stations = await task;

            if (StationsListUserControl == null)
                StationsListUserControl = new StationsListPage();

            await ((App)Application.Current).ViewModelLocator.StationsListVM.LoadDataAsync(new ObservableCollection<RadioStation>(stations));
        }

        public SearchViewModel()
        {
            System.Diagnostics.Debug.WriteLine("SearchViewModel created");
        }
    }
}
