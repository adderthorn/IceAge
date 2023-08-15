using System;
using System.ComponentModel;
using Mastonet.Entities;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class TootControl : UserControl, INotifyPropertyChanged
{
    private bool _isFavorite;
    private bool _isBoosted;
    private bool _lockedAccount;
    private long _boostedCount = 1540;
    private string _profileImageUrl;
    private string _username;
    private string _displayName;
    private DateTime _created;
    private Status _status;

    public TootControl()
    {
        this.InitializeComponent();
    }

    public TootControl(Status status)
    {
        this.InitializeComponent();
        Status = status;
    }

    public Status Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            if (this.IsLoaded)
            {
                UserControl_Loaded(this, new RoutedEventArgs());
            }
            IsFavorite = Status.Favourited.Value;
            IsBoosted = Status.Reblogged.Value;
            BoostedCount = Status.ReblogCount;
            LockedAccount = Status.Account.Locked;
            ProfileImageUrl = Status.Account.AvatarUrl;
            Username = $"@{Status.Account.AccountName}";
            DisplayName = Status.Account.DisplayName;
            Created = Status.CreatedAt;
        }
    }

    public bool IsBoostOrReply => true;

    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            if (_isFavorite == value) return;
            _isFavorite = value;
            NotifyPropertyChanged(nameof(IsFavorite));
        }
    }

    public bool IsBoosted
    {
        get => _isBoosted;
        set
        {
            if (_isBoosted == value) return;
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

            if (_boostedCount == value) return;
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
            if (_lockedAccount == value) return;
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
            if (_profileImageUrl == value) return;
            _profileImageUrl = value;
            NotifyPropertyChanged(nameof(ProfileImageUrl));
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            if (_username == value) return;
            _username = value;
            NotifyPropertyChanged(nameof(Username));
        }
    }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (_displayName == value) return;
            _displayName = value;
            NotifyPropertyChanged(nameof(DisplayName));
        }
    }

    public DateTime Created
    {
        get => _created;
        set
        {
            if (_created == value) return;
            _created = value;
            NotifyPropertyChanged(nameof(Created));
            NotifyPropertyChanged(nameof(CreatedTimeAgo));
        }
    }

    public string CreatedTimeAgo
    {
        get
        {
            var diff = Created - DateTime.Now;
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

    public string ReplyCount => "1+";

    public Visibility BoostedCountVisibility => BoostedCount > 0 ? Visibility.Visible : Visibility.Collapsed;

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
    }

    private void BoostButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        BoostScaleAnimation.StartAsync();
        IsBoosted = !IsBoosted;
        if (IsBoosted)
        {
            BoostedCount++;
        }
        else
        {
            BoostedCount--;
        }
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await ContentWebView.EnsureCoreWebView2Async();
        //var str = @"<p><span class=""h-card""><a href=""https://infosec.exchange/@briankrebs"" class=""u-url mention"" rel=""nofollow noopener noreferrer"" target=""_blank"">@<span>briankrebs</span></a></span> I had this issue over the weekend their emails went into $null.</p><p>I had to email them twice, they said they sent me a new login email just in case (twice) but I only received their reply emails... Until I finally received one of the login emails and email flow started working again.</p><p>A US court permits bankrupt crypto lender Celsius to poll account holders on its proposal to relaunch as a user-owned company and distribute ~$2B of BTC and ETH (Jonathan Randles/Bloomberg)</p><p><a href=""https://www.bloomberg.com/news/articles/2023-08-14/celsius-to-poll-customers-on-launching-new-user-owned-company"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://www.</span><span class=""ellipsis"">bloomberg.com/news/articles/20</span><span class=""invisible"">23-08-14/celsius-to-poll-customers-on-launching-new-user-owned-company</span></a><br><a href=""http://www.techmeme.com/230814/p14#a230814p14"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">http://www.</span><span class=""ellipsis"">techmeme.com/230814/p14#a23081</span><span class=""invisible"">4p14</span></a></p><p>John Clifton Davies, a convicted fraudster estimated to have bilked dozens of technology startups out of more than $30 million through phony investment schemes, has a brand new pair of scam companies that are busy dashing startup dreams: A fake investment firm called Equity-Invest[.]ch, and Diligere[.]co.uk, a scam due diligence company that Equity-Invest insists all investment partners use.</p><p>A native of the United Kingdom, Mr. Davies absconded from justice before being convicted on multiple counts of fraud in 2015. Prior to his conviction, Davies served 16 months in jail before being cleared on suspicion of murdering his third wife on their honeymoon in India.</p><p><a href=""https://krebsonsecurity.com/2023/08/diligere-equity-invest-are-new-firms-of-u-k-con-man/"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://</span><span class=""ellipsis"">krebsonsecurity.com/2023/08/di</span><span class=""invisible"">ligere-equity-invest-are-new-firms-of-u-k-con-man/</span></a></p><p>Seriously though, MagSafe is the best thing since AirPods. Here’s the sticky thing I use when I don't want to start a fire with a charging puck: <a href=""https://www.amazon.com/gp/product/B09NCFFSWQ/ref=ewc_pr_img_1?smid=ATVPDKIKX0DER&amp;psc=1"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://www.</span><span class=""ellipsis"">amazon.com/gp/product/B09NCFFS</span><span class=""invisible"">WQ/ref=ewc_pr_img_1?smid=ATVPDKIKX0DER&amp;psc=1</span></a></p><p>Sources: Amazon's devices chief David Limp, who oversees Alexa, Echo, and more, plans to retire in the coming months; Amazon confirms he's leaving this year (Wall Street Journal)</p><p><a href=""https://www.wsj.com/articles/amazons-leader-on-alexa-echo-and-other-devices-plans-to-leave-cde4f689?mod=djemalertNEWS"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://www.</span><span class=""ellipsis"">wsj.com/articles/amazons-leade</span><span class=""invisible"">r-on-alexa-echo-and-other-devices-plans-to-leave-cde4f689?mod=djemalertNEWS</span></a><br><a href=""http://www.techmeme.com/230814/p13#a230814p13"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">http://www.</span><span class=""ellipsis"">techmeme.com/230814/p13#a23081</span><span class=""invisible"">4p13</span></a></p><p>You can just make the same ones yourself in a bunch of pretty colors <a href=""https://www.amazon.com/dp/B095KMC7XN/ref=sspa_dk_detail_4?pd_rd_i=B0BW4F546Z&amp;pd_rd_w=DY08m&amp;content-id=amzn1.sym.0d1092dc-81bb-493f-8769-d5c802257e94&amp;pf_rd_p=0d1092dc-81bb-493f-8769-d5c802257e94&amp;pf_rd_r=TSTGA8GQMQF6WR0VWW6H&amp;pd_rd_wg=nb5OD&amp;pd_rd_r=407b0276-05da-482a-9c14-8d0ecf14b813&amp;s=wireless&amp;sp_csd=d2lkZ2V0TmFtZT1zcF9kZXRhaWwy&amp;th=1"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://www.</span><span class=""ellipsis"">amazon.com/dp/B095KMC7XN/ref=s</span><span class=""invisible"">spa_dk_detail_4?pd_rd_i=B0BW4F546Z&amp;pd_rd_w=DY08m&amp;content-id=amzn1.sym.0d1092dc-81bb-493f-8769-d5c802257e94&amp;pf_rd_p=0d1092dc-81bb-493f-8769-d5c802257e94&amp;pf_rd_r=TSTGA8GQMQF6WR0VWW6H&amp;pd_rd_wg=nb5OD&amp;pd_rd_r=407b0276-05da-482a-9c14-8d0ecf14b813&amp;s=wireless&amp;sp_csd=d2lkZ2V0TmFtZT1zcF9kZXRhaWwy&amp;th=1</span></a></p><p>WAIT POP SOCKET HAD THE PILL SHAPED ONES AND NOW THEY TOOK MY LITTLE CUSTOM PUCK? I feel violated. <a href=""https://www.amazon.com/stores/page/D2E1AB9C-3501-438F-ADF1-C1A63E09E91B?ingress=2&amp;visitId=fe3a6500-118e-42a4-9eda-83de1978f495&amp;ref_=ast_bln"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://www.</span><span class=""ellipsis"">amazon.com/stores/page/D2E1AB9</span><span class=""invisible"">C-3501-438F-ADF1-C1A63E09E91B?ingress=2&amp;visitId=fe3a6500-118e-42a4-9eda-83de1978f495&amp;ref_=ast_bln</span></a></p><p>I keep coming back to Beatrice Rana&#39;s recording of the Goldberg Variations *over*, gasp, Glenn Gould’s (I prefer his 1955 one over 1981). <a href=""https://music.apple.com/us/album/bach-goldberg-variations-bwv-988/1196512243"" target=""_blank"" rel=""nofollow noopener noreferrer""><span class=""invisible"">https://</span><span class=""ellipsis"">music.apple.com/us/album/bach-</span><span class=""invisible"">goldberg-variations-bwv-988/1196512243</span></a> Also came across cellist Raphaela Gromes’s  collection, Femmes, from just this year—has a terrific range across centuries of music composed by women. <a href=""https://music.apple.com/us/album/femmes/1650200987"" target=""_blank"" rel=""nofollow noopener noreferrer""><span class=""invisible"">https://</span><span class=""ellipsis"">music.apple.com/us/album/femme</span><span class=""invisible"">s/1650200987</span></a></p><p>Is there a generally accepted term to describe what driverless taxis are doing when they are ""standing"" somewhere illegally blocking traffic because they're confused or don't know what to do next? If not, seems like a good opportunity to coin a new phrase. </p><p>Robonogo?<br>Tax-Z?<br>Geezing?<br>Snoozling?</p><p>Alright cut me some slack. It's a Monday</p><p>They are conflating multiple different battles about copyright, some of which affect current authors, making a living from their work, and some of which relate to orphansed works or works that should be out of copyright but due to vagaries, their status is unknown. If they focused, I believe there would be a much happier outcome. Their legal arguments are highly unconvincing to me. But their moral arguments have real standing. There’s also a lot of nonsense in copyright law about older works.</p><p>I am an absolute fan of the Internet Archive and all the work they’ve done to preserve cultural and technical history. But as this article makes clear, they are fighting a legal battle they cannot win, because the law is clear. They need to be fighting a structural battle, all about the law, because they will not win these cases. A judge would have to come up with novel interpretations that would surely be overturned at appellate or Supreme Court level. <a href=""https://www.nytimes.com/2023/08/13/business/media/internet-archive-emergency-lending-library.html?smid=nytcore-ios-share&amp;referringSource=articleShare"" target=""_blank"" rel=""nofollow noopener noreferrer""><span class=""invisible"">https://www.</span><span class=""ellipsis"">nytimes.com/2023/08/13/busines</span><span class=""invisible"">s/media/internet-archive-emergency-lending-library.html?smid=nytcore-ios-share&amp;referringSource=articleShare</span></a></p><p>He just can't help himself.	</p><p><a href=""https://secondnexus.com/trump-chutkan-order-january-6?utm_source=mastodon&amp;utm_medium=infeed&amp;utm_campaign=linkprogram"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://</span><span class=""ellipsis"">secondnexus.com/trump-chutkan-</span><span class=""invisible"">order-january-6?utm_source=mastodon&amp;utm_medium=infeed&amp;utm_campaign=linkprogram</span></a></p>";
        ContentWebView.NavigateToString(Status.Content);
    }
}
