using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using IceAge.Models;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.UI.Notifications;

namespace IceAge.ViewModels;
public partial class NotificationsViewModel : ObservableObject
{
    private bool _isWritingCacheFile;
    private readonly DispatcherTimer _timer;

    protected readonly MastodonInterop _mastodonInterop;
    protected readonly JsonSerializer _serializer;

    public StorageFile CacheFile { get; set; }

    [ObservableProperty]
    public partial NotificationList NotificationList { get; set; }

    protected NotificationsViewModel(MastodonInterop mastodonInterop)
    {
        _isWritingCacheFile = false;
        _mastodonInterop = mastodonInterop;
        var s = new JsonSerializerSettings() { Formatting = Formatting.Indented };
        _serializer = JsonSerializer.Create(s);
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMinutes(5);
        _timer.Tick += (s, e) =>
        {
            // get notifications
        };
    }

    public void GetNotifications()
    {
        _mastodonInterop.MastodonClient.GetNotifications();
    }

    public async void ShowHelloWorldToast()
    {
        //// Create the toast content
        //var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
        //var textNodes = toastXml.GetElementsByTagName("text");
        //textNodes[0].AppendChild(toastXml.CreateTextNode("Hello World"));
        //textNodes[1].AppendChild(toastXml.CreateTextNode("This is a toast notification"));

        //// Create the toast notification
        //var toast = new ToastNotification(toastXml);

        //// Show the toast notification
        //ToastNotificationManager.CreateToastNotifier().Show(toast);
        var notifications = _mastodonInterop.GetNotifications();
        foreach (var notification in await notifications)
        {
            Debug.WriteLine(notification.Id);
        }
    }

    protected async Task SaveCacheFileAsync()
    {
        if (CacheFile == null || _isWritingCacheFile)
            return;
        _isWritingCacheFile = true;
        
        var stream = await CacheFile.OpenStreamForWriteAsync();
        using (var streamWriter = new StreamWriter(stream))
        using (var writer = new JsonTextWriter(streamWriter))
        {
            _serializer.Serialize(writer, NotificationList);
            await writer.FlushAsync();
        }
        _isWritingCacheFile = false;
    }

    public async void ClearNotifications()
    {
        NotificationList.MarkRead();
        await SaveCacheFileAsync();
    }
}
