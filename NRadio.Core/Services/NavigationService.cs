using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using NRadio.Models;
using Windows.UI.Xaml.Navigation;
using System.Linq;

namespace NRadio.Core.Services
{
    public static class NavigationService
    {
        private static Page shellPage;
        private static Frame frame;
        private static object lastParamUsed;

        public static event NavigatedEventHandler Navigated;
        public static event NavigationFailedEventHandler NavigationFailed;

        public static Dictionary<Type, NavigationTarget.Target> Pages { get; private set; }

        public static Page ShellPage
        {
            get => shellPage ?? Window.Current.Content as Page;
            set => shellPage = value;
        }

        public static Frame Frame
        {
            get
            {
                if (frame == null)
                {
                    frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return frame;
            }

            set
            {
                UnregisterFrameEvents();
                frame = value;
                RegisterFrameEvents();
            }
        }

        public static bool CanGoBack => Frame.CanGoBack;

        public static bool CanGoForward => Frame.CanGoForward;

        public static void Initialize(Dictionary<Type, NavigationTarget.Target> pages)
        {
            Pages = pages; 
        }

        public static Type GetPageType(NavigationTarget.Target target)
        {
            if (!Pages.ContainsValue(target))
            {
                throw new ArgumentException($"Invalid navTarget '{target}', please provide a valid navTarget.", nameof(target));
            }

            return Pages.FirstOrDefault(kvp => kvp.Value == target).Key;
        }

        public static bool GoBack()
        {
            if (CanGoBack)
            {
                Frame.GoBack();
                return true;
            }

            return false;
        }

        public static void GoForward() => Frame.GoForward();

        public static bool Navigate(NavigationTarget.Target target, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            if (!Pages.ContainsValue(target))
            {
                throw new ArgumentException($"Invalid navTarget '{target}', please provide a valid navTarget.", nameof(target));
            }
            Type pageType = Pages.FirstOrDefault(kvp => kvp.Value == target).Key;

            // Don't open the same page multiple times
            if (Frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(lastParamUsed)))
            {
                bool navigationResult = Frame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult)
                {
                    lastParamUsed = parameter;
                }

                return navigationResult;
            }
            else
            {
                return false;
            }
        }

        public static bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null)
            where T : Page
        {
            NavigationTarget.Target target;
            if (!Pages.TryGetValue(typeof(T), out target))
            {
                throw new ArgumentException($"Invalid page type '{typeof(T)}', please provide a valid page type.", nameof(T));
            }

            return Navigate(target, parameter, infoOverride);
        }


        public static void Refresh()
        {
            var currentPageType = Frame.CurrentSourcePageType;
            var currentPageParameter = Frame.CurrentSourcePageType;

            bool navigationResult = Frame.Navigate(currentPageType, currentPageParameter);
            if (navigationResult)
            {
                lastParamUsed = currentPageParameter;
            }
        }

        private static void RegisterFrameEvents()
        {
            if (frame != null)
            {
                frame.Navigated += Frame_Navigated;
                frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private static void UnregisterFrameEvents()
        {
            if (frame != null)
            {
                frame.Navigated -= Frame_Navigated;
                frame.NavigationFailed -= Frame_NavigationFailed;
            }
        }

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);

        private static void Frame_Navigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);
    }
}
