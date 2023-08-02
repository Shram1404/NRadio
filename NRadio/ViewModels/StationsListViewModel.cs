using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Animations;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Services;
using NRadio.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    public class StationsListViewModel : ObservableObject
    {
        private ICommand _itemClickCommand;

        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<RadioStation>(OnItemClick));

        private ObservableCollection<RadioStation> _source = new ObservableCollection<RadioStation>();
        public ObservableCollection<RadioStation> Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        public StationsListViewModel()
        {
            System.Diagnostics.Debug.WriteLine("StationsListViewModel created");
        }

        public async Task LoadDataAsync(ObservableCollection<RadioStation> stations)
        {
            Source.Clear();

            var data = stations;
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        private void OnItemClick(RadioStation clickedItem)
        {
            if (clickedItem != null)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(clickedItem);
                NavigationService.Navigate<StationDetailPage>(clickedItem.Name);
                ((App)Application.Current).ViewModelLocator.StationDetailVM.CurrentSongIndex = Source.IndexOf(clickedItem);
                ((App)Application.Current).ViewModelLocator.StationDetailVM.Source = Source;
            }
        }
    }
}
