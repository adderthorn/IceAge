using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Interop;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceAge.ViewModels;

public partial class TootViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsFavorite { get; set; }

    [ObservableProperty]
    public partial bool IsBoosted { get; set; }

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
}
