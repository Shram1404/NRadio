using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Models;
using NRadio.Core.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NRadio.ViewModels
{
    public class HorizontalItemScrollViewModel : ObservableObject
    {
        private const int MinOffset = 0;
        private const int MoveOffset = 200;

        private readonly ViewModelLocator vml;

        private double horizontalOffset;
        private List<RadioStation> source;

        public HorizontalItemScrollViewModel(IServiceProvider serviceProvider)
        {
            vml = new ViewModelLocator(serviceProvider);
        }

        public ICommand ScrollLeftCommand => new RelayCommand(() => HorizontalOffset -= MoveOffset);
        public ICommand ScrollRightCommand => new RelayCommand(() => HorizontalOffset += MoveOffset);
        public ICommand ItemClickCommand => new RelayCommand<(RadioStation station, List<RadioStation> stations)>(OnClickAtStation);

        public double HorizontalOffset
        {
            get => horizontalOffset;
            set
            {
                value = Math.Max(MinOffset, value);
                SetProperty(ref horizontalOffset, value);
            }
        }
        public List<RadioStation> Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }

        private void OnClickAtStation((RadioStation clickedItem, List<RadioStation> thisSource) args)
        {
            var (clickedItem, thisSource) = args;
            NavigationService.Navigate(NavigationTarget.Target.StationDetailPage, clickedItem.Name);
            vml.StationDetailVM.Initialize(thisSource, clickedItem, source.IndexOf(clickedItem));
        }
    }
}