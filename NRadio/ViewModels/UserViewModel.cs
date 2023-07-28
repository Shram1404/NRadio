using System;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using NRadio.Helpers;

using Windows.UI.Xaml.Media.Imaging;

namespace NRadio.ViewModels
{
    public class UserViewModel : ObservableObject
    {
        private string _name;
        private string _userPrincipalName;
        private BitmapImage _photo;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string UserPrincipalName
        {
            get => _userPrincipalName;
            set => SetProperty(ref _userPrincipalName, value);
        }

        public BitmapImage Photo
        {
            get => _photo;
            set => SetProperty(ref _photo, value);
        }

        public UserViewModel()
        {
        }
    }
}
