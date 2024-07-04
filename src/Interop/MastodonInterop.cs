using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;

namespace IceAge.Interop;
public class MastodonInterop
{
    private string _lastReadNotification;

    public HttpClient HttpClient { get; }
    public MastodonClient MastodonClient { get; set; }

    public MastodonInterop()
    {
        HttpClient = new HttpClient();
    }

    public async Task<MastodonList<Notification>> GetNotifications()
    {
        if (MastodonClient == null)
            return null;

        var opts = new ArrayOptions();
        if (_lastReadNotification != null)
        {
            opts.SinceId = _lastReadNotification;
        }
        var notifications = await MastodonClient.GetNotifications(opts);
        var firstNotification = notifications.FirstOrDefault();
        if (firstNotification != null)
        {
            _lastReadNotification = firstNotification.Id;
        }
        return notifications;
    }
}
