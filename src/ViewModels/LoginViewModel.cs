using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using IceAge.Pages;
using Mastonet;
using Microsoft.UI.Xaml.Controls;
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

    public MastodonInterop MastodonInterop { get; }

    public string LabelText => WaitingOnAuthCode
        ? _resourceLoader.GetString("LoginPage/AuthCode")
        : _resourceLoader.GetString("LoginPage/Handle");

    public bool NotWaitingOnAuthCode => !WaitingOnAuthCode;

    public bool NotIsLoggingIn => !IsLoggingIn;

    [ObservableProperty]
    private bool _waitingOnAuthCode;

    [ObservableProperty]
    private bool _isLoggingIn;

    [ObservableProperty]
    private string _invalidHandleTitle;

    [ObservableProperty]
    private string _invalidHandleSubtitle;

    [ObservableProperty]
    private bool _invalidHandleIsOpen;

    public LoginViewModel(MastodonInterop mastodonInterop)
    {
        this.MastodonInterop = mastodonInterop;
        this._resourceLoader = new ResourceLoader();
        IsLoggingIn = false;
        WaitingOnAuthCode = false;
    }

    partial void OnWaitingOnAuthCodeChanged(bool value)
    {
        OnPropertyChanged(nameof(LabelText));
        OnPropertyChanged(nameof(NotWaitingOnAuthCode));
    }

    partial void OnIsLoggingInChanged(bool value)
    {
        OnPropertyChanged(nameof(NotIsLoggingIn));
    }

    public async Task<bool> AuthenticateAsync(string authCode)
    {
        if (string.IsNullOrWhiteSpace(authCode))
            return false;
        try
        {
            App.Current.Settings.AuthCode = authCode;
            App.Current.Settings.Auth = await _authClient.ConnectWithCode(App.Current.Settings.AuthCode);
            MastodonInterop.MastodonClient = new MastodonClient(_authClient.Instance, App.Current.Settings.Auth.AccessToken);
            await App.Current.Settings.SaveAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
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
            _authClient = new AuthenticationClient(instance, MastodonInterop.HttpClient);
            App.Current.Settings.AppRegistration = await _authClient.CreateApp(
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
