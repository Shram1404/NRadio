
namespace NRadio.Resources
{
	using GalaSoft.MvvmLight;
	using System.Globalization;
	using NRadio.Resources.Core;
	using System;

	/// <summary>
    /// Provides access to resources from Resources.resw file.
    /// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Yet it will be unstatic.")]
	public class Resources
	{
		/// <summary>
        /// Contains logic for accessing contsnt of resource file.
        /// </summary>
		private static ResourceProvider resourceProvider = new ResourceProvider("NRadio.Resources/Resources");
		
		/// <summary>
        /// Overrides the current thread's CurrentUICulture property for all
        /// resource lookups using this strongly typed resource class.
        /// </summary>
        public static CultureInfo Culture
        {
            get
            {
                return resourceProvider.OverridedCulture;
            }
            set
            {
                resourceProvider.OverridedCulture = value;
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Best radio"
        /// </summary>
		public static string AppDescription
        {
            get
            {
                return resourceProvider.GetString("AppDescription");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "NRadio"
        /// </summary>
		public static string AppDisplayName
        {
            get
            {
                return resourceProvider.GetString("AppDisplayName");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "All Radio"
        /// </summary>
		public static string Browse_All_Radio_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_All_Radio_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "By Language"
        /// </summary>
		public static string Browse_ByLanguage_Radio_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_ByLanguage_Radio_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "By Location"
        /// </summary>
		public static string Browse_ByLocation_Radio_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_ByLocation_Radio_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Favorites"
        /// </summary>
		public static string Browse_Favorites_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Favorites_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Local"
        /// </summary>
		public static string Browse_Local_Radio_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Local_Radio_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Music"
        /// </summary>
		public static string Browse_Music_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Music_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "News & Talk"
        /// </summary>
		public static string Browse_NewsAndTalk_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_NewsAndTalk_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Podcasts"
        /// </summary>
		public static string Browse_Podcasts_Radio_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Podcasts_Radio_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Premium"
        /// </summary>
		public static string Browse_Premium_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Premium_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Recent"
        /// </summary>
		public static string Browse_Recent_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Recent_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Sports"
        /// </summary>
		public static string Browse_Sports_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Sports_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Trending"
        /// </summary>
		public static string Browse_Trending_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Browse_Trending_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "It looks like your subscription hasn't expired yet"
        /// </summary>
		public static string Dialog_AlreadyPurchasedContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_AlreadyPurchased/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "OK"
        /// </summary>
		public static string Dialog_AlreadyPurchasedOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_AlreadyPurchased/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "You already have a subscription"
        /// </summary>
		public static string Dialog_AlreadyPurchasedTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_AlreadyPurchased/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "We are updating the list of stations for the first time, it may take up to 1 minute. If you have any problems with the list of stations or want to update it, you can do it here: Settings -> Update stations"
        /// </summary>
		public static string Dialog_NeedStationsUpdateContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_NeedStationsUpdate/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "OK"
        /// </summary>
		public static string Dialog_NeedStationsUpdateOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_NeedStationsUpdate/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Looks like you need to update the station list"
        /// </summary>
		public static string Dialog_NeedStationsUpdateTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_NeedStationsUpdate/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "We're sorry, but you can't access premium stations because you don't have an active premium license. You can purchase a premium license in the "Settings""
        /// </summary>
		public static string Dialog_PremiumNotActiveContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_PremiumNotActive/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "OK"
        /// </summary>
		public static string Dialog_PremiumNotActiveOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_PremiumNotActive/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Premium is not active"
        /// </summary>
		public static string Dialog_PremiumNotActiveTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_PremiumNotActive/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Thank you for purchasing a subscription!"
        /// </summary>
		public static string Dialog_PurchaseCompleteContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseComplete/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "OK"
        /// </summary>
		public static string Dialog_PurchaseCompleteOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseComplete/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Successfully purchased"
        /// </summary>
		public static string Dialog_PurchaseCompleteTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseComplete/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "In this case, your subscription will expire in the following number of days - "
        /// </summary>
		public static string Dialog_PurchaseConfirmContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseConfirm/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "No, mayber later"
        /// </summary>
		public static string Dialog_PurchaseConfirmNo
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseConfirm/No");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Purchase of a subscription - "
        /// </summary>
		public static string Dialog_PurchaseConfirmTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseConfirm/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Yes"
        /// </summary>
		public static string Dialog_PurchaseConfirmYes
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseConfirm/Yes");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Something seems went wrong, please try again later"
        /// </summary>
		public static string Dialog_PurchaseFailedContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseFailed/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "OK"
        /// </summary>
		public static string Dialog_PurchaseFailedOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseFailed/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Unable to purchase a subscription"
        /// </summary>
		public static string Dialog_PurchaseFailedTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_PurchaseFailed/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "You can do this by saying "Radio" and then hearing a voice response - say one of these commands: "Play" | "Pause" to pause or resume playback; "Next" | "Previous" - to switch between stations; "Mute" | "Unmute" to mute and unmute the sound; If your command is not recognized, you will hear a double beep."
        /// </summary>
		public static string Dialog_ShowVoiceCommandsListContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_ShowVoiceCommandsList/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Ok"
        /// </summary>
		public static string Dialog_ShowVoiceCommandsListOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_ShowVoiceCommandsList/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "You can control the player with your voice."
        /// </summary>
		public static string Dialog_ShowVoiceCommandsListTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_ShowVoiceCommandsList/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "It's possible that the voice recognition component on your device is damaged or you don't have a language pack.  To fix this problem, go to Windows Settings, then Time & language - Add languages - English (United States). You may also need to turn on voice recognition using this path. Accessibility - Speech - Windows Speech Recognition."
        /// </summary>
		public static string Dialog_SpeechComponentErrorContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_SpeechComponentError/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Ok"
        /// </summary>
		public static string Dialog_SpeechComponentErrorOk
        {
            get
            {
                return resourceProvider.GetString("Dialog_SpeechComponentError/Ok");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "There is an error with your Speech Recognition drivers"
        /// </summary>
		public static string Dialog_SpeechComponentErrorTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_SpeechComponentError/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "When you refresh, the list of your recent stations and favorites will also be updated. You will also need to wait about a minute"
        /// </summary>
		public static string Dialog_StationsUpdateConfirmContent
        {
            get
            {
                return resourceProvider.GetString("Dialog_StationsUpdateConfirm/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "No, no, thanks"
        /// </summary>
		public static string Dialog_StationsUpdateConfirmNo
        {
            get
            {
                return resourceProvider.GetString("Dialog_StationsUpdateConfirm/No");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Do you really want to update the station list?"
        /// </summary>
		public static string Dialog_StationsUpdateConfirmTitle
        {
            get
            {
                return resourceProvider.GetString("Dialog_StationsUpdateConfirm/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Yes, i agree"
        /// </summary>
		public static string Dialog_StationsUpdateConfirmYes
        {
            get
            {
                return resourceProvider.GetString("Dialog_StationsUpdateConfirm/Yes");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Thanks for downloading NRadio!"
        /// </summary>
		public static string FirstRun_BodyText
        {
            get
            {
                return resourceProvider.GetString("FirstRun_Body/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Ok"
        /// </summary>
		public static string FirstRunDialogPrimaryButtonText
        {
            get
            {
                return resourceProvider.GetString("FirstRunDialog/PrimaryButtonText");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Welcome"
        /// </summary>
		public static string FirstRunDialogTitle
        {
            get
            {
                return resourceProvider.GetString("FirstRunDialog/Title");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "ListDetails"
        /// </summary>
		public static string ListDetailsListHeader
        {
            get
            {
                return resourceProvider.GetString("ListDetails/ListHeader");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Select an item from the list."
        /// </summary>
		public static string ListDetails_NoSelectionText
        {
            get
            {
                return resourceProvider.GetString("ListDetails_NoSelection/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Click to log in"
        /// </summary>
		public static string LogInButtonContent
        {
            get
            {
                return resourceProvider.GetString("LogInButton/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "LogIn"
        /// </summary>
		public static string LogInPageTitleText
        {
            get
            {
                return resourceProvider.GetString("LogInPageTitle/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Favorites"
        /// </summary>
		public static string Main_FavoriteStationsText
        {
            get
            {
                return resourceProvider.GetString("Main_FavoriteStations/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Local"
        /// </summary>
		public static string Main_LocalStationsText
        {
            get
            {
                return resourceProvider.GetString("Main_LocalStations/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Recent"
        /// </summary>
		public static string Main_RecentStationsText
        {
            get
            {
                return resourceProvider.GetString("Main_RecentStations/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Language"
        /// </summary>
		public static string Setting_Language_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Setting_Language_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "About this application"
        /// </summary>
		public static string Settings_AboutText
        {
            get
            {
                return resourceProvider.GetString("Settings_About/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "This is my first UWP app."
        /// </summary>
		public static string Settings_AboutDescriptionText
        {
            get
            {
                return resourceProvider.GetString("Settings_AboutDescription/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Language"
        /// </summary>
		public static string Settings_LanguageText
        {
            get
            {
                return resourceProvider.GetString("Settings_Language/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Log out"
        /// </summary>
		public static string Settings_LogOutContent
        {
            get
            {
                return resourceProvider.GetString("Settings_LogOut/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Personalization"
        /// </summary>
		public static string Settings_PersonalizationText
        {
            get
            {
                return resourceProvider.GetString("Settings_Personalization/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Premium benefits"
        /// </summary>
		public static string Settings_PremiumText
        {
            get
            {
                return resourceProvider.GetString("Settings_Premium/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Buy premium"
        /// </summary>
		public static string Settings_Premium_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Settings_Premium_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Privacy Statement"
        /// </summary>
		public static string Settings_PrivacyTermsLinkContent
        {
            get
            {
                return resourceProvider.GetString("Settings_PrivacyTermsLink/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "https://YourPrivacyUrlGoesHere/"
        /// </summary>
		public static string Settings_PrivacyTermsLinkNavigateUri
        {
            get
            {
                return resourceProvider.GetString("Settings_PrivacyTermsLink/NavigateUri");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Radio list settings"
        /// </summary>
		public static string Settings_RadioListSettingsText
        {
            get
            {
                return resourceProvider.GetString("Settings_RadioListSettings/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Choose Theme"
        /// </summary>
		public static string Settings_ThemeText
        {
            get
            {
                return resourceProvider.GetString("Settings_Theme/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Dark"
        /// </summary>
		public static string Settings_Theme_DarkContent
        {
            get
            {
                return resourceProvider.GetString("Settings_Theme_Dark/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Windows default"
        /// </summary>
		public static string Settings_Theme_DefaultContent
        {
            get
            {
                return resourceProvider.GetString("Settings_Theme_Default/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Light"
        /// </summary>
		public static string Settings_Theme_LightContent
        {
            get
            {
                return resourceProvider.GetString("Settings_Theme_Light/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Update list"
        /// </summary>
		public static string Settings_UpdateStationsContent
        {
            get
            {
                return resourceProvider.GetString("Settings_UpdateStations/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Voice control settings"
        /// </summary>
		public static string Settings_VoiceControllText
        {
            get
            {
                return resourceProvider.GetString("Settings_VoiceControll/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Enable voice control"
        /// </summary>
		public static string Settings_VoiceControll_ButtonContent
        {
            get
            {
                return resourceProvider.GetString("Settings_VoiceControll_Button/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Browse"
        /// </summary>
		public static string Shell_BrowseContent
        {
            get
            {
                return resourceProvider.GetString("Shell_Browse/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Home"
        /// </summary>
		public static string Shell_MainContent
        {
            get
            {
                return resourceProvider.GetString("Shell_Main/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Player"
        /// </summary>
		public static string Shell_PlayerContent
        {
            get
            {
                return resourceProvider.GetString("Shell_Player/Content");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Search"
        /// </summary>
		public static string Shell_SearchContent
        {
            get
            {
                return resourceProvider.GetString("Shell_Search/Content");
            }
        }

        /// <summary>
        /// Gets a localized string similar to "User info"
        /// </summary>
        public static string Shell_UserInfoButton_AutomationPropertiesName
        {
            get
            {
                return resourceProvider.GetString("Shell_UserInfoButton/[using:Windows/UI/Xaml/Automation]AutomationProperties/Name");
            }
        }

        /// <summary>
        /// Gets a localized string similar to "User info"
        /// </summary>
        public static string Shell_UserInfoButton_ToolTipServiceToolTip
        {
            get
            {
                return resourceProvider.GetString("Shell_UserInfoButton/[using:Windows/UI/Xaml/Controls]ToolTipService/ToolTip");
            }
        }

        /// <summary>
        /// Gets a localized string similar to "Bitrate"
        /// </summary>
        public static string StationDetail_BitrateText
        {
            get
            {
                return resourceProvider.GetString("StationDetail_Bitrate/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Categories"
        /// </summary>
		public static string StationDetail_CategoriesText
        {
            get
            {
                return resourceProvider.GetString("StationDetail_Categories/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Country"
        /// </summary>
		public static string StationDetail_CountryText
        {
            get
            {
                return resourceProvider.GetString("StationDetail_Country/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Country code"
        /// </summary>
		public static string StationDetail_CountryCodeText
        {
            get
            {
                return resourceProvider.GetString("StationDetail_CountryCode/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Language"
        /// </summary>
		public static string StationDetail_LanguageText
        {
            get
            {
                return resourceProvider.GetString("StationDetail_Language/Text");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "Something went wrong during the login, please try again."
        /// </summary>
		public static string StatusLoginFails
        {
            get
            {
                return resourceProvider.GetString("StatusLoginFails");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "There is no internet connection, please connect to the internet and try again."
        /// </summary>
		public static string StatusNoNetworkAvailable
        {
            get
            {
                return resourceProvider.GetString("StatusNoNetworkAvailable");
            }
        }

		/// <summary>
        /// Gets a localized string similar to "User is unauthorized"
        /// </summary>
		public static string StatusUnauthorized
        {
            get
            {
                return resourceProvider.GetString("StatusUnauthorized");
            }
        }

	}


	public sealed class LocalizedStrings : ObservableObject
    {
		/// <summary>
        /// Initialize a new instance of <see cref="LocalizedStrings"/> class.
        /// </summary>
        public LocalizedStrings()
        {
            this.RefreshLanguageSettings();
        }

		public static event EventHandler LocalizedStringsRefreshedEvent;

		public void OnLocalizedStringsRefreshedEvent()
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = LocalizedStringsRefreshedEvent;

            // Event will be null if there are no subscribers 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, new EventArgs());
            }
        }

		        /// <summary>
		/// Gets resources that are common across application.
		/// </summary>
		public Resources Resources { get; private set; }

	
		public void RefreshLanguageSettings()
        {
			            this.Resources = new Resources();
			this.RaisePropertyChanged("Resources");
		

			OnLocalizedStringsRefreshedEvent();
		}
	}
}