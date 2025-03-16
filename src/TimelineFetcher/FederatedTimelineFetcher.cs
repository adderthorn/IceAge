using IceAge.Interop;
using Mastonet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ConversationalAgent;

namespace IceAge.TimelineFetcher;
public class FederatedTimelineFetcher : TimelineFetcherBase
{
    private readonly TimelineStreaming _streaming;

    public FederatedTimelineFetcher(MastodonInterop mastodonInterop) : base(mastodonInterop)
    {
        Timeline = new Mastonet.Entities.MastodonList<Mastonet.Entities.Status>();
        _streaming = _mastodonInterop.MastodonClient.GetPublicStreaming();
    }

    public override TimelineStreaming Streaming => _streaming;

    public async override Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null)
    {
        IsLoadingTimeline = true;
        await PopulateFromCache();
        var statuses = await _mastodonInterop.MastodonClient.GetPublicTimeline(options, local: false);
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
        await SaveCacheFileAsync();
        IsLoadingTimeline = false;
    }
}
