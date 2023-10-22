using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
using Mastonet;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page, INotifyPropertyChanged
{
    private const string kMastodonUrl = "https://www.joinmastodon.org/";
    private const string kAppName = "IceAge";
    private bool _waitingOnAuthCode;
    private AuthenticationClient _authClient;

    public event PropertyChangedEventHandler PropertyChanged;

    public Settings Settings => (App.Current as App).Settings;
    public HttpClient HttpClient {  get; set; }
    public bool WaitingOnAuthCode
    {
        get => _waitingOnAuthCode;
        set
        {
            if (_waitingOnAuthCode != value)
            {
                _waitingOnAuthCode = value;
                notifyPropertyChanged(nameof(WaitingOnAuthCode));
                notifyPropertyChanged(nameof(NotWaitingOnAuthCode));
                notifyPropertyChanged(nameof(LabelText));
            }
        }
    }

    public string LabelText => NotWaitingOnAuthCode ? "Handle:" : "Auth Code:";

    public bool NotWaitingOnAuthCode => !_waitingOnAuthCode;

    public LoginPage()
    {
        this.InitializeComponent();
        this.HttpClient = new HttpClient();
        _waitingOnAuthCode = false;
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string handle = HandleTextBox.Text.Trim();
        var match = Regex.Match(handle, IceAgeHelper.UsernameRegex, IceAgeHelper.UsernameRegexOptions);
        if (!match.Success)
        {
            InvalidHandleTip.IsOpen = true;
            return;
        }
        var instance = string.Concat("http://", match.Groups.Values.Last());
        _authClient = new AuthenticationClient(instance, HttpClient);
        var url = _authClient.OAuthUrl();
        if (await Windows.System.Launcher.LaunchUriAsync(new Uri(url)))
        {
            WaitingOnAuthCode = true;
        }
    }

    private void notifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void SignupButton_Click(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri(kMastodonUrl));
    }

    private async void AuthCodeButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AutoCodeTextBox.Text))
            return;
        Settings.AppRegistration = await _authClient.CreateApp(kAppName, Scope.Read | Scope.Write | Scope.Follow);
        Settings.AuthCode = AutoCodeTextBox.Text.Trim();
        Settings.Auth = await _authClient.ConnectWithCode(Settings.AuthCode);
        Frame.Navigate(typeof(MainWindow));
    }
}
