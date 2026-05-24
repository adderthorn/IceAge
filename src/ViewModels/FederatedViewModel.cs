using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceAge.TimelineFetcher;
using Mastonet;
using Windows.Storage;

namespace IceAge.ViewModels;
public class FederatedViewModel
{
    private const string kFileName = "federated.json";

    public FederatedTimelineFetcher Fetcher { get; }

    public FederatedViewModel(FederatedTimelineFetcher fetcher)
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
        var fileTask = ApplicationData.Current.LocalCacheFolder.CreateFileAsync(kFileName, CreationCollisionOption.OpenIfExists);
        Fetcher.CacheFile = await fileTask;
    }
}
