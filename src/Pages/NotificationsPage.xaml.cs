using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using IceAge.Controls;
using IceAge.Interop;
using IceAge.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NotificationsPage : Page
{
    private readonly MastodonInterop _interop;

    public NotificationsPage()
    {
        this.InitializeComponent();
        _interop = App.Current.Services.GetService<MastodonInterop>();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var notifications = await _interop.MastodonClient.GetNotifications();
        MainPanel.Children.Clear();
        foreach (var item in notifications.Where(nf => nf.Status != null))
        {
            var vm = new NotificationViewModel(_interop, item);
            var n = new NotificationControl(vm);
            MainPanel.Children.Add(n);
        }
    }
}
