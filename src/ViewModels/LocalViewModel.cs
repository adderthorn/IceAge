using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceAge.TimelineFetcher;
using Windows.Storage;

namespace IceAge.ViewModels;
public class LocalViewModel
{
    private const string kFileName = "local.json";

    public LocalTimelineFetcher Fetcher { get; }

    public LocalViewModel(LocalTimelineFetcher fetcher)
    {
        Fetcher = fetcher;
        fetcher.Streaming.OnUpdate += Streaming_OnUpdateAsync;
        init();
    }

    private async void Streaming_OnUpdateAsync(object sender, Mastonet.StreamUpdateEventArgs e)
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
