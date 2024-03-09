using IceAge.Controls;
using Mastonet;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using Mastonet.Entities;
using Newtonsoft.Json;
using Windows.Storage;
using CommunityToolkit.WinUI.Helpers;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TimelinePage : Page
{
    private bool _loadingMorePages = false;

    public App ThisApp => App.Current as App;
    public TimelineStreaming Streaming { get; private set; }
    public MastodonList<Status> Timeline { get; private set; }

    public TimelinePage()
    {
        this.InitializeComponent();
        Streaming = ThisApp.MastodonClient.GetUserStreaming();
        Streaming.ReconnectStreamOnDisconnect = true;
        Streaming.OnUpdate += Streaming_OnUpdate;
    }

    private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e)
    {
        var toot = new TootControl(e.Status, ThisApp.MastodonClient, ThisApp.Settings.ShortenHyperlinks);
        TootsPanel.Children.Insert(0, toot);
        if (ScrollViewer.VerticalOffset > 0)
        {
            toot.UpdateLayout();
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + toot.ActualHeight);
        }
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        TootsPanel.Children.Clear();
        var localFolder = ApplicationData.Current.LocalFolder;
        var serializer = new JsonSerializer();
        StorageFile file;

        /**
         * This is a temporary measure to load a subset of the timeline and keep using
         * it for testing. I was having issues testing small changes when the content kept
         * changing on me in realtime. This way it caches a set of the timeline and will
         * use it on launch if the timeline file exists. This will be discarded eventually.
         **/
        if (await localFolder.FileExistsAsync("timeline.json"))
        {
            file = await localFolder.GetFileAsync("timeline.json");
            using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
            using (JsonReader jReader = new JsonTextReader(reader))
            {
                Timeline = serializer.Deserialize<MastodonList<Status>>(jReader);
            }
        }
        else
        {
            // TODO: Handle offline
            Timeline = await ThisApp.MastodonClient.GetHomeTimeline();
            file = await localFolder.CreateFileAsync("timeline.json");
            using (var writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            using (JsonWriter jWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jWriter, Timeline);
            }
        }

        foreach (var item in Timeline)
        {
            TootControl toot = new(item, ThisApp.MastodonClient, ThisApp.Settings.ShortenHyperlinks);
            TootsPanel.Children.Add(toot);
        }
        // TODO: need to fix this, right now streaming will close after some time without warning
        // and throw an exception
        await Streaming.Start();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        Streaming.Stop();
    }

    private async Task fetchMoreTimelinePagesAsync()
    {
        try
        {
            _loadingMorePages = true;
            var opts = new ArrayOptions() { MaxId = Timeline.Last().Id };
            var newStatuses = await ThisApp.MastodonClient.GetHomeTimeline(opts);
            foreach (var item in newStatuses)
            {
                Timeline.Add(item);
                var toot = new TootControl(item, ThisApp.MastodonClient, ThisApp.Settings.ShortenHyperlinks);
                TootsPanel.Children.Add(toot);
            }
        }
        catch (HttpRequestException)
        {
            // TODO: Handle offline
            ThisApp.HttpClient.CancelPendingRequests();
        }
        finally
        {
            _loadingMorePages = false;
        }
    }

    private async void ScrollViewer_ViewChangedAsync(object sender, ScrollViewerViewChangedEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;
        // Picking 1600 as an arbitrary offset to start loading more content before the absolute bottom
        // of the ScrollViewer
        if (!_loadingMorePages && (scrollViewer.VerticalOffset + 1600) >= scrollViewer.ScrollableHeight)
        {
            await fetchMoreTimelinePagesAsync();
        }
    }
}
