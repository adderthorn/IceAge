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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public HttpClient HttpClient
    {
        get; set;
    }
    public MastodonClient MastodonClient
    {
        get; set;
    }
    public AuthenticationClient AuthenticationClient
    {
        get; set;
    }

    public Settings Settings => (App.Current as App).Settings;

    public MainWindow()
    {
        this.InitializeComponent();
        this.HttpClient = new HttpClient();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SystemBackdrop = new MicaBackdrop();
        ContentFrame.Navigate(typeof(LoginPage));
    }

    //private async void myButton_Click(object sender, RoutedEventArgs e)
    //{
    //    if (this.AuthenticationClient == null)
    //    {
    //        if (Settings.AppRegistration == null && !string.IsNullOrWhiteSpace(InstanceBox.Text))
    //        {
    //            this.AuthenticationClient = new AuthenticationClient(InstanceBox.Text, HttpClient);
    //        }
    //        else
    //        {
    //            this.AuthenticationClient = new AuthenticationClient(Settings.AppRegistration.Instance, HttpClient);
    //            Settings.AppRegistration = await AuthenticationClient.CreateApp("IceAge", Scope.Read | Scope.Write | Scope.Follow);
    //        }
    //    }
    //    if (string.IsNullOrWhiteSpace(Settings.AuthCode) && string.IsNullOrWhiteSpace(AuthCodeBox.Text))
    //    {
    //        myButton.Content = "Getting Auth Code...";
    //        var url = AuthenticationClient.OAuthUrl();
    //        await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
    //        myButton.Content = "Get Timeline";
    //        return;
    //    }
    //    else if (!string.IsNullOrWhiteSpace(AuthCodeBox.Text))
    //    {
    //        Settings.AuthCode = AuthCodeBox.Text;
    //    }
    //    myButton.Content = "Getting Toots...";
    //    if (Settings.Auth == null)
    //    {
    //        Settings.Auth = await AuthenticationClient.ConnectWithCode(Settings.AuthCode);
    //    }
    //    MastodonClient ??= new MastodonClient(Settings.AppRegistration.Instance, Settings.Auth.AccessToken, HttpClient);
    //    var timeline = await MastodonClient.GetHomeTimeline();
    //    TootsPanel.Children.Clear();
    //    for (int i = 0; i < 5; i++)
    //    {
    //        var tootControl = new TootControl(timeline[i], MastodonClient);
    //        TootsPanel.Children.Add(tootControl);
    //    }
    //    myButton.Content = "Refresh";
    //}

    private void NewTootButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        // C# code to create a new window
        var newWindow = new NewTootWindow();
        newWindow.Activate();

        // C# code to navigate in the new window
    }
}
