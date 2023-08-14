using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Services;
using NRadio.Views;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class StationsListViewModel : ObservableObject
    {
        private ICommand itemClickCommand;
        private List<RadioStation> playlist = new List<RadioStation>();
        private IncrementalLoadingCollection<IncrementalPlaylist, RadioStation> incrementalPlaylist;

        public StationsListViewModel()
        {
            System.Diagnostics.Debug.WriteLine("StationsListViewModel created");
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
                NavigationService.Navigate<StationDetailPage>(clickedItem.Name);

                ((App)Application.Current).ViewModelLocator.StationDetailVM.Initialize(Playlist, clickedItem, Playlist.IndexOf(clickedItem));
            }
        }
    }
}
