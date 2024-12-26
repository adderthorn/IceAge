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

    [ObservableProperty]
    public partial Status Status { get; set; }

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

    public TootViewModel(Status status, MastodonClient client, bool shortenHyperlinks)
    {
        _client = client;
        Status = status;
    }

    partial void OnIsBoostedChanged(bool value)
    {
        OnPropertyChanged(nameof(BoostedWeight));
    }

    partial void OnStatusChanged(Status value)
    {
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
    }
}
