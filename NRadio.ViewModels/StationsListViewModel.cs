using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;
using NRadio.Helpers;
using NRadio.Models;
using NRadio.Services;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class StationsListViewModel : ObservableObject
    {
        private readonly ViewModelLocator vml;

        private ICommand itemClickCommand;
        private List<RadioStation> playlist = new List<RadioStation>();
        private IncrementalLoadingCollection<IncrementalPlaylist, RadioStation> incrementalPlaylist;

        public StationsListViewModel(IServiceProvider serviceProvider)
        {
            System.Diagnostics.Debug.WriteLine("StationsListViewModel created");

            vml = serviceProvider.GetService<ViewModelLocator>();
        }

        public ICommand ItemClickCommand => itemClickCommand ?? (itemClickCommand = new RelayCommand<RadioStation>(OnItemClick));

        public List<RadioStation> Playlist
        {
            get => playlist;
            set => SetProperty(ref playlist, value);
        }
        public IncrementalLoadingCollection<IncrementalPlaylist, RadioStation> IncrementalPlaylist
        {
            get => incrementalPlaylist;
            set => SetProperty(ref incrementalPlaylist, value);
        }

        public void LoadData(List<RadioStation> stations)
        {
            Playlist = new List<RadioStation>();

            var data = stations;
            if (data != null)
            {
                foreach (var item in data)
                {
                    Playlist.Add(item);
                }

                IncrementalPlaylist = new IncrementalLoadingCollection<IncrementalPlaylist, RadioStation>(new IncrementalPlaylist(Playlist));
            }
            else
            {
                throw new System.ArgumentException("Failed to load stations");
            }
        }

        private void OnItemClick(RadioStation clickedItem)
        {
            if (clickedItem != null)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(clickedItem);
                NavigationService.Navigate(NavigationTarget.Target.StationDetailPage, clickedItem.Name);

                vml.StationDetailVM.Initialize(Playlist, clickedItem, Playlist.IndexOf(clickedItem));
            }
        }
    }
}
