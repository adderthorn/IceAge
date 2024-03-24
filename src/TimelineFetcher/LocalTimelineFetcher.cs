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

    public LocalTimelineFetcher(Settings settings) : base(settings)
    {
        _streaming = App.Current.MastodonClient.GetPublicLocalStreaming();
    }

    public override TimelineStreaming Streaming => _streaming;

    public override async Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null)
    {
        var statuses = await App.Current.MastodonClient.GetPublicTimeline(options, local: true);
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
