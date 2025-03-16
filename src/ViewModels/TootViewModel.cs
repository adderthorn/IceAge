using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Controls;
using IceAge.Interop;
using Mastonet;
using Mastonet.Entities;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;

namespace IceAge.ViewModels;

public partial class TootViewModel : ObservableObject
{
    private readonly MastodonClient _client;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    public partial Status Status { get; set; }

    [ObservableProperty]
    public partial string Content {  get; set; }

    [ObservableProperty]
    public partial bool IsFavorite { get; set; }

    [ObservableProperty]
    public partial bool IsBoosted { get; set; }

    public FontWeight BoostedWeight => IsBoosted ? FontWeights.Bold : FontWeights.Normal;

    [ObservableProperty]
    public partial bool LockedAccount { get; set; }

    [ObservableProperty]
    public partial long BoostedCount { get; set; }

    [ObservableProperty]
    public partial long ReplyCount { get; set; }

    [ObservableProperty]
    public partial long FavoriteCount { get; set; }

    [ObservableProperty]
    public partial bool IsNavigatingToNewPage { get; set; }

    [ObservableProperty]
    public partial bool IsContentBoost { get; set; }

    [ObservableProperty]
    public partial bool IsBotAccount { get; set; }

    [ObservableProperty]
    public partial string ProfileImageUrl { get; set; }

    [ObservableProperty]
    public partial string Username { get; set; }

    [ObservableProperty]
    public partial string DisplayName { get; set; }

    [ObservableProperty]
    public partial string OriginalUsername { get; set; }

    [ObservableProperty]
    public partial string OriginalDisplayName { get; set; }

    [ObservableProperty]
    public partial DateTime Created { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<UserControl> AttachmentControls { get; set; }

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
                if (diff.TotalSeconds < 0)
                {
                    return "0s";
                }
                return $"{(int)Math.Ceiling(diff.TotalSeconds)}s";
            }
        }
    }

    public string StatusGlyph => LockedAccount
        ? "\xE785" //Unlock
        : "\xE774"; //Globe

    public TootViewModel(MastodonInterop mastodonInterop, Status status)
        //(Status status, MastodonClient client, bool shortenHyperlinks)
    {
        _client = mastodonInterop.MastodonClient;
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(30);
        _timer.Tick += (s, e) =>
        {
            if (Created.AddHours(2) <= DateTime.Now)
            {
                _timer.Stop();
            }
            OnPropertyChanged(nameof(Created));
            OnPropertyChanged(nameof(CreatedTimeAgo));
        };
        AttachmentControls = new ObservableCollection<UserControl>();
        Status = status;
    }

    partial void OnIsBoostedChanged(bool value)
    {
        OnPropertyChanged(nameof(BoostedWeight));
    }

    partial void OnCreatedChanged(DateTime value)
    {
        OnPropertyChanged(nameof(CreatedTimeAgo));
    }

    partial void OnLockedAccountChanged(bool value)
    {
        OnPropertyChanged(nameof(StatusGlyph));
    }

    partial void OnStatusChanged(Status value)
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }
        IsFavorite = value.Favourited ?? false;
        IsBoosted = value.Reblogged ?? false;
        IsBotAccount = value.Account.Bot ?? false;
        ReplyCount = value.RepliesCount;
        BoostedCount = value.ReblogCount;
        OriginalDisplayName = value.Account.DisplayName;
        OriginalUsername = $"@{value.Account.AccountName}";
        bool isSensitive = value.Sensitive == true || value.Reblog?.Sensitive == true;
        List<Attachment> attachments;

        if (value.Reblog == null)
        {
            Created = value.CreatedAt;
            LockedAccount = value.Account.Locked;
            ProfileImageUrl = value.Account.AvatarUrl;
            Username = OriginalUsername;
            DisplayName = OriginalDisplayName;
            Content = Status.Content;
            IsContentBoost = false;
            attachments = value.MediaAttachments.ToList();
        }
        else
        {
            Created = value.Reblog.CreatedAt;
            LockedAccount = value.Reblog.Account.Locked;
            ProfileImageUrl = value.Reblog.Account.AvatarUrl;
            Username = $"@{value.Reblog.Account.AccountName}";
            DisplayName = value.Reblog.Account.DisplayName;
            Content = Status.Reblog.Content;
            IsContentBoost = true;
            attachments = value.Reblog.MediaAttachments.ToList();
        }

        foreach (var item in attachments)
        {
            uint width = (uint)(item.Meta?.Small?.Width ?? 200);
            uint height = (uint)(item.Meta?.Small?.Height ?? 200);
            if (attachments?.Count > 1)
            {
                width = height = 200;
            }

            switch (item.Type)
            {
                case "image":
                    var img = new ImageAttachmentControl(item, isSensitive, width, height);
                    AttachmentControls.Add(img);
                    break;
                case "gifv":
                case "video":
                    var ani = new AnimatedPreviewAttachmentControl(item, isSensitive, width, height, autoplay: false);
                    AttachmentControls.Add(ani);
                    break;
                case "audio":
                    throw new NotImplementedException();
                    break;
            }
        }
        _timer.Start();
    }

    public void ToggleFavorite(bool? isFavorite = null)
    {
        isFavorite ??= !IsFavorite;
        IsFavorite = isFavorite.Value;
        if (IsFavorite)
        {
            FavoriteCount++;
            _client.Favourite(Status.Id);
        }
        else
        {
            FavoriteCount--;
            _client.Unfavourite(Status.Id);
        }
    }

    public void ToggleBoost(bool? isBoost = null)
    {
        isBoost ??= !IsBoosted;
        IsBoosted = isBoost.Value;
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
}
