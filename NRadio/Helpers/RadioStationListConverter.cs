﻿using System;
using System.Collections.Generic;
using NRadio.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace NRadio.Helpers
{
    public class RadioStationListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (ItemClickEventArgs)value;
            var clickedItem = (RadioStation)args.ClickedItem;

            var stations = (List<RadioStation>)parameter;
            return (clickedItem, stations);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}