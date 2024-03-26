using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Pages;
using Mastonet;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceAge.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ResourceLoader _resourceLoader;

    private AuthenticationClient _authClient;

    public Settings Settings { get; }

    public string LabelText => WaitingOnAuthCode
        ? _resourceLoader.GetString("LoginPage/AuthCode")
        : _resourceLoader.GetString("LoginPage/Handle");

    [ObservableProperty]
    public bool _waitingOnAuthCode;

    [ObservableProperty]
    public bool _isLoggingIn;

    [ObservableProperty]
    public string _invalidHandleTitle;

    [ObservableProperty]
    public string _invalidHandleSubtitle;

    [ObservableProperty]
    public bool _invalidHandleIsOpen;

    public LoginViewModel(Settings settings)
    {
        this.Settings = settings;
        this._resourceLoader = new ResourceLoader();
        IsLoggingIn = false;
    }

    partial void OnWaitingOnAuthCodeChanged(bool value)
    {
        OnPropertyChanged(nameof(LabelText));
    }

    public async Task AuthenticateAsync(string authCode)
    {
        if (string.IsNullOrWhiteSpace(authCode))
            return;
        Settings.AuthCode = authCode;
        Settings.Auth = await _authClient.ConnectWithCode(Settings.AuthCode);
        App.Current.MastodonClient = new MastodonClient(_authClient.Instance, Settings.Auth.AccessToken);
        (MainWindow.Current as MainWindow).Navigate(typeof(TimelinePage));
    }

    public async Task AttemptLoginAsync(string handle)
    {
        IsLoggingIn = true;
        var match = Regex.Match(handle, IceAgeHelper.UsernameRegex, IceAgeHelper.UsernameRegexOptions);
        if (!match.Success)
        {
            InvalidHandleTitle = _resourceLoader.GetString("LoginStatus/InvalidHandle/Title");
            InvalidHandleSubtitle = _resourceLoader.GetString("LoginStatus/InvalidHandle/Subtitle");
            InvalidHandleIsOpen = true;
            IsLoggingIn = false;
            return;
        }
        try
        {
            string instance = match.Groups.Values.LastOrDefault().Value;
            _authClient = new AuthenticationClient(instance, App.Current.HttpClient);
            Settings.AppRegistration = await _authClient.CreateApp(
                appName: Settings.AppName,
                website: null,
                redirectUri: null,
                scope: [GranularScope.Read,
                       GranularScope.Follow,
                       GranularScope.Write,
                       GranularScope.Push]);
            var url = _authClient.OAuthUrl();
            if (await Windows.System.Launcher.LaunchUriAsync(new Uri(url)))
            {
                InvalidHandleIsOpen = false;
                WaitingOnAuthCode = true;
                return;
            }
            InvalidHandleTitle = _resourceLoader.GetString("LoginStatus/Failure/Title");
            InvalidHandleSubtitle = _resourceLoader.GetString("LoginStatus/Failure/Subtitle");
            InvalidHandleIsOpen = true;
            return;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            InvalidHandleTitle = _resourceLoader.GetString("LoginStatus/Error/Title");
            InvalidHandleSubtitle = _resourceLoader.GetString("LoginStatus/Error/Subtitle");
            InvalidHandleIsOpen = true;
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}
