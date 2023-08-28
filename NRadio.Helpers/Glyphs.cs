using Windows.ApplicationModel.Resources;

namespace NRadio.Helpers
{
    public class Glyphs
    {
        public string this[string key]
        {
            get => ResourceLoader.GetForCurrentView("Resources").GetString(key);
        }
    }
}
