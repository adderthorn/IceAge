using Mastonet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ConversationalAgent;

namespace IceAge.TimelineFetcher;
internal class FederatedTimelineFetcher : TimelineFetcherBase
{
    private readonly TimelineStreaming _streaming;

    public FederatedTimelineFetcher(Settings settings) : base(settings)
    {
        _streaming = App.Current.MastodonClient.GetPublicStreaming();
    }

    public override TimelineStreaming Streaming => _streaming;

    public override async Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null)
    {
        var statuses = await App.Current.MastodonClient.GetPublicTimeline(options, local: false);
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
