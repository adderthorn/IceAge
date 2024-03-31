using IceAge.Interop;
using Mastonet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastonet.Entities;
using Windows.Storage.Provider;

namespace IceAge.TimelineFetcher;
public class HomeTimelineFetcher : TimelineFetcherBase
{
    private readonly TimelineStreaming _streaming;

    public HomeTimelineFetcher(MastodonInterop mastodonInterop) : base(mastodonInterop)
    {
        Timeline = new MastodonList<Status>();
        _streaming = _mastodonInterop.MastodonClient.GetUserStreaming();
    }

    public override TimelineStreaming Streaming => _streaming;

    public async override Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null)
    {
        IsLoadingTimeline = true;
        await populateFromCache();
        var statuses = await _mastodonInterop.MastodonClient.GetHomeTimeline(options);
        if (Timeline?.Count == 0)
        {
            Timeline = statuses;
        }
        else
        {
            switch (mode)
            {
                case TimelineMode.Add:
                    _ = await this.AddRangeAsync(statuses);
                    break;
                case TimelineMode.Insert:
                    _ = await this.InsertRangeAsync(0, statuses);
                    break;
            }
            for (int i = Timeline.Count - 1; i > 100; i--)
            {
                this.RemoveAt(i);
            }
        }
        await saveCacheFileAsync();
        IsLoadingTimeline = false;
    }
}
