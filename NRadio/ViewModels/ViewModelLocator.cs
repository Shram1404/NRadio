using Microsoft.Extensions.DependencyInjection;
using System;

namespace NRadio.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ShellViewModel ShellVM => serviceProvider.GetService<ShellViewModel>();
        public PlayerViewModel PlayerVM => serviceProvider.GetService<PlayerViewModel>();
        public StationDetailViewModel StationDetailVM => serviceProvider.GetService<StationDetailViewModel>();
        public StationsListViewModel StationsListVM => serviceProvider.GetService<StationsListViewModel>();
        public MainViewModel MainVM => serviceProvider.GetService<MainViewModel>();
    }
}
