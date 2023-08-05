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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private const string INSTANCE = "XXXXX";

    public HttpClient HttpClient { get; set; }
    public MastodonClient MastodonClient { get; set; }
    public AppRegistration AppRegistration { get; set; }
    public AuthenticationClient AuthenticationClient { get; set; }
    public Auth Auth { get; set; }

    public MainWindow()
    {
        this.InitializeComponent();
        this.HttpClient = new HttpClient();
        AuthenticationClient = new AuthenticationClient(INSTANCE, HttpClient);
    }

    private async void myButton_Click(object sender, RoutedEventArgs e)
    {
        AppRegistration ??= await AuthenticationClient.CreateApp("IceAge", Scope.Read);
        if (string.IsNullOrWhiteSpace(AuthCodeBox.Text))
        {
            myButton.Content = "Getting Auth Code...";
            var url = AuthenticationClient.OAuthUrl();
            await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            myButton.Content = "Get Timeline";
            return;
        }
        myButton.Content = "Getting Toots...";
        Auth ??= await AuthenticationClient.ConnectWithCode(AuthCodeBox.Text);
        MastodonClient ??= new MastodonClient(INSTANCE, Auth.AccessToken, HttpClient);
        var timeline = await MastodonClient.GetHomeTimeline();
        foreach (var item in timeline)
        {
            Debug.WriteLine(item);
        }
        myButton.Content = "Refresh";
    }
}
