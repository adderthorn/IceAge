using IceAge.Controls;
using Mastonet;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using Mastonet.Entities;
using Windows.Storage;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using IceAge.Interop;
using Windows.Graphics.Display;
using IceAge.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TimelinePage : Page
{
    public TimelineViewModel ViewModel { get; }

    public TimelinePage(TimelineViewModel viewModel)
    {
        this.InitializeComponent();
        this.ViewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.Fetcher.StartStreamingAsync();
    }
}
