using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blurhash;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace IceAge;

public static class IceAgeHelper
{
    private const int DPI = 96;

    public const string UsernameRegex = @"@?\b([A-Z0-9._%+-]+)@([A-Z0-9.-]+\.[A-Z]{2,})\b";
    public const RegexOptions UsernameRegexOptions = RegexOptions.IgnoreCase;

    public static byte AssRgb(this double v) => (byte)MathUtils.LinearTosRgb(v);

    public static async Task<IRandomAccessStream> CreateStreamAsync(this Pixel[,] pixels)
    {
        var stream = new InMemoryRandomAccessStream();
        uint width = (uint)pixels.GetLength(0);
        uint height = (uint)pixels.GetLength(1);

        var pixelData = new byte[width * height * 4];
        int i = 0;

        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var pixel = pixels[x, y];
                pixelData[i++] = pixel.Blue.AssRgb();
                pixelData[i++] = pixel.Green.AssRgb();
                pixelData[i++] = pixel.Red.AssRgb();
                pixelData[i++] = 0;
            }

        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, DPI, DPI, pixelData);
        await encoder.FlushAsync();

        stream.Seek(0);
        return stream;
    }
}
