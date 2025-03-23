using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using Mastonet.Entities;

namespace IceAge.ViewModels;
public partial class NotificationViewModel : ObservableObject
{
    private readonly MastodonInterop _interop;

    public TootViewModel TootViewModel { get; private set; }

    [ObservableProperty]
    public partial Notification Notification { get; set; }

    [ObservableProperty]
    public partial DateTime Created { get; set; }

    [ObservableProperty]
    public partial string Type { get; set; }

    public NotificationViewModel(MastodonInterop mastodonInterop, Notification notification)
    {
        _interop = mastodonInterop;
        Notification = notification;
    }

    partial void OnNotificationChanged(Notification value)
    {
        if (value.Status != null)
        {
            TootViewModel = new TootViewModel(_interop, value.Status);
        }
        Created = value.CreatedAt;
        Type = value.Type;
    }
}
