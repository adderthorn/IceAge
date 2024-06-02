using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using IceAge.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Mastonet;
using IceAge.Controls;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TimelinePage : Page
{
    public TimelineViewModel ViewModel { get; }
    public Settings Settings => App.Current.Settings;

    public TimelinePage()
    {
        this.ViewModel = App.Current.Services.GetService<TimelineViewModel>();
        this.InitializeComponent();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            await ViewModel.Fetcher.FetchTimelineAsync(TimelineFetcher.TimelineMode.Add);
            await ViewModel.Fetcher.StartStreamingAsync();
        }
        catch (ServerErrorException ex)
        {
            var dialog = new ErrorContentDialog(ex.Message, this.XamlRoot);
            var result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    // Retry
                    OnNavigatedTo(e);
                    break;
                case ContentDialogResult.Secondary:
                    // Reauth
                    Settings.ClearAuthorization();
                    Frame.Navigate(typeof(LoginPage));
                    break;
            }
        }
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
