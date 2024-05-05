using IceAge.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LocalPage : Page
{
    public LocalViewModel ViewModel { get; }

    public LocalPage()
    {
        this.ViewModel = App.Current.Services.GetService<LocalViewModel>();
        this.InitializeComponent();
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.Fetcher.FetchTimelineAsync(TimelineFetcher.TimelineMode.Add);
        await ViewModel.Fetcher.StartStreamingAsync();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.Fetcher.StopStreaming();
    }

    private async void ScrollViewer_ViewChangedAsync(object sender, ScrollViewerViewChangedEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;
        if ((scrollViewer.VerticalOffset + 1600) >= scrollViewer.ScrollableHeight)
        {
            await ViewModel.Fetcher.FetchNextTimelinePageAsync();
        }
    }
}
