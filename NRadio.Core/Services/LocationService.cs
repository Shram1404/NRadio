using Windows.Globalization;

namespace NRadio.Core.Services
{
    public static class LocationService
    {
        public static string GetCountryCode()
        {
            var region = new GeographicRegion();
            return region.CodeTwoLetter.ToUpper();
        }
    }
}
