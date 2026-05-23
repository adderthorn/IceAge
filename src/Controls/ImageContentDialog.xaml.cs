using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Mastonet.Entities;
using Blurhash;
using Microsoft.Windows.Storage.Pickers;
using Microsoft.UI.Windowing;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;

public sealed partial class ImageContentDialog : ContentDialog, INotifyPropertyChanged
{
    private Attachment _mediaAttachment;

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<AttachmentButtonTappedEventArgs> SaveButtonTapped;

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
        if (!string.IsNullOrEmpty(MediaAttachment.BlurHash))
        {
            uint width = (uint)(MediaAttachment.Meta.Original.Width ?? 700);
            uint height = (uint)(MediaAttachment.Meta.Original.Height ?? 700);
            Pixel[,] pixels = new Pixel[width, height];
            Blurhash.Core.Decode(MediaAttachment.BlurHash, pixels);
            var image = new BitmapImage();
            await image.SetSourceAsync(await pixels.CreateStreamAsync());
            BlurImage.Source = image;
        }
        
        var remoteImg = new BitmapImage() { UriSource = new Uri(MediaAttachment.Url) };
        RemoteImage.Source = remoteImg;
    }

    private void PopoutButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        SaveButtonTapped?.Invoke(this, new AttachmentButtonTappedEventArgs()
        {
            AttachmentUri = new Uri(MediaAttachment.RemoteUrl)
        });
    }

    private void RemoteImage_Tapped(object sender, TappedRoutedEventArgs e)
    {
        AltTextBlock.Visibility = AltTextBlock.Visibility == Microsoft.UI.Xaml.Visibility.Visible
            ? Microsoft.UI.Xaml.Visibility.Collapsed
            : Microsoft.UI.Xaml.Visibility.Visible;
    }
}

public class AttachmentButtonTappedEventArgs : EventArgs
{
    public Uri AttachmentUri { get;set; }
}