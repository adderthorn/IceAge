using IceAge.TimelineFetcher;
using Mastonet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        await Fetcher.InsertAsync(0, e.Status);
    }

    private async void init()
    {
        var fileTask = ApplicationData.Current.LocalCacheFolder.CreateFileAsync(kFileName, CreationCollisionOption.OpenIfExists);
        Fetcher.CacheFile = await fileTask;
    }
}
