using System;
using System.Threading.Tasks;
using NRadio.Models;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace NRadio.Core.Services
{
    public static class DialogService
    {
        public static async Task<ContentDialogResult> ConfirmPurchaseDialogAsync(string productId, int days)
        {
            var dialog = new Dialog();
            var loader = new ResourceLoader();

            string titleRes = loader.GetString($"Dialog_PurchaseConfirm/Title");
            string contentRes = loader.GetString($"Dialog_PurchaseConfirm/Content");
            dialog.Title = $"{titleRes}{productId}";
            dialog.Content = $"{contentRes}{days}";
            dialog.PrimaryButtonText = loader.GetString("Dialog_PurchaseConfirm/Yes");
            dialog.CloseButtonText = loader.GetString("Dialog_PurchaseConfirm/No");

            return await ShowConfirmDialog(dialog);
        }
        public static async Task<ContentDialogResult> ConfirmStationsUpdateDialogAsync()
        {
            string resourceName = "Dialog_StationsUpdateConfirm";
            var dialog = SetConfirmDialog(resourceName);

            return await ShowConfirmDialog(dialog);
        }

        public static async Task AlreadyPurchasedDialogAsync()
        {
            string resourceName = "Dialog_AlreadyPurchased";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }
        public static async Task PurchaseFailedDialogAsync()
        {
            string resourceName = "Dialog_PurchaseFailed";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }
        public static async Task PurchaseCompleteDialogAsync()
        {
            string resourceName = "Dialog_PurchaseComplete";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }
        public static async Task PremiumNotActiveDialogAsync()
        {
            string resourceName = "Dialog_PremiumNotActive";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }
        public static async Task NeedStationsUpdateDialogAsync()
        {
            string resourceName = "Dialog_NeedStationsUpdate";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }
        public static async Task SpeechComponentErrorDialogAsync()
        {
            string resourceName = "Dialog_SpeechComponentError";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }
        public static async Task ShowVoiceCommandsListDialogAsync()
        {
            string resourceName = "Dialog_ShowVoiceCommandsList";
            var dialog = SetOkDialog(resourceName);

            await ShowOkDialog(dialog);
        }

        private static Dialog SetConfirmDialog(string resourceName)
        {
            var dialog = new Dialog();
            var loader = new ResourceLoader();

            dialog.Title = loader.GetString($"{resourceName}/Title");
            dialog.Content = loader.GetString($"{resourceName}/Content");
            dialog.PrimaryButtonText = loader.GetString($"{resourceName}/Yes");
            dialog.CloseButtonText = loader.GetString($"{resourceName}/No");

            return dialog;
        }

        private static Dialog SetOkDialog(string resourceName)
        {
            var dialog = new Dialog();
            var loader = new ResourceLoader();

            dialog.Title = loader.GetString($"{resourceName}/Title");
            dialog.Content = loader.GetString($"{resourceName}/Content");
            dialog.CloseButtonText = loader.GetString($"{resourceName}/Ok");

            return dialog;
        }

        private static async Task<ContentDialogResult> ShowConfirmDialog(Dialog d)
        {
            var dialog = new ContentDialog
            {
                Title = d.Title,
                Content = d.Content,
                PrimaryButtonText = d.PrimaryButtonText,
                CloseButtonText = d.CloseButtonText
            };
            return await dialog.ShowAsync();
        }
        private static async Task ShowOkDialog(Dialog d)
        {
            var dialog = new ContentDialog
            {
                Title = d.Title,
                Content = d.Content,
                CloseButtonText = d.CloseButtonText
            };
            await dialog.ShowAsync();
        }
    }
}
