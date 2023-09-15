using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using NRadio.Helpers;
using NRadio.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace NRadio.Core.Services
{
    public class IdentityService
    {
        private const string ClientId = "fe43a2a4-d553-41cf-9389-ab484c5f1a39";
        private const string RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";

        private readonly string[] graphScopes = new string[] { "user.read" };
        private bool integratedAuthAvailable;
        private IPublicClientApplication client;
        private AuthenticationResult authenticationResult;

        public event EventHandler LoggedIn;
        public event EventHandler LoggedOut;

        public void InitializeWithAadAndPersonalMsAccounts()
        {
            integratedAuthAvailable = false;
            client = PublicClientApplicationBuilder.Create(ClientId)
                                                    .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                                                    .WithRedirectUri(RedirectUri)
                                                    .WithBroker(true)
                                                    .WithLogging(new IdentityLogger(EventLogLevel.Warning), enablePiiLogging: false)
                                                    .Build();
        }

        public bool IsLoggedIn() => authenticationResult != null;

        public async Task<LoginEnum.LoginResultType> LoginAsync()
        {
            var login = new LoginEnum();
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return LoginEnum.LoginResultType.NoNetworkAvailable;
            }

            try
            {
                var accounts = await client.GetAccountsAsync();
                authenticationResult = await client.AcquireTokenInteractive(graphScopes)
                                                     .WithAccount(accounts.FirstOrDefault())
                                                     .ExecuteAsync();
                if (!IsAuthorized())
                {
                    authenticationResult = null;
                    return LoginEnum.LoginResultType.Unauthorized;
                }

                LoggedIn?.Invoke(this, EventArgs.Empty);
                return LoginEnum.LoginResultType.Success;
            }
            catch (MsalClientException ex)
            {
                if (ex.ErrorCode == "authentication_canceled")
                {
                    return LoginEnum.LoginResultType.CancelledByUser;
                }

                return LoginEnum.LoginResultType.UnknownError;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{ex}");
                return LoginEnum.LoginResultType.UnknownError;
            }
        }

        public bool IsAuthorized()
        {
            return true;
        }

        public string GetAccountUserName()
        {
            return authenticationResult?.Account?.Username;
        }

        public async Task LogoutAsync()
        {
            try
            {
                var accounts = await client.GetAccountsAsync();
                var account = accounts.FirstOrDefault();
                if (account != null)
                {
                    await client.RemoveAsync(account);
                }

                authenticationResult = null;
                LoggedOut?.Invoke(this, EventArgs.Empty);
            }
            catch (MsalException ex)
            {
                var messageDialog = new MessageDialog(ex.Message, "There was an error, send this text to our email");
                await messageDialog.ShowAsync();
            }
        }

        public async Task<string> GetAccessTokenForGraphAsync() => await GetAccessTokenAsync(graphScopes);

        public async Task<string> GetAccessTokenAsync(string[] scopes)
        {
            bool acquireTokenSuccess = await AcquireTokenSilentAsync(scopes);
            if (acquireTokenSuccess)
            {
                return authenticationResult.AccessToken;
            }
            else
            {
                try
                {
                    var accounts = await client.GetAccountsAsync();
                    authenticationResult = await client.AcquireTokenInteractive(scopes)
                                                         .WithAccount(accounts.FirstOrDefault())
                                                         .ExecuteAsync();
                    return authenticationResult.AccessToken;
                }
                catch (MsalException ex)
                {
                    Debug.WriteLine($"MsalException (Access token issue): {ex.Message}");
                    authenticationResult = null;
                    LoggedOut?.Invoke(this, EventArgs.Empty);
                    return string.Empty;
                }
            }
        }

        public async Task<bool> AcquireTokenSilentAsync() => await AcquireTokenSilentAsync(graphScopes);

        private async Task<bool> AcquireTokenSilentAsync(string[] scopes)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            try
            {
                var accounts = await client.GetAccountsAsync();
                authenticationResult = await client.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                                     .ExecuteAsync();
                return true;
            }
            catch (MsalUiRequiredException)
            {
                if (integratedAuthAvailable)
                {
                    try
                    {
                        authenticationResult = await client.AcquireTokenByIntegratedWindowsAuth(graphScopes)
                                                             .ExecuteAsync();
                        return true;
                    }
                    catch (MsalUiRequiredException ex)
                    {
                        Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                        authenticationResult = await client.AcquireTokenInteractive(scopes)
                                                          .ExecuteAsync()
                                                          .ConfigureAwait(false);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (MsalException ex)
            {
                Debug.WriteLine($"MsalException: {ex.Message}");
                return false;
            }
        }
    }
}
