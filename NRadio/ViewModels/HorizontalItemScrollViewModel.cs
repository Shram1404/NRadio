using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Models;
using NRadio.Services;
using NRadio.Views;
using Windows.UI.Xaml;

namespace NRadio.ViewModels
{
    class HorizontalItemScrollViewModel : ObservableObject
    {
        private const int MinOffset = 0;
        private const int MoveOffset = 200;

        private double horizontalOffset;
        private List<RadioStation> source;

        public HorizontalItemScrollViewModel() { }

        public ICommand ScrollLeftCommand => new RelayCommand(() => HorizontalOffset -= MoveOffset);
        public ICommand ScrollRightCommand => new RelayCommand(() => HorizontalOffset += MoveOffset);
        public ICommand ItemClickCommand => new RelayCommand<(RadioStation station, List<RadioStation> stations)>(OnClickAtStation);

        public List<RadioStation> Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }
        public double HorizontalOffset
        {
            get => horizontalOffset;
            set
            {
                value = Math.Max(MinOffset, value);
                SetProperty(ref horizontalOffset, value);
            }
        }

        private void OnClickAtStation((RadioStation clickedItem, List<RadioStation> thisSource) args)
        {
            var (clickedItem, thisSource) = args;
            NavigationService.Navigate<StationDetailPage>(clickedItem.Name);
            ((App)Application.Current).ViewModelLocator.StationDetailVM.Initialize(thisSource, clickedItem, source.IndexOf(clickedItem));
        }
    }
}