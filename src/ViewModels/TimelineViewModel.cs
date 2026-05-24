using IceAge.TimelineFetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;
using Windows.Storage;
using System.Diagnostics;

namespace IceAge.ViewModels;
public class TimelineViewModel
{
    private const string kFileName = "timeline.json";

    public HomeTimelineFetcher Fetcher { get; }

    public TimelineViewModel(HomeTimelineFetcher fetcher)
    { 
        Fetcher = fetcher;
        fetcher.Streaming.OnUpdate += Streaming_OnUpdateAsync;
        init();
    }

    private async void Streaming_OnUpdateAsync(object sender, StreamUpdateEventArgs e)
    {
        try
        {
            await Fetcher.InsertAsync(0, e.Status);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating timeline: {ex.Message}");
        }
    }

    private async void init()
    {
        var fileTask = ApplicationData.Current.LocalFolder.CreateFileAsync(kFileName, CreationCollisionOption.OpenIfExists);
        Fetcher.CacheFile = await fileTask;
    }
}
