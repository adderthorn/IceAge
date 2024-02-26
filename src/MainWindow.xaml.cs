using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using IceAge.Interconnect;
using System.Net.Http;
using Mastonet;
using System.Diagnostics;
using Mastonet.Entities;
using System.Threading.Tasks;
using IceAge.Controls;
using System.Threading;
using IceAge.Pages;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public App ThisApp => App.Current as App;

    public MainWindow()
    {
        this.InitializeComponent();
        ThisApp.HttpClient = new HttpClient();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SystemBackdrop = new MicaBackdrop();
        if (ThisApp.Settings.Auth != null)
        {
            ThisApp.Auth = ThisApp.Settings.Auth;
            ThisApp.MastodonClient = new MastodonClient(ThisApp.Settings.AppRegistration.Instance, ThisApp.Settings.Auth.AccessToken);
            ContentFrame.Navigate(typeof(TimelinePage));
        }
        else
        {
            ContentFrame.Navigate(typeof(LoginPage));
        }

        this.AppWindow.Closing += async (s, e) =>
        {
            if (AppWindow.Size.Width > 0 && AppWindow.Size.Height > 0)
            {
                var rect = new RectInt32(AppWindow.Position.X, AppWindow.Position.Y, AppWindow.Size.Width, AppWindow.Size.Height);
                ThisApp.Settings.WindowSizeAndPosition = rect;
                await ThisApp.Settings.SaveAsync();
            }
        };

        if (ThisApp.Settings.WindowSizeAndPosition.Width > 0 && ThisApp.Settings.WindowSizeAndPosition.Height > 0)
        {
            AppWindow.MoveAndResize(ThisApp.Settings.WindowSizeAndPosition);
        }
    }

    private void NewTootButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        // C# code to create a new window
        var newWindow = new NewTootWindow();
        newWindow.Activate();

        // C# code to navigate in the new window
    }
}
