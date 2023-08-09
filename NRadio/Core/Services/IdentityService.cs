using Microsoft.Identity.Client;
using NRadio.Core.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NRadio.Core.Services
{
    public class IdentityService
    {
        // For more information about using Identity, see
        // https://github.com/microsoft/TemplateStudio/blob/main/docs/UWP/services/identity.md
        //
        // Read more about Microsoft Identity Client here
        // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki
        // https://docs.microsoft.com/azure/active-directory/develop/v2-overview

        // TODO: Please create a ClientID following these steps and update the app.config IdentityClientId.
        // https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app
        private readonly string clientId = ConfigurationManager.AppSettings["IdentityClientId"];

        private readonly string redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";

        private readonly string[] graphScopes = new string[] { "user.read" };

        private bool integratedAuthAvailable;
        private IPublicClientApplication client;
        private AuthenticationResult authenticationResult;

        public event EventHandler LoggedIn;

        public event EventHandler LoggedOut;

        public void InitializeWithAadAndPersonalMsAccounts()
        {
            integratedAuthAvailable = false;
            client = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
        }

        public void InitializeWithPersonalMsAccount()
        {
            integratedAuthAvailable = false;
            client = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AadAuthorityAudience.PersonalMicrosoftAccount)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
        }

        public void InitializeWithAadMultipleOrgs(bool integratedAuth = false)
        {
            integratedAuthAvailable = integratedAuth;
            client = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
        }

        public void InitializeWithAadSingleOrg(string tenant, bool integratedAuth = false)
        {
            integratedAuthAvailable = integratedAuth;
            client = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AzureCloudInstance.AzurePublic, tenant)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
        }

        public bool IsLoggedIn() => authenticationResult != null;

        public async Task<LoginResultType> LoginAsync()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return LoginResultType.NoNetworkAvailable;
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
                    return LoginResultType.Unauthorized;
                }

                LoggedIn?.Invoke(this, EventArgs.Empty);
                return LoginResultType.Success;
            }
            catch (MsalClientException ex)
            {
                if (ex.ErrorCode == "authentication_canceled")
                {
                    return LoginResultType.CancelledByUser;
                }

                return LoginResultType.UnknownError;
            }
            catch (Exception)
            {
                return LoginResultType.UnknownError;
            }
        }

        public bool IsAuthorized()
        {
            // TODO: You can also add extra authorization checks here.
            // i.e.: Checks permisions of _authenticationResult.Account.Username in a database.
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
            catch (MsalException)
            {
                // TODO: LogoutAsync can fail please handle exceptions as appropriate to your scenario
                // For more info on MsalExceptions see
                // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/exceptions
            }
        }

        public async Task<string> GetAccessTokenForGraphAsync() => await GetAccessTokenAsync(graphScopes);

        public async Task<string> GetAccessTokenAsync(string[] scopes)
        {
            var acquireTokenSuccess = await AcquireTokenSilentAsync(scopes);
            if (acquireTokenSuccess)
            {
                return authenticationResult.AccessToken;
            }
            else
            {
                try
                {
                    // Interactive authentication is required
                    var accounts = await client.GetAccountsAsync();
                    authenticationResult = await client.AcquireTokenInteractive(scopes)
                                                         .WithAccount(accounts.FirstOrDefault())
                                                         .ExecuteAsync();
                    return authenticationResult.AccessToken;
                }
                catch (MsalException)
                {
                    // AcquireTokenSilent and AcquireTokenInteractive failed, the session will be closed.
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
                    catch (MsalUiRequiredException)
                    {
                        // Interactive authentication is required
                        return false;
                    }
                }
                else
                {
                    // Interactive authentication is required
                    return false;
                }
            }
            catch (MsalException)
            {
                // TODO: Silentauth failed, please handle this exception as appropriate to your scenario
                // For more info on MsalExceptions see
                // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/exceptions
                return false;
            }
        }
    }
}
