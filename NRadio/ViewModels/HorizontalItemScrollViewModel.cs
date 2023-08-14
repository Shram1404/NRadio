

using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace NRadio.ViewModels
{
    class HorizontalItemScrollViewModel : ObservableObject
    {
        private const int MinOffset = 0;
        private const int MoveOffset = 200;

        private double horizontalOffset;

        public HorizontalItemScrollViewModel()
        {

        }

        private ICommand ScrollLeftCommand => new RelayCommand(() => HorizontalOffset -= MoveOffset);

        public double HorizontalOffset
        {
            get => horizontalOffset;
            set
            {
                value = Math.Max(MinOffset, value);
                SetProperty(ref horizontalOffset, value);
            }
        }
    }
}
