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
using Newtonsoft.Json;
using Windows.Storage;
using CommunityToolkit.WinUI.Helpers;
using Mastonet.Entities;
using Windows.Devices.Usb;

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
        var localFolder = ApplicationData.Current.LocalFolder;
        var serializer = new JsonSerializer();
        MastodonList<Status> timeline;
        StorageFile file;

        /**
         * This is a temporary measure to load a subset of the timeline and keep using
         * it for testing. I was having issues testing small changes when the content kept
         * changing on me in realtime. This way it caches a set of the timeline and will
         * use it on launch if the timeline file exists. This will be discarded eventually.
         **/
        if (await localFolder.FileExistsAsync("timeline.json"))
        {
            file = await localFolder.GetFileAsync("timeline.json");
            using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
            using (JsonReader jReader = new JsonTextReader(reader))
            {
                timeline = serializer.Deserialize<MastodonList<Status>>(jReader);
            }
        }
        else
        {
            timeline = await ThisApp.MastodonClient.GetHomeTimeline();
            file = await localFolder.CreateFileAsync("timeline.json");
            using (var writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            using (JsonWriter jWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jWriter, timeline);
            }
        }

        foreach (var item in timeline)
        {
            TootControl toot = new(item, ThisApp.MastodonClient);
            TootsPanel.Children.Add(toot);
        }
        // TODO: need to fix this, right now streaming will close after some time without warning
        // and throw an exception
        //await Streaming.Start();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        Streaming.Stop();
    }
}
