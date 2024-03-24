using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Controls;
using Mastonet;
using Mastonet.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using System.Net.Http;

namespace IceAge.TimelineFetcher;

public enum TimelineMode
{
    Add,
    Insert
}

public abstract partial class TimelineFetcherBase : ObservableObject
{
    private bool _isWritingCacheFile;
    private bool _isLoadingMorePages;

    protected readonly Settings _settings;

    protected TimelineFetcherBase(Settings settings)
    {
        _isWritingCacheFile = false;
        _isLoadingMorePages = false;
        _settings = settings;
    }

    public StorageFile CacheFile { get; set; }
    public abstract TimelineStreaming Streaming { get; }


    [ObservableProperty]
    private MastodonList<Status> _timeline;

    [ObservableProperty]
    private List<TootControl> _tootControls;

    public abstract Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null);

    public async Task<TootControl> AddAsync(Status status)
    {
        if (Timeline.Any(s => s.Id == status.Id))
            return null;

        Timeline.Add(status);
        var toot = createControl(status);
        TootControls.Add(toot);
        await saveCacheFileAsync();
        return toot;
    }

    public async Task<IEnumerable<TootControl>> AddRangeAsync(IEnumerable<Status> statuses)
    {
        var addedControls = new List<TootControl>();
        foreach (var status in statuses)
        {
            if (Timeline.Any(s => s.Id == status.Id))
                continue;
            Timeline.Add(status);
            var toot = createControl(status);
            TootControls.Add(toot);
            addedControls.Add(toot);
        }
        await saveCacheFileAsync();
        return addedControls;
    }

    public async Task RemoveAsync(Status status)
    {
        Timeline.Remove(status);
        await saveCacheFileAsync();
    }

    public async Task<TootControl> InsertAsync(int index, Status status)
    {
        if (Timeline.Any(s => s.Id == status.Id))
            return null;
        Timeline.Insert(index, status);
        var toot = createControl(status);
        TootControls.Insert(index, toot);
        await saveCacheFileAsync();
        return toot;
    }

    public async Task<IEnumerable<TootControl>> InsertRangeAsync(int index, IEnumerable<Status> statuses)
    {
        var insertedControls = new List<TootControl>();
        foreach (var status in statuses)
        {
            if (Timeline.Any(s => s.Id == status.Id))
                continue;
            Timeline.Insert(index, status);
            var toot = createControl(status);
            TootControls.Insert(index++, toot);
            insertedControls.Add(toot);
        }
        await saveCacheFileAsync();
        return insertedControls;
    }

    public async Task FetchNextTimelinePageAsync()
    {
        if (_isLoadingMorePages)
            return;

        try
        {
            _isLoadingMorePages = true;
            var opts = new ArrayOptions() { MaxId = Timeline.Last().Id };
            await FetchTimelineAsync(TimelineMode.Add, opts);
        }
        catch (HttpRequestException)
        {
            // TODO: Handle offline
            App.Current.HttpClient.CancelPendingRequests();
            throw;
        }
        finally
        {
            _isLoadingMorePages = false;
        }
    }

    public async Task StartStreamingAsync() => await Streaming.Start();
    public void StopStreaming() => Streaming.Stop();

    private async Task saveCacheFileAsync()
    {
        if (CacheFile == null || _isWritingCacheFile)
            return;
        _isWritingCacheFile = true;
        using (var writer = new StreamWriter(await CacheFile.OpenStreamForWriteAsync()))
        {
            var typeInfo = JsonTypeInfo.CreateJsonTypeInfo(typeof(MastodonList<Status>), JsonSerializerOptions.Default);
            var json = JsonSerializer.Serialize(Timeline, typeInfo);
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }
        _isWritingCacheFile = false;
    }

    partial void OnTimelineChanged(MastodonList<Status> value) =>
        TootControls = [.. Timeline.Select(createControl)];

    protected TootControl createControl(Status status) => new(status, App.Current.MastodonClient, _settings.ShortenHyperlinks);
}
