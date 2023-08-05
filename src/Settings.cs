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
    #endregion

    #region Public Properties
    public const string AppName = "IceAge";
    public event PropertyChangedEventHandler PropertyChanged;

    public AppRegistration AppRegistration
    {
        get => _appRegistration;
        set
        {
            if (_appRegistration == value) return;
            _appRegistration = value;
            RaisePropertyChanged(nameof(AppRegistration));
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
                var settings = serializer.Deserialize<Settings>(reader);
                settings._canSave = true;
                return settings;
            }
        }
        catch (FileNotFoundException)
        {
            file = await localFolder.CreateFileAsync(FILE_NAME);
            var settings = new Settings();
            await settings.saveAsync();
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
        if (!_canSave) return;
        var localFolder = ApplicationData.Current.LocalFolder;
        var file = await localFolder.GetFileAsync(FILE_NAME);
        using (StreamWriter streamWriter = new StreamWriter(await file.OpenStreamForWriteAsync(), new UTF8Encoding(false)))
        using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
        {
            var serializer = new JsonSerializer();
            serializer.Serialize(jsonWriter, this);
        }
    }
    #endregion
}
