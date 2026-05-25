using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using Mastonet;
using Mastonet.Entities;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace IceAge.ViewModels;

public partial class NewTootViewModel : ObservableObject
{
    public const int MaxTootLength = 500;

    public MastodonInterop MastodonInterop { get; private set; }

    [ObservableProperty]
    public partial Account Account { get; set; }

    [ObservableProperty]
    public partial string ProfileImageUrl {  get; set; }

    [ObservableProperty]
    public partial string Content {  get; set; }

    [ObservableProperty]
    public partial string ContentLength { get; private set; } = $"0 / {MaxTootLength}";

    [ObservableProperty]
    public partial Mastonet.Visibility SelectedVisibility { get; set; } = Mastonet.Visibility.Public;

    public string SelectedVisibilityGlyph
    {
        get
        {
            switch (SelectedVisibility)
            {
                case Mastonet.Visibility.Public:
                    return "\uE774";
                case Mastonet.Visibility.Unlisted:
                    return "\uE7ED";
                case Mastonet.Visibility.Private:
                    return "\uE72E";
                case Mastonet.Visibility.Direct:
                    return "@";
                default:
                    return string.Empty;
            }
        }
    }

    public string SelectedVisibilityFont
    {
        get
        {
            switch (SelectedVisibility)
            {
                case Mastonet.Visibility.Direct:
                    return "Segoe UI";
                default:
                    return "Segoe Fluent Icons";
            }
        }
    }

    public static async Task<NewTootViewModel> CreateAsync(MastodonInterop mastodonInterop)
    {
        var viewModel = new NewTootViewModel();
        viewModel.MastodonInterop = mastodonInterop;
        viewModel.Account = await mastodonInterop.MastodonClient.GetCurrentUser();

        return viewModel;
    }

    public async Task<Status> PostStatusAsync(string Content) => await MastodonInterop.MastodonClient.PublishStatus(
        Content,
        SelectedVisibility,
        replyStatusId: null,
        mediaIds: null,
        sensitive: false,
        spoilerText: null,
        scheduledAt: null,
        language: null,
        poll: null);

    public void SetVisibilityFromTag(string Tag)
    {
        switch (Tag)
        {
            case "public":
                SelectedVisibility = Mastonet.Visibility.Public;
                break;
            case "quiet":
                SelectedVisibility = Mastonet.Visibility.Unlisted;
                break;
            case "followers":
                SelectedVisibility = Mastonet.Visibility.Private;
                break;
            case "private":
                SelectedVisibility = Mastonet.Visibility.Direct;
                break;
        }
    }

    partial void OnAccountChanged(Account value)
    {
        ProfileImageUrl = value.AvatarUrl;
    }

    partial void OnContentChanged(string value)
    {
        ContentLength = $"{Content.Length} / {MaxTootLength}";
    }

    partial void OnSelectedVisibilityChanged(Visibility value)
    {
        OnPropertyChanged(nameof(SelectedVisibilityGlyph));
        OnPropertyChanged(nameof(SelectedVisibilityFont));
    }
}

public class StatusPostedEventArgs : EventArgs
{
    public Status Status { get; set; }

    public StatusPostedEventArgs(Status status)
    {
        Status = status;
    }
}