using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceAge.Interop;
using Mastonet;

namespace IceAge.TimelineFetcher;
public class ExploreFetcher : TimelineFetcherBase
{
    public ExploreFetcher(MastodonInterop mastodonInterop) : base(mastodonInterop)
    {
        Timeline = new Mastonet.Entities.MastodonList<Mastonet.Entities.Status>();
    }

    // This feature doesn't support streaming.
    public override TimelineStreaming Streaming => null;

    public async override Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null)
    {
        IsLoadingTimeline = true;
        var statuses = await _mastodonInterop.MastodonClient.GetTrendingStatuses(null, options?.Limit);
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
        IsLoadingTimeline = false;
    }
}
