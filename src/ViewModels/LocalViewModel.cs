using IceAge.TimelineFetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        await Fetcher.InsertAsync(0, e.Status);
    }

    private async void init()
    {
        var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(kFileName, CreationCollisionOption.OpenIfExists);
        Fetcher.CacheFile = file;
    }
}
