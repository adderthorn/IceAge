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
using HtmlAgilityPack;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;

public enum LoginAttemptStatus
{
    InvalidHandle,
    UnhandledError,
    Failure,
    Success
}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page, INotifyPropertyChanged
{
    private const string kMastodonUrl = "https://www.joinmastodon.org/";
    private const string kAppName = "IceAge";
    private bool _waitingOnAuthCode;
    private bool _isLoggingIn;
    private AuthenticationClient _authClient;

    public event PropertyChangedEventHandler PropertyChanged;

    public App ThisApp => App.Current as App;

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

    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set
        {
            if (_isLoggingIn != value)
            {
                _isLoggingIn = value;
                notifyPropertyChanged(nameof(IsLoggingIn));
                notifyPropertyChanged(nameof(NotLoggingIn));
            }
        }
    }

    public bool NotLoggingIn => !_isLoggingIn;

    public LoginPage()
    {
        this.InitializeComponent();
        ThisApp.HttpClient = new HttpClient();
        _waitingOnAuthCode = false;
        _isLoggingIn = false;
    }

    private async Task<LoginAttemptStatus> AttemptLogin()
    {
        string handle = HandleTextBox.Text.Trim();
        var match = Regex.Match(handle, IceAgeHelper.UsernameRegex, IceAgeHelper.UsernameRegexOptions);
        if (!match.Success)
        {
            return LoginAttemptStatus.InvalidHandle;
        }
        try
        {
            string instance = match.Groups.Values.LastOrDefault().Value;//string.Concat("https://", match.Groups.Values.Last());
            _authClient = new AuthenticationClient(instance, ThisApp.HttpClient);
            ThisApp.Settings.AppRegistration = await _authClient.CreateApp(kAppName, Scope.Read | Scope.Write | Scope.Follow);
            var url = _authClient.OAuthUrl();
            if (await Windows.System.Launcher.LaunchUriAsync(new Uri(url)))
            {
                return LoginAttemptStatus.Success;
            }
            return LoginAttemptStatus.Failure;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return LoginAttemptStatus.UnhandledError;
        }
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        IsLoggingIn = true;
        //WaitingOnAuthCode = await AttemptLogin();
        switch (await AttemptLogin())
        {
            case LoginAttemptStatus.InvalidHandle:
                InvalidHandleTip.IsOpen = true;
                break;
            case LoginAttemptStatus.UnhandledError:
                break;
            case LoginAttemptStatus.Failure:
                break;
            case LoginAttemptStatus.Success:
                WaitingOnAuthCode = true;
                break;
            default:
                break;
        }
        IsLoggingIn = false;
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
        ThisApp.Settings.AuthCode = AutoCodeTextBox.Text.Trim();
        ThisApp.Settings.Auth = await _authClient.ConnectWithCode(ThisApp.Settings.AuthCode);
        Frame.Navigate(typeof(TimelinePage));
    }

    private void HandleTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            LoginButton_Click(sender, e);
        }
    }
}
