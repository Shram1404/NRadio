namespace NRadio.Core.Services
{
    public class ConfigService // TODO: Move to models or use a default config file
    {
        public string RadioStationsFileName { get; set; }
        public string RecentStationsFileName { get; set; }
        public string FavoriteStationsFileName { get; set; }
        public string PremiumStationsFileName { get; set; }
        public string ServerUrl { get; set; }
    }
}
