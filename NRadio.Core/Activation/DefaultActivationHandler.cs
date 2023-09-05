using System;
using System.Threading.Tasks;
using NRadio.Models;
using NRadio.Core.Services;
using Windows.ApplicationModel.Activation;

namespace NRadio.Core.Activation
{
    internal class DefaultActivationHandler : ActivationHandler<IActivatedEventArgs>
    {
        private readonly NavigationTarget.Target navElement;

        public DefaultActivationHandler(NavigationTarget.Target navElement)
        {
            this.navElement = navElement;
        }

        protected override async Task HandleInternalAsync(IActivatedEventArgs args)
        {
            object arguments = null;
            if (args is LaunchActivatedEventArgs launchArgs)
            {
                arguments = launchArgs.Arguments;
            }

            NavigationService.Navigate(navElement, arguments);

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(IActivatedEventArgs args)
        {
            return NavigationService.Frame.Content == null && navElement != null;
        }
    }
}
