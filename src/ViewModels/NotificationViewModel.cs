using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using Mastonet.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.Windows.ApplicationModel.Resources;

namespace IceAge.ViewModels;
public partial class NotificationViewModel : ObservableObject
{
    private readonly MastodonInterop _interop;
    private readonly ResourceLoader _resourceLoader;

    public TootViewModel TootViewModel { get; private set; }

    [ObservableProperty]
    public partial Notification Notification { get; set; }

    [ObservableProperty]
    public partial DateTime Created { get; set; }

    public string CreatedCurrentCulture => Created.ToString("f", CultureInfo.CurrentCulture);

    [ObservableProperty]
    public partial string Type { get; set; }

    public Visibility AccountVisibility
    {
        get
        {
            switch (this.Type)
            {
                case "mention":
                case "reblog":
                case "favourite":
                case "follow":
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }
    }

    public string Phrase => _resourceLoader.GetString($"NotificationType/{this.Type}");

    public String Glyph
    {
        get
        {
            switch (this.Type)
            {
                case "mention":
                    return "\xE97A";
                case "reblog":
                    return "\xE8EB";
                case "follow":
                    return "\xE8FA";
                case "favourite":
                    return "\xE734";
                case "poll":
                    return "\xEADF";
                default:
                    return string.Empty;
            }
        }
    }

    public NotificationViewModel(MastodonInterop mastodonInterop, Notification notification)
    {
        this._interop = mastodonInterop;
        this._resourceLoader = new ResourceLoader();
        Notification = notification;
       
    }

    partial void OnNotificationChanged(Notification value)
    {
        if (value.Status != null)
        {
            TootViewModel = new TootViewModel(_interop, value.Status);
        }
        Created = value.CreatedAt;
        Type = value.Type;
        OnPropertyChanged(nameof(AccountVisibility));
    }

    partial void OnCreatedChanged(DateTime value)
    {
        OnPropertyChanged(nameof(CreatedCurrentCulture));
    }
}
