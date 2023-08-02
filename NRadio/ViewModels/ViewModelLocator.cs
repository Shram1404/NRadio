using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRadio.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ShellViewModel ShellVM => _serviceProvider.GetService<ShellViewModel>();
        public PlayerViewModel PlayerVM => _serviceProvider.GetService<PlayerViewModel>();
        public StationDetailViewModel StationDetailVM => _serviceProvider.GetService<StationDetailViewModel>();
        public StationsListViewModel StationsListVM => _serviceProvider.GetService<StationsListViewModel>();
    }
}
