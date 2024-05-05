using System;
using Windows.Storage;
using Mastonet.Entities;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization.Metadata;
using Newtonsoft.Json;

namespace IceAge;

public partial class Settings : ObservableObject
{
    #region Private Variables
    private const string kFileName = "settings.json";
    private bool _isSaving;
    private readonly JsonSerializer serializer;

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

    [ObservableProperty]
    private bool _authSuccessful;
    #endregion

    #region Constructors
    public Settings()
    {
        AppRegistration = kAppRegistration;
        AuthCode = kAuthCode;
        Auth = kAuth;
        ElementTheme = kElementTheme;
        WindowSizeAndPosition = kWindowSizeAndPosition;
        SaveWindowSizeAndPosition = kSaveWindowSizeAndPosition;
        ShortenHyperlinks = kShortenHyperlinks;
        AutoPlay = kAutoPlay;
        NewWindows = kNewWindows;
        serializer = JsonSerializer.Create();
    }

    public static async Task<Settings> CreateAsync()
    {
        StorageFile settingsFile;
        try
        {
            settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync(kFileName);
            var stream = await settingsFile.OpenStreamForReadAsync();
            Settings settings;
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader))
            {
                var serializer = JsonSerializer.Create();
                settings = serializer.Deserialize<Settings>(reader);
            }
            return settings;
        }
        catch (FileNotFoundException)
        {
            settingsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(kFileName);
            var settings = new Settings();
            await settings.SaveAsync(await settingsFile.OpenStreamForWriteAsync());
            return settings;
        }
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Saves the settings to the local data container.
    /// </summary>
    public async Task SaveAsync()
    {
        var settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync(kFileName);
        var stream = await settingsFile.OpenStreamForWriteAsync();
        await SaveAsync(stream);
    }

    public async Task SaveAsync(Stream stream)
    {
        if (_isSaving)
            return;

        _isSaving = true;
        using (var streamWriter = new StreamWriter(stream))
        using (var writer = new JsonTextWriter(streamWriter))
        {
            serializer.Serialize(writer, this);
            await writer.FlushAsync();
            await streamWriter.FlushAsync();
        }
        _isSaving = false;
    }
    #endregion

    #region Private Functions
    partial void OnSaveWindowSizeAndPositionChanged(bool value)
    {
        if (!value)
            WindowSizeAndPosition = new RectInt32();
    }
    #endregion
}
