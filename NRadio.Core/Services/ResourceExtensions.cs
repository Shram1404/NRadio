using Windows.ApplicationModel.Resources;

namespace NRadio.Services
{
    public static class ResourceExtensions
    {
        private static ResourceLoader resLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return resLoader.GetString(resourceKey);
        }
    }
}
