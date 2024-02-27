using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using IceAge.Interop;
using Mastonet;
using Mastonet.Entities;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class TootControl : UserControl, INotifyPropertyChanged
{
    private bool _isFavorite;
    private bool _isBoosted;
    private bool _lockedAccount;
    private long _boostedCount;
    private long _replyCount;
    private bool _isNavigatingToNewPage = false;
    private bool _isContentBoost;
    private bool _isBotAccount;
    private string _profileImageUrl;
    private string _username;
    private string _displayName;
    private string _originalUsername;
    private string _originalDisplayName;
    private readonly MastodonClient _client;
    private DateTime _created;
    private Status _status;
    private readonly RichTextInterop _interop;
    private readonly DispatcherTimer _timer;

    public TootControl(Status status, MastodonClient client)
    {
        this.InitializeComponent();
        _client = client;
        _interop = new RichTextInterop(ContentBlock);
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(30);
        _timer.Tick += (s, e) =>
        {
            if (Created.AddHours(2) <= DateTime.Now)
            {
                _timer.Stop();
            }
            NotifyPropertyChanged(nameof(Created));
            NotifyPropertyChanged(nameof(CreatedTimeAgo));
        };
        Status = status;
    }

    public Status Status
    {
        get => _status;
        set
        {
            if (_status == value)
                return;
            setStatusContent(value);
            NotifyPropertyChanged(nameof(Status));
        }
    }

    private void setStatusContent(Status value)
    {
        if (_timer.IsEnabled)
            _timer.Stop();
        _status = value;
        IsFavorite = Status.Favourited.Value;
        IsBoosted = Status.Reblogged.Value;
        IsBotAccount = Status.Account.Bot == true;
        ReplyCount = Status.RepliesCount;
        BoostedCount = Status.ReblogCount;
        Created = Status.CreatedAt;
        OriginalDisplayName = Status.Account.DisplayName;
        OriginalUsername = $"@{Status.Account.AccountName}";
        List<Attachment> mediaAttachments;
        bool isSensitive = Status.Sensitive == true || Status.Reblog?.Sensitive == true;

        if (Status.Reblog == null)
        {
            LockedAccount = Status.Account.Locked;
            ProfileImageUrl = Status.Account.AvatarUrl;
            Username = $"@{Status.Account.AccountName}";
            DisplayName = Status.Account.DisplayName;
            _interop.Content = Status.Content;
            IsContentBoost = false;
            mediaAttachments = Status.MediaAttachments.ToList();
        }
        else
        {
            LockedAccount = Status.Reblog.Account.Locked;
            ProfileImageUrl = Status.Reblog.Account.AvatarUrl;
            Username = $"@{Status.Reblog.Account.AccountName}";
            DisplayName = Status.Reblog.Account.DisplayName;
            _interop.Content = Status.Reblog.Content;
            IsContentBoost = true;
            mediaAttachments = Status.Reblog.MediaAttachments.ToList();
        }

        if (mediaAttachments?.Count > 0)
        {
            foreach (var item in mediaAttachments)
            {
                switch (item.Type)
                {
                    case "image":
                        uint width = (uint)(item.Meta.Small.Width ?? 200);
                        uint height = (uint)(item.Meta.Small.Height ?? 200);

                        if (mediaAttachments.Count > 1)
                        {
                            width = height = 200;
                        }

                        var img = new MediaAttachmentControl(item, isSensitive, width, height);
                        img.Tapped += Img_Tapped;
                        AttachmentBlock.Items.Add(img);
                        break;
                    case "gifv":
                        break;
                    case "video":
                        break;
                    case "audio":
                        break;
                    default:
                        break;
                }
            }
        }

        _timer.Start();
    }

    private async void Img_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var img = sender as MediaAttachmentControl;
        var dialog = new ImageContentDialog(img.MediaAttachment)
        {
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }

    public bool IsContentBoost
    {
        get => _isContentBoost;
        set
        {
            if ( _isContentBoost == value)
                return;
            _isContentBoost = value;
            NotifyPropertyChanged(nameof(IsContentBoost));
        }
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            if (_isFavorite == value)
                return;
            _isFavorite = value;
            NotifyPropertyChanged(nameof(IsFavorite));
        }
    }

    public bool IsBoosted
    {
        get => _isBoosted;
        set
        {
            if (_isBoosted == value)
                return;
            _isBoosted = value;
            NotifyPropertyChanged(nameof(IsBoosted));
            NotifyPropertyChanged(nameof(BoostedWeight));
        }
    }

    public long BoostedCount
    {
        get => _boostedCount;
        set
        {

            if (_boostedCount == value)
                return;
            _boostedCount = value;
            NotifyPropertyChanged(nameof(BoostedCount));
        }
    }

    public bool LockedAccount
    {
        get => _lockedAccount;
        set
        {
            if (_lockedAccount == value)
                return;
            _lockedAccount = value;
            NotifyPropertyChanged(nameof(LockedAccount));
            NotifyPropertyChanged(nameof(StatusGlyph));
        }
    }

    public string StatusGlyph
    {
        get
        {
            if (LockedAccount)
            {
                return "\xE785"; // Unlock
            }
            return "\xE774"; // Globe
        }
    }

    public string ProfileImageUrl
    {
        get => _profileImageUrl;
        set
        {
            if (_profileImageUrl == value)
                return;
            _profileImageUrl = value;
            NotifyPropertyChanged(nameof(ProfileImageUrl));
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            if (_username == value)
                return;
            _username = value;
            NotifyPropertyChanged(nameof(Username));
        }
    }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (_displayName == value)
                return;
            _displayName = value;
            NotifyPropertyChanged(nameof(DisplayName));
        }
    }

    public string OriginalUsername
    {
        get => _originalUsername;
        set
        {
            if (value == _originalUsername)
                return;
            _originalUsername = value;
            NotifyPropertyChanged(nameof(OriginalUsername));
        }
    }

    public string OriginalDisplayName
    {
        get => _originalDisplayName;
        set
        {
            if (OriginalDisplayName == value)
                return;
            _originalDisplayName = value;
            NotifyPropertyChanged(nameof(OriginalDisplayName));
        }
    }

    public DateTime Created
    {
        get => _created;
        set
        {
            if (_created == value)
                return;
            _created = value.ToLocalTime();
            NotifyPropertyChanged(nameof(Created));
            NotifyPropertyChanged(nameof(CreatedTimeAgo));
        }
    }

    public string CreatedTimeAgo
    {
        get
        {
            var diff = DateTime.Now - Created;
            if (diff.TotalDays > 365)
            {
                return $"{(int)(diff.TotalDays / 365)}y";
            }
            else if (diff.TotalDays > 30)
            {
                return $"{(int)(diff.TotalDays / 30)}m";
            }
            else if (diff.TotalDays > 1)
            {
                return $"{(int)diff.TotalDays}d";
            }
            else if (diff.TotalHours > 1)
            {
                return $"{(int)diff.TotalHours}h";
            }
            else if (diff.TotalMinutes > 1)
            {
                return $"{(int)diff.TotalMinutes}m";
            }
            else
            {
                return $"{(int)diff.TotalSeconds}s";
            }
        }
    }

    public long ReplyCount
    {
        get => _replyCount;
        set
        {
            if (_replyCount == value) return;
            _replyCount = value;
            NotifyPropertyChanged(nameof(ReplyCount));
            NotifyPropertyChanged(nameof(ReplyCountText));
        }
    }

    public string ReplyCountText
    {
        get
        {
            if (ReplyCount <= 0)
                return string.Empty;
            else if (ReplyCount == 1)
                return "1";
            else
                return "1+";
        }
    }

    public bool IsBotAccount
    {
        get => _isBotAccount;
        set
        {
            if (_isBotAccount == value) return;
            _isBotAccount = value;
            NotifyPropertyChanged(nameof(IsBotAccount));
        }
    }

    public FontWeight BoostedWeight => IsBoosted ? FontWeights.Bold : FontWeights.Normal;

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void ActionButton_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void FavoriteButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        FavoriteRotateAnimation.StartAsync();
        IsFavorite = !IsFavorite;
        if (IsFavorite)
            _client.Favourite(Status.Id);
        else
            _client.Unfavourite(Status.Id);
    }

    private void BoostButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        BoostScaleAnimation.StartAsync();
        IsBoosted = !IsBoosted;
        if (IsBoosted)
        {
            BoostedCount++;
            _client.Reblog(Status.Id);
        }
        else
        {
            BoostedCount--;
            _client.Unreblog(Status.Id);
        }
    }

    private async void ContentWebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
    {
        if (_isNavigatingToNewPage || !args.IsUserInitiated)
            return;
        _isNavigatingToNewPage = true;
        args.Cancel = true;
        await Launcher.LaunchUriAsync(new Uri(args.Uri));
        _isNavigatingToNewPage = false;
    }

    private void StackPanel_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        Debug.WriteLine(Status);
    }
}
