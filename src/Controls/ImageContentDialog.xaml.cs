using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
    private bool isLoaded;

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
            }
        }
    }

    public ImageContentDialog(Attachment MediaAttachment)
    {
        this.InitializeComponent();
        this.MediaAttachment = MediaAttachment;
        isLoaded = false;
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
        image.ImageOpened += Image_ImageOpened;
        await image.SetSourceAsync(await pixels.CreateStreamAsync());
        DisplayImage.Source = image;
    }

    private void Image_ImageOpened(object sender, RoutedEventArgs e)
    {
        if (!isLoaded)
        {
            var img = sender as BitmapImage;
            img.UriSource = new Uri(MediaAttachment.Url);
            isLoaded = true;
        }
    }
}
