using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using IceAge.Interop;
using Mastonet;
using Mastonet.Entities;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
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
    private string _profileImageUrl;
    private string _username;
    private string _displayName;
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
            if (_timer.IsEnabled)
                _timer.Stop();
            _status = value;
            _interop.Content = Status.Content;
            IsFavorite = Status.Favourited.Value;
            IsBoosted = Status.Reblogged.Value;
            ReplyCount = Status.RepliesCount;
            BoostedCount = Status.ReblogCount;
            LockedAccount = Status.Account.Locked;
            ProfileImageUrl = Status.Account.AvatarUrl;
            Username = $"@{Status.Account.AccountName}";
            DisplayName = Status.Account.DisplayName;
            Created = Status.CreatedAt;
            _timer.Start();
        }
    }

    public bool IsBoostOrReply => true;

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
            NotifyPropertyChanged(nameof(BoostedCountVisibility));
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

    public Microsoft.UI.Xaml.Visibility BoostedCountVisibility => BoostedCount > 0 ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;

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
