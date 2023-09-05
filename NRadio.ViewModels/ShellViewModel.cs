using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Services;
using NRadio.Helpers;
using NRadio.Models;
using NRadio.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace NRadio.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private readonly ViewModelLocator vml;

        private readonly KeyboardAccelerator altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        private bool isPlayerCreated;
        private bool isBackEnabled;
        private IList<KeyboardAccelerator> keyboardAccelerators;
        private WinUI.NavigationView navigationView;
        private WinUI.NavigationViewItem selected;
        private ICommand loadedCommand;
        private ICommand itemInvokedCommand;
        private ICommand userProfileCommand;
        private ICommand playerCommand;
        private UserViewModel user;
        private UserControl miniPlayer;

        public ShellViewModel(IServiceProvider serviceProvider)
        {
            Debug.WriteLine("ShellVM created");
            vml = serviceProvider.GetService<ViewModelLocator>();
        }

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;
        private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        public ICommand LoadedCommand => loadedCommand ?? (loadedCommand = new RelayCommand(OnLoaded));
        public ICommand ItemInvokedCommand => itemInvokedCommand ?? (itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked));
        public ICommand UserProfileCommand => userProfileCommand ?? (userProfileCommand = new RelayCommand(OnUserProfile));
        public ICommand NavigateToPlayerCommand => playerCommand ?? (playerCommand = new RelayCommand(OnNavigateToPlayer));

        public bool IsPlayerCreated
        {
            get => isPlayerCreated;
            set => SetProperty(ref isPlayerCreated, value);
        }
        public bool IsBackEnabled
        {
            get => isBackEnabled;
            set => SetProperty(ref isBackEnabled, value);
        }
        public WinUI.NavigationViewItem Selected
        {
            get => selected;
            set => SetProperty(ref selected, value);
        }
        public UserViewModel User
        {
            get => user;
            set => SetProperty(ref user, value);
        }
        public UserControl MiniPlayer
        {
            get => miniPlayer;
            set => SetProperty(ref miniPlayer, value);
        }

        public void Initialize(Frame frame, WinUI.NavigationView navigationView, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            this.navigationView = navigationView;
            this.keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            vml.PlayerVM.IsPlayerCreatedChanged += OnIsPlayerCreatedChanged;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            this.navigationView.BackRequested += OnBackRequested;
            IdentityService.LoggedOut += OnLoggedOut;
            UserDataService.UserDataUpdated += OnUserDataUpdated;
        }

        private async void OnLoaded()
        {
            keyboardAccelerators.Add(altLeftKeyboardAccelerator);
            keyboardAccelerators.Add(backKeyboardAccelerator);
            User = await UserDataService.GetUserAsync();
        }

        private void OnUserDataUpdated(object sender, dynamic userData) => User = userData;

        private void OnLoggedOut(object sender, EventArgs e)
        {
            NavigationService.NavigationFailed -= Frame_NavigationFailed;
            NavigationService.Navigated -= Frame_Navigated;
            navigationView.BackRequested -= OnBackRequested;
            UserDataService.UserDataUpdated -= OnUserDataUpdated;
            IdentityService.LoggedOut -= OnLoggedOut;
        }

        private void OnUserProfile()
        {
            NavigationService.Navigate(NavigationTarget.Target.SettingsPage);
        }

        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(NavigationTarget.Target.SettingsPage, null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as WinUI.NavigationViewItem;
                var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;

                if (pageType != null)
                {
                    var navTarget = NavigationService.Pages[pageType];
                    NavigationService.Navigate(navTarget, null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            Type searchPage = NavigationService.Pages.FirstOrDefault(kvp => kvp.Value == NavigationTarget.Target.SearchPage).Key;
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == searchPage)
            {
                Selected = navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            var selectedItem = GetSelectedItem(navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }

        private void OnNavigateToPlayer()
        {
            NavigationService.Navigate(NavigationTarget.Target.PlayerPage);
        }

        private void OnIsPlayerCreatedChanged(object sender, EventArgs e)
        {
            var playerViewModel = sender as PlayerViewModel;
            IsPlayerCreated = playerViewModel.IsPlayerCreated;
        }
    }
}