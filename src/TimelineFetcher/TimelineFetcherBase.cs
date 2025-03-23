using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Controls;
using Mastonet;
using Mastonet.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using System.Net.Http;
using Newtonsoft.Json;
using IceAge.Interop;
using System.Collections.ObjectModel;
using System.Diagnostics;
using IceAge.ViewModels;

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

    protected readonly MastodonInterop _mastodonInterop;
    protected readonly JsonSerializer _serializer;

    protected TimelineFetcherBase(MastodonInterop mastodonInterop)
    {
        _isWritingCacheFile = false;
        _isLoadingMorePages = false;
        _mastodonInterop = mastodonInterop;
        var s = new JsonSerializerSettings() { Formatting = Formatting.Indented };
        _serializer = JsonSerializer.Create(s);
        Timeline = new MastodonList<Status>();
    }

    public StorageFile CacheFile { get; set; }
    public abstract TimelineStreaming Streaming { get; }


    [ObservableProperty]
    public partial MastodonList<Status> Timeline { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<TootControl> TootControls { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingTimeline { get; set; }

    public abstract Task FetchTimelineAsync(TimelineMode mode, ArrayOptions options = null);

    public async Task<TootControl> AddAsync(Status status)
    {
        if (Timeline.Any(s => s.Id == status.Id))
            return null;

        Timeline.Add(status);
        var toot = CreateControl(status);
        TootControls.Add(toot);
        await SaveCacheFileAsync();
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
            var toot = CreateControl(status);
            TootControls.Add(toot);
            addedControls.Add(toot);
        }
        await SaveCacheFileAsync();
        return addedControls;
    }

    public void Remove(Status status)
    {
        Timeline.Remove(status);
        TootControls.Remove(TootControls.FirstOrDefault(tc => tc.ViewModel.Status.Id == status.Id));
    }

    public void RemoveAt(int index)
    {
        Timeline.RemoveAt(index);
        TootControls.RemoveAt(index);
    }

    public async Task<TootControl> InsertAsync(int index, Status status)
    {
        if (Timeline.Any(s => s.Id == status.Id))
            return null;
        Timeline.Insert(index, status);
        var toot = CreateControl(status);
        TootControls.Insert(index, toot);
        await SaveCacheFileAsync();
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
            var toot = CreateControl(status);
            TootControls.Insert(index++, toot);
            insertedControls.Add(toot);
        }
        await SaveCacheFileAsync();
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
            _mastodonInterop.HttpClient.CancelPendingRequests();
            throw;
        }
        finally
        {
            _isLoadingMorePages = false;
        }
    }

    public async Task StartStreamingAsync() => await Streaming.Start();
    public void StopStreaming() => Streaming.Stop();

    protected async Task SaveCacheFileAsync()
    {
        if (CacheFile == null || _isWritingCacheFile)
            return;
        _isWritingCacheFile = true;
        var stream = await CacheFile.OpenStreamForWriteAsync();
        using (var streamWriter = new StreamWriter(stream))
        using (var writer = new JsonTextWriter(streamWriter))
        {
            _serializer.Serialize(writer, Timeline);
            await writer.FlushAsync();
        }
        _isWritingCacheFile = false;
    }

    partial void OnTimelineChanged(MastodonList<Status> value) =>
        TootControls = [.. Timeline.Select(CreateControl)];

    protected TootControl CreateControl(Status status)
    {
        var viewModel = new TootViewModel(_mastodonInterop, status);
        var control = new TootControl(viewModel, App.Current.Settings.ShortenHyperlinks);
        return control;
    }

    protected async Task PopulateFromCache()
    {
        if (CacheFile == null || _isWritingCacheFile)
            return;

        Debug.WriteLine("Populating from cache...");
        var stream = await CacheFile.OpenStreamForReadAsync();
        using (var streamReader = new StreamReader(stream))
        using (var reader = new JsonTextReader(streamReader))
        {
            Timeline = _serializer.Deserialize<MastodonList<Status>>(reader);
        }
    }
}
