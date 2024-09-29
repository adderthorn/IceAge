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
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isBoosted;

    [ObservableProperty]
    private bool _lockedAccount;

    [ObservableProperty]
    private long _boostedCount;

    [ObservableProperty]
    private long _replyCount;

    [ObservableProperty]
    private long _favoriteCount;

    [ObservableProperty]
    private bool _isNavigatingToNewPage;

    [ObservableProperty]
    private bool _isContentBoost;

    [ObservableProperty]
    private bool _isBotAccount;

    [ObservableProperty]
    private string _profileImageUrl;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private string _originalUsername;

    [ObservableProperty]
    private string _originalDisplayName;

    [ObservableProperty]
    private DateTime _created;
}
