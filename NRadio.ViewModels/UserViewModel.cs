using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Windows.UI.Xaml.Media.Imaging;

namespace NRadio.ViewModels
{
    public class UserViewModel : ObservableObject
    {
        private readonly IServiceProvider serviceProvider;
        private string name;
        private string userPrincipalName;
        private BitmapImage photo;

        public UserViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string UserPrincipalName
        {
            get => userPrincipalName;
            set => SetProperty(ref userPrincipalName, value);
        }
        public BitmapImage Photo
        {
            get => photo;
            set => SetProperty(ref photo, value);
        }
    }
}
