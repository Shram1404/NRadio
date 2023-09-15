using Microsoft.Toolkit.Uwp.Helpers;
using NRadio.Models.Enum;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace NRadio.Core.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;
        private static ContentDialog firstRunDialog;

        internal static async Task ShowIfAppropriateAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    if (SystemInformation.Instance.IsFirstRun && !shown)
                    {
                        shown = true;
                        var pageType = NavigationService.GetPageType(NavigationTarget.FirstRunDialog);
                        firstRunDialog = new ContentDialog
                        {
                            Content = (Windows.UI.Xaml.UIElement)Activator.CreateInstance(pageType)
                        };

                        await firstRunDialog.ShowAsync();
                    }
                });
        }
    }
}