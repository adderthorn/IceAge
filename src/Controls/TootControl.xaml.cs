using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using IceAge.Interop;
using IceAge.ViewModels;
using Mastonet;
using Mastonet.Entities;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class TootControl : UserControl, INotifyPropertyChanged
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
        await dialog.ShowAsync();
    }

    private async void Animated_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var ctrl = sender as AnimatedPreviewAttachmentControl;
        var dialog = new AnimatedContentDialog(ctrl.MediaAttachment) { XamlRoot = this.XamlRoot };
        await dialog.ShowAsync();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
