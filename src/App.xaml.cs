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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public HttpClient HttpClient { get; set; }
    public MastodonClient MastodonClient { get; set; }
    public AuthenticationClient AuthenticationClient { get; set; }
    public Auth Auth { get; set; }
    public static new App Current => Application.Current as App;
    public IServiceProvider Services { get; }

    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Settings>()
            .AddSingleton<ITootFactory, TootFactory>();

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
        this.HttpClient = new HttpClient();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow(this.Services.GetService<Settings>());
        m_window.Activate();
    }

    private Window m_window;
}
