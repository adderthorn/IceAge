using System;
using Windows.Storage;
using Mastonet.Entities;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace IceAge;

public partial class Settings : ObservableObject
{
    #region Private Variables
    private readonly ApplicationDataContainer localSettings;

    // Defaults
    private const AppRegistration kAppRegistration = null;
    private const string kAuthCode = null;
    private const Auth kAuth = null;
    private const ElementTheme kElementTheme = ElementTheme.Default;
    private readonly RectInt32 kWindowSizeAndPosition = new();
    private const bool kSaveWindowSizeAndPosition = true;
    private const bool kShortenHyperlinks = true;
    private const bool kAutoPlay = true;
    private const bool kNewWindows = false;
    #endregion

    #region Public Properties
    /// <summary>
    /// Name of the Application for use in Mastodon API registration.
    /// </summary>
    public const string AppName = "IceAge";

    [ObservableProperty]
    private AppRegistration _appRegistration;

    [ObservableProperty]
    private string _authCode;

    [ObservableProperty]
    private Auth _auth;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private RectInt32 _windowSizeAndPosition;

    [ObservableProperty]
    private bool _saveWindowSizeAndPosition;

    [ObservableProperty]
    private bool _shortenHyperlinks;

    [ObservableProperty]
    private bool _autoPlay;

    [ObservableProperty]
    private bool _newWindows;
    #endregion

    #region Constructors
    /// <summary>
    /// Loads the settings object and populates all settings from the
    /// local data container.
    /// </summary>
    /// <exception cref="Exception">The local settings container was null.</exception>
    public Settings()
    {
        localSettings = ApplicationData.Current.LocalSettings;

        if (localSettings == null)
            throw new Exception("Local settings cannot be null");

        this.AppRegistration = getSetting(nameof(AppRegistration), kAppRegistration);
        this.AuthCode = getSetting(nameof(AuthCode), kAuthCode);
        this.Auth = getSetting(nameof(Auth), kAuth);
        this.ElementTheme = getSetting(nameof(ElementTheme), kElementTheme);
        this.WindowSizeAndPosition = getSetting(nameof(WindowSizeAndPosition), kWindowSizeAndPosition);
        this.SaveWindowSizeAndPosition = getSetting(nameof(SaveWindowSizeAndPosition), kSaveWindowSizeAndPosition);
        this.ShortenHyperlinks = getSetting(nameof(ShortenHyperlinks), kShortenHyperlinks);
        this.AutoPlay = getSetting(nameof(AutoPlay), kAutoPlay);
        this.NewWindows = getSetting(nameof(NewWindows), kNewWindows);
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Saves the settings to the local data container.
    /// </summary>
    public void Save()
    {
        localSettings.Values[nameof(AppRegistration)] = this.AppRegistration;
        localSettings.Values[nameof(AuthCode)] = this.AuthCode;
        localSettings.Values[nameof(Auth)] = this.Auth;
        localSettings.Values[nameof(ElementTheme)] = this.ElementTheme;
        localSettings.Values[nameof(WindowSizeAndPosition)] = this.WindowSizeAndPosition;
        localSettings.Values[nameof(SaveWindowSizeAndPosition)] = this.SaveWindowSizeAndPosition;
        localSettings.Values[nameof(ShortenHyperlinks)] = this.ShortenHyperlinks;
        localSettings.Values[nameof(AutoPlay)] = this.AutoPlay;
        localSettings.Values[nameof(NewWindows)] = this.NewWindows;
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Gets either the saved setting value from the
    /// local data container or a specified default value.
    /// </summary>
    /// <typeparam name="T">Type of the value object.</typeparam>
    /// <param name="key">Setting key name.</param>
    /// <param name="defaultValue">Default value of the setting.</param>
    /// <returns>Setting value.</returns>
    private T getSetting<T>(string key, T defaultValue)
    {
        try
        {
            Object obj = localSettings.Values[key];
            if (obj == null || obj.GetType() != typeof(T))
            {
                return defaultValue;
            }
            return (T)obj;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return defaultValue;
        }
    }

    partial void OnSaveWindowSizeAndPositionChanged(bool value)
    {
        if (!value)
            WindowSizeAndPosition = new RectInt32();
    }
    #endregion
}
