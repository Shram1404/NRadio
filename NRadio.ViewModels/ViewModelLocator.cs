using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NRadio.Helpers;
using NRadio.Models;

namespace NRadio.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            ViewModelLocatorHelper.Initialize(GetKeyValuePairs());
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

        private Dictionary<Type, ViewModelType.VM> GetKeyValuePairs()
        {
            var keyValuePairs = new Dictionary<Type, ViewModelType.VM>
            {
                { typeof(UserViewModel), ViewModelType.VM.UserVM },
            };

            return keyValuePairs;
        }
    }
}
