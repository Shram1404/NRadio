using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using NRadio.Helpers;
using System.Collections.Generic;

namespace NRadio.Core.Services
{
    public static class LanguageSelectorService
    {
        private const string SettingsKey = "AppLanguage";

        private static readonly Dictionary<string, string> LanguageNames = new Dictionary<string, string>
        {
            { "en-US", "English" },
            { "uk-UA", "Українська" }
        };

        public static string Language { get; set; } = "en-US";

        public static string GetCurrentLanguageName()
        {
            if (LanguageNames.TryGetValue(Language, out string name))
            {
                return name;
            }

            return Language;
        }

        public static async Task InitializeAsync()
        {
            Language = await LoadLanguageFromSettingsAsync();
        }

        public static async Task SetLanguageAsync(string language)
        {
            Language = language;

            await SetRequestedLanguageAsync();
            await SaveLanguageInSettingsAsync(Language);
        }

        public static async Task SetRequestedLanguageAsync()
        {
            foreach (var view in CoreApplication.Views)
            {
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = Language;
                });
            }
        }

        private static async Task<string> LoadLanguageFromSettingsAsync()
        {
            string cacheLanguage = "en-US";
            string languageName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(languageName))
            {
                cacheLanguage = languageName;
            }

            return cacheLanguage;
        }

        private static async Task SaveLanguageInSettingsAsync(string language)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, language);
        }
    }
}
