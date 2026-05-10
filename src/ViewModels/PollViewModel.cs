using CommunityToolkit.Mvvm.ComponentModel;
using IceAge.Controls;
using Mastonet.Entities;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceAge.ViewModels;

public partial class PollViewModel : ObservableObject
{
    private readonly ResourceLoader _resourceLoader;

    [ObservableProperty]
    public partial Poll Poll { get; set; }
    public bool IsClosed => Poll.Expired;

    public string ExpiresIn
    {
        get
        {
            if (Poll == null || Poll.Expired)
            {
                return _resourceLoader.GetString("Poll/Ended");
            }
            else
            {
                var timeLeft = Poll.ExpiresAt - DateTime.UtcNow;
                if (timeLeft?.TotalMinutes < 1)
                {
                    return $"{timeLeft?.Seconds} {_resourceLoader.GetString("Poll/Ending/Seconds")}";
                }
                else if (timeLeft?.TotalHours < 1)
                {
                    return $"{timeLeft?.Minutes} {_resourceLoader.GetString("Poll/Ending/Minutes")}";
                }
                else if (timeLeft?.TotalDays < 1)
                {
                    return $"{timeLeft?.Hours} {_resourceLoader.GetString("Poll/Ending/Hours")}";
                }
                else
                {
                    return $"{timeLeft?.Days} {_resourceLoader.GetString("Poll/Ending/Days")}";
                }
            }
        }
    }

    public string TotalVotesText => $"{Poll?.VotesCount:N0} {_resourceLoader.GetString("Poll/VoterCountText")}";

    [ObservableProperty]
    public partial ObservableCollection<PollOptionControl> PollOptionControls { get; set; }

    public PollViewModel(Poll Poll)
    {
        this._resourceLoader = new ResourceLoader();
        this.Poll = Poll;
    }

    partial void OnPollChanged(Poll value)
    {
        OnPropertyChanged(nameof(IsClosed));
        OnPropertyChanged(nameof(ExpiresIn));
        OnPropertyChanged(nameof(TotalVotesText));
        PollOptionControls = new ObservableCollection<PollOptionControl>(Poll.Options.Select(option => new PollOptionControl(option, Poll.VotesCount)));
    }
}

