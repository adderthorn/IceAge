using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IceAge.Controls;
using Mastonet;
using Mastonet.Entities;
using System.Text.Json;
using Windows.Storage;
using System.Text.Json.Serialization.Metadata;
using System.Net.Http;

namespace IceAge.Interop;

internal class TootFactory : ITootFactory
{
    private readonly Settings _settings;
    private readonly StorageFile _cacheFile;
    private MastodonList<Status> _timeline;
    private List<TootControl> _controls;
    private bool _isWritingCacheFile;

    public MastodonList<Status> Timeline
    {
        get => _timeline;
        set
        {
            _timeline = value;
            init();
        }
    }

    public HttpClient HttpClient { get; }

    public MastodonClient MastodonClient { get; }

    public AuthenticationClient AuthenticationClient => throw new NotImplementedException();

    public Auth Auth => throw new NotImplementedException();

    public TootFactory()
    {
        MastodonClient = _settings.
        _timeline = timeline;
        _settings = settings;
        _cacheFile = cacheFile;
        _isWritingCacheFile = false;
        init();
    }

    public async Task<TootControl> AddAsync(Status status)
    {
        _timeline.Add(status);
        var toot = createControl(status);
        _controls.Add(toot);
        await saveCacheFileAsync();
        return toot;
    }

    public async Task RemoveAsync(Status status)
    {
        _timeline.Remove(status);
        await saveCacheFileAsync();
    }

    public async Task<TootControl> Insert(int index, Status status)
    {
        _timeline.Insert(index, status);
        var toot = createControl(status);
        _controls.Insert(index, toot);
        await saveCacheFileAsync();
        return toot;
    }

    public List<TootControl> GetTootControls() => _controls;

    private void init()
    {
        _controls = new List<TootControl>();
        foreach (var status in _timeline)
        {
            var toot = createControl(status);
            _controls.Add(toot);
        }
    }

    private async Task saveCacheFileAsync()
    {
        if (_cacheFile == null || _isWritingCacheFile)
            return;
        _isWritingCacheFile = true;
        using (var writer = new StreamWriter(await _cacheFile.OpenStreamForWriteAsync()))
        {
            var typeInfo = JsonTypeInfo.CreateJsonTypeInfo(typeof(MastodonList<Status>), JsonSerializerOptions.Default);
            var json = JsonSerializer.Serialize(_timeline, typeInfo);
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }
        _isWritingCacheFile = false;
    }

    private TootControl createControl(Status status) => new(status, _client, _settings.ShortenHyperlinks);
}
