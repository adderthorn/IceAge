using IceAge.Controls;
using Mastonet;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TimelinePage : Page
{
    public App ThisApp => App.Current as App;

    public TimelineStreaming Streaming { get; private set; }


    public TimelinePage()
    {
        this.InitializeComponent();
        Streaming = ThisApp.MastodonClient.GetUserStreaming();
        Streaming.OnUpdate += Streaming_OnUpdate;
    }

    private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e)
    {
        Debug.WriteLine("Updated: {0}", e.Status);
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        TootsPanel.Children.Clear();
        var timeline = await ThisApp.MastodonClient.GetHomeTimeline();
        foreach (var item in timeline)
        {
            TootControl toot = new(item, ThisApp.MastodonClient);
            TootsPanel.Children.Add(toot);
        }
        await Streaming.Start();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        Streaming.Stop();
    }
}
