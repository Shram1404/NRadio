using Microsoft.Extensions.DependencyInjection;
using NRadio.Helpers;
using NRadio.Models;
using System;
using System.Collections.Generic;

namespace NRadio.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            ViewModelLocatorHelper.Initialize(GetTypeDictionary(), GetInstanceDictionary());
        }

        public ShellViewModel ShellVM => serviceProvider.GetService<ShellViewModel>();
        public PlayerViewModel PlayerVM => serviceProvider.GetService<PlayerViewModel>();
        public StationDetailViewModel StationDetailVM => serviceProvider.GetService<StationDetailViewModel>();
        public StationsListViewModel StationsListVM => serviceProvider.GetService<StationsListViewModel>();
        public MainViewModel MainVM => serviceProvider.GetService<MainViewModel>();
        public RecordingViewModel RecordingVM => serviceProvider.GetService<RecordingViewModel>();
        public SettingsViewModel SettingsVM => serviceProvider.GetService<SettingsViewModel>();
        public UserViewModel UserVM => serviceProvider.GetService<UserViewModel>();
        public SearchViewModel SearchVM => serviceProvider.GetService<SearchViewModel>();
        public LogInViewModel LogInVM => serviceProvider.GetService<LogInViewModel>();
        public BrowseViewModel BrowseVM => serviceProvider.GetService<BrowseViewModel>();
        public HorizontalItemScrollViewModel HorizontalItemScrollVM => serviceProvider.GetService<HorizontalItemScrollViewModel>();

        private Dictionary<Type, VMLocatorEnum.VM> GetTypeDictionary()
        {
            var keyValuePairs = new Dictionary<Type, VMLocatorEnum.VM>
            {
                { typeof(UserViewModel), VMLocatorEnum.VM.UserVM },
                { typeof(PlayerViewModel), VMLocatorEnum.VM.PlayerVM }
            };

            return keyValuePairs;
        }

        private Dictionary<dynamic, VMLocatorEnum.VM> GetInstanceDictionary()
        {
            var keyValuePairs = new Dictionary<dynamic, VMLocatorEnum.VM>
            {
                { serviceProvider.GetService<UserViewModel>(), VMLocatorEnum.VM.UserVM },
                { serviceProvider.GetService<PlayerViewModel>(), VMLocatorEnum.VM.PlayerVM }
            };

            return keyValuePairs;
        }
    }
}
