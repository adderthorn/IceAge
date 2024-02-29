using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Blurhash;
using Mastonet.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class AnimatedContentDialog : ContentDialog, INotifyPropertyChanged
{
    private Attachment _mediaAttachment;
    private uint _mediaWidth;
    private uint _mediaHeight;

    public Attachment MediaAttachment
    {
        get => _mediaAttachment;
        set
        {
            if (value != _mediaAttachment)
            {
                _mediaAttachment = value;
                NotifyPropertyChanged(nameof(MediaAttachment));
                NotifyPropertyChanged(nameof(IsGifv));
                NotifyPropertyChanged(nameof(NotIsGifv));
            }
        }
    }

    public bool IsGifv => MediaAttachment.Type == "gifv";
    public bool NotIsGifv => !IsGifv;

    public uint MediaWidth
    {
        get => _mediaWidth;
        set
        {
            if (value != _mediaWidth)
            {
                _mediaWidth = value;
                NotifyPropertyChanged(nameof(MediaWidth));
            }
        }
    }

    public uint MediaHeight
    {
        get => _mediaHeight;
        set
        {
            if (value != _mediaHeight)
            {
                _mediaHeight = value;
                NotifyPropertyChanged(nameof(MediaHeight));
            }
        }
    }

    public AnimatedContentDialog(Attachment MediaAttachment)
    {
        this.InitializeComponent();
        this.MediaAttachment = MediaAttachment;
        MediaHeight = (uint)(MediaAttachment.Meta.Original.Height ?? 700);
        MediaWidth = (uint)(MediaAttachment.Meta.Original.Width ?? 700);
        initMedia();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        this.Hide();
    }

    private async void initMedia()
    {
        uint width = (uint)(MediaAttachment.Meta.Original.Width ?? 700);
        uint height = (uint)(MediaAttachment.Meta.Original.Height ?? 700);
        Pixel[,] pixels = new Pixel[width, height];
        Blurhash.Core.Decode(MediaAttachment.BlurHash, pixels);
        var blurImage = new BitmapImage();
        await blurImage.SetSourceAsync(await pixels.CreateStreamAsync());
        BlurImage.Source = blurImage;

        Player.Source = MediaSource.CreateFromUri(new Uri(MediaAttachment.Url));
        Player.MediaPlayer.IsLoopingEnabled = IsGifv;
    }
}
