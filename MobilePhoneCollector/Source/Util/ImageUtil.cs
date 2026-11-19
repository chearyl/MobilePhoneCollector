using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MobilePhoneCollector.Source.Util
{
    public class ImageUtil
    {
        public static async Task<ImageSource> ByteArrayToImageSource(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0) return new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));

            try
            {
                using InMemoryRandomAccessStream ms = new();
                await ms.WriteAsync(bytes.AsBuffer());
                ms.Seek(0);
                BitmapImage bi = new();
                await bi.SetSourceAsync(ms);
                return bi;
            }
            catch (Exception ex)
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
            }
            
        }

        public static (int, int, string, long) GetImageInfo(byte[] bytes)
        {
            long sizeInBytes = bytes.Length;

            using var ms = new MemoryStream(bytes);
            // Passing false for validateImageData speeds up reading headers
            using var img = Image.FromStream(ms, useEmbeddedColorManagement: false, validateImageData: false);

            // Dimensions in pixels
            int width = img.Width;
            int height = img.Height;

            // DPI Resolution
            float horizontalDpi = img.HorizontalResolution;
            float verticalDpi = img.VerticalResolution;

            // Image Type / Format
            string formatName = img.RawFormat.ToString();

            return (width, height, formatName, sizeInBytes);
        }
    }
}
