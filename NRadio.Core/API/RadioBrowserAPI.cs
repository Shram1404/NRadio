using NRadio.Core.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace NRadio.Core.API
{
    public static class RadioBrowserAPI
    {
        public static async Task<string> GetStationsByCountryAsync(string country)
        {
            var cfg = await RadioStationsLoader.LoadConfigAsync();
            string baseUrl = cfg.ServerUrl;

            HttpClient client = new HttpClient();
            using (client)
            {
                var response = await client.GetAsync($"{baseUrl}/stations/bycountry/{country}");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                else
                    throw new HttpRequestException("Failed to get stations");
            }
        }
    }
}
