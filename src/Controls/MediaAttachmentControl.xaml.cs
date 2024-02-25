using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Mastonet.Entities;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.WebUI;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using ABI.Windows.Devices.Geolocation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class MediaAttachmentControl : UserControl, INotifyPropertyChanged
{
    private const double DPI = 96;
    private const uint DEFAULT_WIDTH_HEIGHT = 200;

    private readonly Attachment _mediaAttachment;
    private bool _isSensitive;
    private uint _width;
    private uint _height;
    private string _altText;

    public event PropertyChangedEventHandler PropertyChanged;

    public Attachment MediaAttachment => _mediaAttachment;

    public bool IsSensitive
    {
        get => _isSensitive;
        set
        {
            if (_isSensitive != value)
            {
                _isSensitive = value;
                NotifyPropertyChanged(nameof(IsSensitive));
            }
        }
    }

    public uint ImageWidth
    {
        get => _width;
        set
        {
            if (value != _width)
            {
                _width = value;
                NotifyPropertyChanged(nameof(ImageWidth));
            }
        }
    }

    public uint ImageHeight
    {
        get => _height;
        set
        {
            if (value != _height)
            {
                _height = value;
                NotifyPropertyChanged(nameof(ImageHeight));
            }
        }
    }

    public string AltText
    {
        get => _altText;
        set
        {
            if (value != _altText)
            {
                _altText = value;
                NotifyPropertyChanged(nameof(AltText));
                NotifyPropertyChanged(nameof(HasAltText));
            }
        }
    }

    public bool HasAltText => AltText?.Length > 0;

    public MediaAttachmentControl(Attachment attachment, bool isSensitive, uint width, uint height)
    {
        InitializeComponent();
        _mediaAttachment = attachment;
        _isSensitive = isSensitive;
        _width = width;
        _height = height;
        _altText = attachment.Description;
        initImage();
    }

    private async void initImage()
    {
        var img = new BitmapImage();
        
        await img.SetSourceAsync(await createBitmapStreamFromBlurAsync());
        DisplayImage.Source = img;

        if (!IsSensitive)
        {
            var previewImg = new BitmapImage();
            //previewImg.DecodePixelWidth = (int)_width;
            previewImg.ImageFailed += async (s, e) =>
            {
                var failedImg = s as BitmapImage;
                await failedImg.SetSourceAsync(await createBitmapStreamFromBlurAsync());
            };
            previewImg.UriSource = new Uri(MediaAttachment.PreviewUrl);
            DisplayImage.Source = previewImg;
        }
    }

    private async Task<IRandomAccessStream> createBitmapStreamFromBlurAsync()
    {
        Blurhash.Pixel[,] pixels = new Blurhash.Pixel[_width, _height];
        Blurhash.Core.Decode(MediaAttachment.BlurHash, pixels);
        var stream = new InMemoryRandomAccessStream();

        var pixelData = new byte[_width * _height * 4];
        int i = 0;

        for (var y = 0; y < _height; y++)
        for (var x = 0; x < _width; x++)
        {
                var pixel = pixels[x, y];
                pixelData[i++] = pixel.Blue.AssRgb();
                pixelData[i++] = pixel.Green.AssRgb();
                pixelData[i++] = pixel.Red.AssRgb();
                pixelData[i++] = 0;
        }

        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, _width, _height, DPI, DPI, pixelData);
        await encoder.FlushAsync();

        stream.Seek(0);
        return stream;
    }

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
