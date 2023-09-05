using NRadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRadio.Helpers
{
    public static class ViewModelLocatorHelper
    {
        private static Dictionary<Type, ViewModelType.VM> viewModels;

        public static void Initialize(Dictionary<Type, ViewModelType.VM> viewModelsDictionary)
        {
            viewModels = viewModelsDictionary;
        }

        public static Type GetViewModelType(ViewModelType.VM viewModel)
        {
            return viewModels.FirstOrDefault(x => x.Value == viewModel).Key;
        }
    }
}