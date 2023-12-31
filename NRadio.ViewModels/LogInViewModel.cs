﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NRadio.Core.Services;
using NRadio.Helpers;
using NRadio.Models.Enum;
using System;
using System.Threading.Tasks;

namespace NRadio.ViewModels
{
    public class LogInViewModel : ObservableObject
    {
        private readonly IServiceProvider serviceProvider;
        private string statusMessage;
        private bool isBusy;
        private AsyncRelayCommand loginCommand;

        public LogInViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                SetProperty(ref isBusy, value);
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        public AsyncRelayCommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand(OnLogin, () => !IsBusy));

        private async Task OnLogin()
        {
            IsBusy = true;
            StatusMessage = string.Empty;
            var loginResult = await IdentityService.LoginAsync();
            StatusMessage = GetStatusMessage(loginResult);
            IsBusy = false;
        }

        private string GetStatusMessage(LoginResultType loginResult)
        {
            switch (loginResult)
            {
                case LoginResultType.Unauthorized:
                    return "StatusUnauthorized".GetLocalized();
                case LoginResultType.NoNetworkAvailable:
                    return "StatusNoNetworkAvailable".GetLocalized();
                case LoginResultType.UnknownError:
                    return "StatusLoginFails".GetLocalized();
                default:
                    return string.Empty;
            }
        }
    }
}
