using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Controls;
using IceAge.Interop;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace IceAge.ViewModels;
public class NotificationPageViewModel : ObservableObject
{
    private readonly MastodonInterop _interop;

    public NotificationPageViewModel()
    {
        _interop = App.Current.Services.GetService<MastodonInterop>();
    }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<NotificationControl> NotificationControls { get; set; }

    private async Task FetchNotificationsAsync()
    {
        IsLoading = true;
        var notifications = await _interop.MastodonClient.GetNotifications();
        NotificationControls.Clear();
        foreach (var item in notifications)
        {
            var vm = new NotificationViewModel(_interop, item);
            var n = new NotificationControl(vm);
            NotificationControls.Add(n);
        }
        IsLoading = false;
    }
}
