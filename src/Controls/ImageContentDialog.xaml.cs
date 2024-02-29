using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Mastonet.Entities;
using Blurhash;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;

public sealed partial class ImageContentDialog : ContentDialog, INotifyPropertyChanged
{
    private Attachment _mediaAttachment;

    public event PropertyChangedEventHandler PropertyChanged;

    public Attachment MediaAttachment
    {
        get => _mediaAttachment;
        set
        {
            if (_mediaAttachment != value)
            {
                _mediaAttachment = value;
                NotifyPropertyChanged(nameof(MediaAttachment));
                NotifyPropertyChanged(nameof(BitmapImage));
                NotifyPropertyChanged(nameof(HasAltText));
            }
        }
    }

    public bool HasAltText => _mediaAttachment.Description?.Length > 0;

    public ImageContentDialog(Attachment MediaAttachment)
    {
        this.InitializeComponent();
        this.MediaAttachment = MediaAttachment;
        initImage();
    }

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        this.Hide();
    }

    private async void initImage()
    {
        uint width = (uint)(MediaAttachment.Meta.Original.Width ?? 700);
        uint height = (uint)(MediaAttachment.Meta.Original.Height ?? 700);
        Pixel[,] pixels = new Pixel[width, height];
        Blurhash.Core.Decode(MediaAttachment.BlurHash, pixels);
        var image = new BitmapImage();
        await image.SetSourceAsync(await pixels.CreateStreamAsync());
        BlurImage.Source = image;
        
        var remoteImg = new BitmapImage() { UriSource = new Uri(MediaAttachment.Url) };
        RemoteImage.Source = remoteImg;
    }
}
