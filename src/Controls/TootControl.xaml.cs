using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IceAge.Interop;
using IceAge.ViewModels;
using Mastonet;
using Mastonet.Entities;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.Storage.Pickers;
using Windows.Graphics;
using Windows.System;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class TootControl : UserControl
{
    //private readonly MastodonClient _client;
    private readonly RichTextInterop _interop;

    public TootViewModel ViewModel { get; }

    public TootControl(TootViewModel viewModel, bool shortenHyperlinks)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        _interop = new RichTextInterop(ContentBlock, viewModel, shortenHyperlinks);
        AttachmentBlock.Items.Clear();
        foreach (var ctrl in ViewModel.AttachmentControls)
        {
            if (ctrl is ImageAttachmentControl imgCtrl)
            {
                imgCtrl.ContentTapped += Img_Tapped;
            }
            else if (ctrl is AnimatedPreviewAttachmentControl aniCtrl)
            {
                aniCtrl.ContentTapped += Animated_Tapped;
            }
            AttachmentBlock.Items.Add(ctrl);
        }
    }

    private async void Img_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var img = sender as ImageAttachmentControl;
        var dialog = new ImageContentDialog(img.MediaAttachment)
        {
            XamlRoot = this.XamlRoot
        };
        dialog.SaveButtonTapped += Dialog_SaveButtonTapped;
        dialog.PopupButtonTapped += Dialog_PopupButtonTapped;

        await dialog.ShowAsync();
    }

    private void Dialog_PopupButtonTapped(object sender, EventArgs e)
    {
        ImageContentDialog dialogCtrl = sender as ImageContentDialog;
        var window = new Window()
        {
            ExtendsContentIntoTitleBar = true,
            SystemBackdrop = new MicaBackdrop(),
            Content = new Page()
            {
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri(dialogCtrl.MediaAttachment.RemoteUrl)),
                    Stretch = Stretch.Uniform
                },
                RequestedTheme = this.ActualTheme
            }
        };
        window.AppWindow.ResizeClient(new SizeInt32(500, 500));
        window.Activate();
    }

    private async void Dialog_SaveButtonTapped(object sender, AttachmentButtonTappedEventArgs e)
    {
        var fileName = e.AttachmentUri.Segments.LastOrDefault();
        if (string.IsNullOrEmpty(fileName) || !fileName.Contains('.'))
            throw new ArgumentException($"URI {fileName} is incorrect.");

        var fileExtension = "." + fileName.Split('.').LastOrDefault();

        var picker = new FileSavePicker(App.Current.MainWindow.AppWindow.Id)
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeChoices = { { "Images", new[] { fileExtension } } }
        };
        var result = await picker.PickSaveFileAsync();
        if (result != null)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(e.AttachmentUri))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var writer = File.Create(result.Path, 8192, FileOptions.WriteThrough))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(writer);
            }
        }
    }

    private async void Animated_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var ctrl = sender as AnimatedPreviewAttachmentControl;
        var dialog = new AnimatedContentDialog(ctrl.MediaAttachment) { XamlRoot = this.XamlRoot };
        await dialog.ShowAsync();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void ActionButton_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void FavoriteButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        FavoriteRotateAnimation.StartAsync();
        ViewModel.ToggleFavorite();
    }

    private void BoostButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        BoostScaleAnimation.StartAsync();
        ViewModel.ToggleBoost();
    }
}
