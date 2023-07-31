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

namespace NRadio.ViewModels
{
    public class ContentGridViewModel : ObservableObject
    {
        private ICommand _itemClickCommand;

        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<RadioStation>(OnItemClick));

        public ObservableCollection<RadioStation> Source { get; } = new ObservableCollection<RadioStation>();

        public ContentGridViewModel()
        {
            System.Diagnostics.Debug.WriteLine("ContentGridViewModel created");
        }

        public async Task LoadDataAsync()
        {
            Source.Clear();

            var data = RadioStationsContainer.AllStations;
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
                NavigationService.Navigate<ContentGridDetailPage>(clickedItem.Name);
            }
        }
    }
}
