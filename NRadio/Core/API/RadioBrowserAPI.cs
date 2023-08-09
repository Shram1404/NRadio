using System.Net.Http;
using System.Threading.Tasks;

namespace NRadio.Core.API
{
    public static class RadioBrowserAPI
    {

        private const string BaseUrl = "https://de1.api.radio-browser.info/json";

        public static async Task<string> GetStationsByCountryAsync(string country)
        {
            HttpClient client = new HttpClient();
            using (client)
            {
                var response = await client.GetAsync($"{BaseUrl}/stations/bycountry/{country}");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                else
                    throw new HttpRequestException("Failed to get stations");
            }
        }
    }
}
