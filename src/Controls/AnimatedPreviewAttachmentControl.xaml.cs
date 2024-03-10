using System;
using System.ComponentModel;
using Blurhash;
using Mastonet.Entities;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class AnimatedPreviewAttachmentControl : UserControl, INotifyPropertyChanged
{
    private readonly Attachment _mediaAttachment;
    private bool _isSensitive;
    private bool _autoplay;
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

    public bool Autoplay
    {
        get =>_autoplay;
        set
        {
            if (value != _autoplay)
            {
                _autoplay = value;
                NotifyPropertyChanged(nameof(Autoplay));
                NotifyPropertyChanged(nameof(NotAutoplay));
            }
        }
    }

    public bool NotAutoplay => !_autoplay;

    public uint ImageWidth
    {
        get => _width;
        set
        {
            if (value != _width)
            {
                _width = value;
                NotifyPropertyChanged(nameof(Width));
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
                NotifyPropertyChanged(nameof(Height));
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

    public AnimatedPreviewAttachmentControl(Attachment attachment, bool isSensitive, uint width, uint height, bool autoplay)
    {
        this.InitializeComponent();
        _mediaAttachment = attachment;
        _isSensitive = isSensitive;
        _autoplay = autoplay;
        _width = width;
        _height = height;
        _altText = attachment.Description;
        initImage();
    }

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void initImage()
    {
        if (!string.IsNullOrEmpty(MediaAttachment.BlurHash))
        {
            var img = new BitmapImage();
            Pixel[,] pixels = new Pixel[_width, _height];
            Blurhash.Core.Decode(MediaAttachment.BlurHash, pixels);

            await img.SetSourceAsync(await pixels.CreateStreamAsync());
            BlurImage.Source = img;
        }

        if (!IsSensitive)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri(MediaAttachment.Url));
            Player.MediaPlayer.IsLoopingEnabled = Autoplay;
        }
    }
}
