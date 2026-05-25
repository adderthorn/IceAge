using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using Mastonet.Entities;
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

    public static async Task<NewTootViewModel> CreateAsync(MastodonInterop mastodonInterop)
    {
        var viewModel = new NewTootViewModel();
        viewModel.MastodonInterop = mastodonInterop;
        viewModel.Account = await mastodonInterop.MastodonClient.GetCurrentUser();

        return viewModel;
    }

    public async Task<Status> PostStatusAsync(string Content) => await MastodonInterop.MastodonClient.PublishStatus(
        Content,
        Mastonet.Visibility.Public,
        replyStatusId: null,
        mediaIds: null,
        sensitive: false,
        spoilerText: null,
        scheduledAt: null,
        language: null,
        poll: null);

    partial void OnAccountChanged(Account value)
    {
        ProfileImageUrl = value.AvatarUrl;
    }

    partial void OnContentChanged(string value)
    {
        ContentLength = $"{Content.Length} / {MaxTootLength}";
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