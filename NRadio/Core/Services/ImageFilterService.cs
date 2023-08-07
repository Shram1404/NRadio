using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace NRadio.Core.Services
{
    public static class ImageFilterService
    {
        public static async Task<BitmapImage> BlurImageAsync(string url) // Warning: this method don't work
        {
            using (Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient())
            {
                Uri uriResult;
                bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                var response = await httpClient.GetAsync(uriResult);

                // Load image from response stream
                var device = new CanvasDevice();
                var bitmap = await CanvasBitmap.LoadAsync(device, response.Content.ReadAsInputStreamAsync().GetResults().ToString());

                // Apply Gaussian blur effect
                var blur = new GaussianBlurEffect()
                {
                    BlurAmount = 5.0f,
                    Source = bitmap
                };

                // Create drawing session and draw blurred image
                var renderer = new CanvasRenderTarget(device, bitmap.SizeInPixels.Width, bitmap.SizeInPixels.Height, bitmap.Dpi);
                using (var ds = renderer.CreateDrawingSession())
                {
                    ds.DrawImage(blur);
                }

                // Save result to stream and set as source of BitmapImage
                var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                await renderer.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                BitmapImage image = new BitmapImage();
                image.SetSource(stream);

                return image;
            }
        }
    }
}
