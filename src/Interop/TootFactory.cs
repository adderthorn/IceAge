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
using Newtonsoft.Json;
using Windows.Storage;

namespace IceAge.Interop;

class TootFactory
{
    private MastodonList<Status> _timeline;
    private MastodonClient _client;
    private List<TootControl> _controls;
    private Settings _settings;
    private StorageFile _cacheFile;
    private bool _isWritingCacheFile;
    private readonly JsonSerializer _serializer;

    public MastodonList<Status> Timeline
    {
        get => _timeline;
        set
        {
            _timeline = value;
            init();
        }
    }

    public TootFactory(MastodonClient mastodonClient, Settings settings, MastodonList<Status> timeline, StorageFile cacheFile = null)
    {
        _client = mastodonClient;
        _timeline = timeline;
        _settings = settings;
        _cacheFile = cacheFile;
        _serializer = new JsonSerializer();
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
        if (_cacheFile == null)
            return;
        _isWritingCacheFile = true;
        using (var writer = new StreamWriter(await _cacheFile.OpenStreamForWriteAsync()))
        using (var jsonWriter = new JsonTextWriter(writer))
        {
            _serializer.Serialize(jsonWriter, _timeline);
        }
        _isWritingCacheFile = false;
    }

    private TootControl createControl(Status status) => new(status, _client, _settings.ShortenHyperlinks);
}
