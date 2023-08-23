using Microsoft.Toolkit.Mvvm.ComponentModel;

using Windows.UI.Xaml.Media.Imaging;

namespace NRadio.ViewModels
{
    public class UserViewModel : ObservableObject
    {
        private string name;
        private string userPrincipalName;
        private BitmapImage photo;

        public UserViewModel()
        {
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
