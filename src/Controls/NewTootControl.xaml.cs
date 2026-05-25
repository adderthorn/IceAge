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
using Windows.UI.Popups;
using IceAge.ViewModels;
using Microsoft.UI.Windowing;
using System.Diagnostics;

namespace IceAge.Controls;

public sealed partial class NewTootControl : UserControl
{
    public NewTootViewModel ViewModel { get; }

    public NewTootControl(NewTootViewModel ViewModel)
    {
        this.ViewModel = ViewModel;
        this.InitializeComponent();
    }

    public event EventHandler<StatusPostedEventArgs> StatusPosted;

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        ContentTextBox.Focus(FocusState.Keyboard);
    }

    private async void PostButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ViewModel.Content.Length > 500)
        {
            // TODO: Show warning
            return;
        }
        var status = await ViewModel.PostStatusAsync(ViewModel.Content);
        StatusPosted?.Invoke(this, new StatusPostedEventArgs(status));
    }
    
    private void PhotoButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void VideoButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void HashtagButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void MentionButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var item = sender as MenuFlyoutItem;
        ViewModel.SetVisibilityFromTag(item.Tag as string);
    }
}
