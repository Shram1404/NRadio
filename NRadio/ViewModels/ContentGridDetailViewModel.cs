using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using NRadio.Core.Helpers;
using NRadio.Core.Models;
using NRadio.Core.Services;

namespace NRadio.ViewModels
{
    public class ContentGridDetailViewModel : ObservableObject
    {
        private RadioStation _item;

        public RadioStation Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public ContentGridDetailViewModel()
        {
            System.Diagnostics.Debug.WriteLine("ContentGridDetailViewModel created");
        }

        public void Initialize(string name)
        {
            var data = RadioStationsContainer.AllStations;
            Item = data.FirstOrDefault(i => i.Name == name);
        }
    }
}
