﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Controls
{
    public sealed partial class FirstRunDialogPage : ContentDialog
    {
        public FirstRunDialogPage()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }
    }
}
