using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace IceAge.Interop;
internal class BitmapInterop
{
    private const double DPI = 96;

    public uint Width { get; set; }
    public uint Height { get; set; }
    public string Hash { get; set; }

    public BitmapInterop(uint width, uint height)
    {
        Height = height;
        Width = width;
    }

    public async Task<BitmapImage> CreateImageFromBlurhashAsync(string hash)
    {
        Hash = hash;
        Blurhash.Pixel[,] pixles = new Blurhash.Pixel[Width, Height];
        Blurhash.Core.Decode(hash, pixles);
        var stream = new InMemoryRandomAccessStream();

        var pixelData = new byte[Width * Height * 4];
        int i = 0;

        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++) 
        {
                var pixel = pixles[x, y];
                pixelData[i++] = pixel.Blue.AssRgb();
                pixelData[i++] = pixel.Green.AssRgb();
                pixelData[i++] = pixel.Red.AssRgb();
                pixelData[i++] = 0;
        }

        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, Width, Height, DPI, DPI, pixelData);
        await encoder.FlushAsync();

        var img = new BitmapImage();
        stream.Seek(0);
        await img.SetSourceAsync(stream);
        return img;
    }
}
