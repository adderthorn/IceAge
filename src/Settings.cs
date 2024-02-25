using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using Mastonet.Entities;
using System.Xml.Linq;
using System.IO;
using Mastonet;

namespace IceAge;

public class Settings : INotifyPropertyChanged
{
    #region Private Variables
    private const string FILE_NAME = "settings.json";
    private bool _canSave = false;

    private AppRegistration _appRegistration;
    private string _authCode;
    private Auth _auth;
    #endregion

    #region Public Properties
    public const string AppName = "IceAge";
    public event PropertyChangedEventHandler PropertyChanged;


    [JsonProperty]
    public AppRegistration AppRegistration
    {
        get => _appRegistration;
        set
        {
            if (_appRegistration == value) return;
            _appRegistration = value;
            RaisePropertyChanged(nameof(AppRegistration));
            saveAsync().ConfigureAwait(false);
        }
    }

    public string AuthCode
    {
        get => _authCode;
        set
        {
            if (_authCode == value) return;
            _authCode = value;
            RaisePropertyChanged(nameof(AuthCode));
            saveAsync().ConfigureAwait(false);
        }
    }

    public Auth Auth
    {
        get => _auth;
        set
        {
            if (_auth == value) return;
            _auth = value;
            RaisePropertyChanged(nameof(Auth));
            saveAsync().ConfigureAwait(false);
        }
    }
    #endregion

    #region Constructors & Static Methods
    public static async Task<Settings> LoadSettingsAsync()
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        StorageFile file;
        try
        {
            file = await localFolder.GetFileAsync(FILE_NAME);
            using (StreamReader streamReader = new StreamReader(await file.OpenStreamForReadAsync()))
            using (JsonReader reader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                var settings = serializer.Deserialize<Settings>(reader) ?? new Settings();
                settings._canSave = true;
                return settings;
            }
        }
        catch (FileNotFoundException)
        {
            file = await localFolder.CreateFileAsync(FILE_NAME);
            var settings = new Settings();
            await settings.saveAsync(file);
            return settings;
        }
    }

    private Settings()
    {
        AppRegistration = null;
        _canSave = true;
    }
    #endregion

    #region Private Functions
    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task saveAsync()
    {
        if (!_canSave)
            return;
        _canSave = false;
        var localFolder = ApplicationData.Current.LocalFolder;
        var file = await localFolder.GetFileAsync(FILE_NAME);
        await saveAsync(file);
    }

    private async Task saveAsync(StorageFile file)
    {
        if (!_canSave)
            return;
        _canSave = false;
        using (StreamWriter streamWriter = new StreamWriter(await file.OpenStreamForWriteAsync(), new UTF8Encoding(false)))
        using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
        {
            var serializer = new JsonSerializer();
            jsonWriter.Formatting = Formatting.Indented;
            serializer.Serialize(jsonWriter, this);
        }
        _canSave = true;
    }
    #endregion
}
