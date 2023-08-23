using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NRadio.Core.Helpers;
using NRadio.Models;

namespace NRadio.Core.Services
{
    public class MicrosoftGraphService
    {
        private const string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/";
        private const string apiServiceMe = "me/";
        private const string apiServiceMePhoto = "me/photo/$value";

        public MicrosoftGraphService()
        {
        }

        public async Task<User> GetUserInfoAsync(string accessToken)
        {
            User user = null;
            var httpContent = await GetDataAsync($"{graphAPIEndpoint}{apiServiceMe}", accessToken);
            if (httpContent != null)
            {
                var userData = await httpContent.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(userData))
                {
                    user = await Json.ToObjectAsync<User>(userData);
                }
            }

            return user;
        }

        public async Task<string> GetUserPhoto(string accessToken)
        {
            var httpContent = await GetDataAsync($"{graphAPIEndpoint}{apiServiceMePhoto}", accessToken);

            if (httpContent == null)
            {
                return string.Empty;
            }

            var stream = await httpContent.ReadAsStreamAsync();
            return stream.ToBase64String();
        }

        private async Task<HttpContent> GetDataAsync(string url, string accessToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content;
                    }
                    else
                    {
                        // TODO: Please handle other status codes as appropriate to your scenario
                    }
                }
            }
            catch (HttpRequestException)
            {
                // TODO: The request failed due to an underlying issue such as
                // network connectivity, DNS failure, server certificate validation or timeout.
                // Please handle this exception as appropriate to your scenario
            }
            catch (Exception)
            {
                // TODO: This call can fail please handle exceptions as appropriate to your scenario
            }

            return null;
        }
    }
}
