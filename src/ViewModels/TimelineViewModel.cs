using IceAge.TimelineFetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;

namespace IceAge.ViewModels;
public class TimelineViewModel
{
    public Settings Settings { get; }
    
    public HomeTimelineFetcher Fetcher { get; }

    public TimelineViewModel(Settings settings, HomeTimelineFetcher fetcher)
    { 
        Settings = settings;
        Fetcher = fetcher;
        fetcher.Streaming.OnUpdate += Streaming_OnUpdateAsync;
    }

    private async void Streaming_OnUpdateAsync(object sender, StreamUpdateEventArgs e)
    {
        await Fetcher.InsertAsync(0, e.Status);
    }
}
