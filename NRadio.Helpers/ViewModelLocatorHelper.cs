using NRadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRadio.Helpers
{
    public static class ViewModelLocatorHelper
    {
        private static Dictionary<Type, VMLocatorEnum.VM> viewModels;
        private static Dictionary<dynamic, VMLocatorEnum.VM> viewModelInstances;

        public static void Initialize(Dictionary<Type, VMLocatorEnum.VM> viewModelsDictionary, 
                                      Dictionary<dynamic, VMLocatorEnum.VM> viewModelInstancesDictionary)
        {
            viewModels = viewModelsDictionary;
            viewModelInstances = viewModelInstancesDictionary;
        }

        public static Type GetViewModelType(VMLocatorEnum.VM viewModel)
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

        public static dynamic GetViewModelInstance(VMLocatorEnum.VM viewModel)
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