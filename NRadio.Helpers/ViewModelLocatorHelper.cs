using NRadio.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRadio.Helpers
{
    public static class ViewModelLocatorHelper
    {
        private static Dictionary<Type, VMLocator> viewModels;
        private static Dictionary<dynamic, VMLocator> viewModelInstances;

        public static void Initialize(Dictionary<Type, VMLocator> viewModelsDictionary, 
                                      Dictionary<dynamic, VMLocator> viewModelInstancesDictionary)
        {
            viewModels = viewModelsDictionary;
            viewModelInstances = viewModelInstancesDictionary;
        }

        public static Type GetViewModelType(VMLocator viewModel)
        {
            if (viewModels.ContainsValue(viewModel))
            {
                return viewModels.FirstOrDefault(x => x.Value == viewModel).Key;
            }
            else
            {
                throw new ArgumentException($"The view model {viewModel} is not registered in the ViewModelLocatorHelper as type.");
            }
        }

        public static dynamic GetViewModelInstance(VMLocator viewModel)
        {
            if (viewModelInstances.ContainsValue(viewModel))
            {
                return viewModelInstances[viewModel];
            }
            else
            {
                throw new ArgumentException($"The view model {viewModel} is not registered in the ViewModelLocatorHelper as instance.");
            }
        }
    }
}