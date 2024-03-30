using IceAge.Interop;
using Mastonet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceAge.TimelineFetcher;
internal class LocalTimelineFetcher : TimelineFetcherBase
{
    private readonly TimelineStreaming _streaming;

    public LocalTimelineFetcher(MastodonInterop mastodonInterop) : base(mastodonInterop)
    {
        _streaming = _mastodonInterop.MastodonClient.GetPublicLocalStreaming();
    }

    public override TimelineStreaming Streaming => _streaming;

    public async override Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null)
    {
        var statuses = await _mastodonInterop.MastodonClient.GetPublicTimeline(options, local: true);
        if (Timeline?.Count == 0)
        {
            Timeline = statuses;
            return;
        }
        switch (mode)
        {
            case TimelineMode.Add:
                _ = await this.AddRangeAsync(statuses);
                break;
            case TimelineMode.Insert:
                _ = await this.InsertRangeAsync(0, statuses);
                break;
        }
    }
}
