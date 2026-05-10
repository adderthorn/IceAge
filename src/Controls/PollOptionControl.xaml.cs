using Mastonet.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;

public sealed partial class PollOptionControl : UserControl, INotifyPropertyChanged
{
    private readonly ResourceLoader _resourceLoader;
    private string _optionText;
    private int _votesCount;
    private long _totalVotes;

    public string OptionText
    {
        get => _optionText;
        set
        {
            if (_optionText != value)
            {
                _optionText = value;
                OnNotifyPropertyChanged(nameof(OptionText));
            }
        }
    }
    public int VotesCount
    {
        get => _votesCount;
        set
        {
            if (_votesCount != value)
            {
                _votesCount = value;
                OnNotifyPropertyChanged(nameof(VotesCount));
                OnNotifyPropertyChanged(nameof(VotePercentage));
                OnNotifyPropertyChanged(nameof(VotePercentageText));
                OnNotifyPropertyChanged(nameof(VotesCountText));
            }
        }
    }
    public long TotalVotes
    {
        get => _totalVotes;
        set
        {
            if (_totalVotes != value)
            {
                _totalVotes = value;
                OnNotifyPropertyChanged(nameof(TotalVotes));
                OnNotifyPropertyChanged(nameof(VotePercentage));
                OnNotifyPropertyChanged(nameof(VotePercentageText));
            }
        }
    }
    public double VotePercentage => TotalVotes == 0 ? 0 : (double)VotesCount / TotalVotes * 100;
    public string VotePercentageText => $"{VotePercentage:F0}%";
    public string VotesCountText => $"{VotesCount} {_resourceLoader.GetString("Poll/Votes")}";


    public event PropertyChangedEventHandler PropertyChanged;

    public PollOptionControl(PollOption pollOption, long totalVotes)
    {
        InitializeComponent();
        this._resourceLoader = new ResourceLoader();
        this.OptionText = pollOption.Title;
        this.VotesCount = pollOption.VotesCount ?? 0;
        this.TotalVotes = totalVotes;
    }

    private void OnNotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
