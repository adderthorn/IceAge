using Mastonet.Entities;
using Mastonet;
using Microsoft.UI.Xaml;
using System.Net.Http;
using Application = Microsoft.UI.Xaml.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using IceAge.Interop;
using IceAge.TimelineFetcher;
using IceAge.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static new App Current => Application.Current as App;
    public Settings Settings { get; private set; }
    public IServiceProvider Services { get; }

    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MastodonInterop>()
            .AddSingleton<HomeTimelineFetcher>()
            .AddSingleton<LocalTimelineFetcher>()
            .AddSingleton<FederatedTimelineFetcher>()
            .AddSingleton<ExploreFetcher>()
            .AddSingleton<LoginViewModel>()
            .AddSingleton<TimelineViewModel>()
            .AddSingleton<NotificationsViewModel>()
            .AddSingleton<LocalViewModel>()
            .AddSingleton<FederatedViewModel>()
            .AddSingleton<ExploreViewModel>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.Services = ConfigureServices();
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        this.Settings = await Settings.CreateAsync();
        m_window = new MainWindow(this.Services.GetService<MastodonInterop>());
        m_window.Activate();
    }

    private Window m_window;
}
