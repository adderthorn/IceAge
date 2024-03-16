using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using IceAge.Interconnect;
using System.Net.Http;
using Mastonet;
using System.Diagnostics;
using Mastonet.Entities;
using System.Threading.Tasks;
using IceAge.Controls;
using System.Threading;
using IceAge.Pages;
using Windows.Graphics;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public App ThisApp => App.Current as App;

    public Settings Settings { get; }

    public static readonly Dictionary<string, Type> NavigationPageDictionary = new()
    {
        { "Home", typeof(TimelinePage) },
        { "Settings", typeof(SettingsPage) }
    };

    public MainWindow(Settings settings)
    {
        this.Settings = settings;
        this.InitializeComponent();
        ThisApp.HttpClient = new HttpClient();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SystemBackdrop = new MicaBackdrop();
        if (Settings.Auth != null)
        {
            ThisApp.Auth = Settings.Auth;
            ThisApp.MastodonClient = new MastodonClient(Settings.AppRegistration.Instance, Settings.Auth.AccessToken, ThisApp.HttpClient);
            Navigate(typeof(TimelinePage), new EntranceNavigationTransitionInfo());
        }
        else
        {
            Navigate(typeof(LoginPage));
        }

        this.AppWindow.Closing += (s, e) =>
        {
            // TODO Prevent this if window is maximized.
            if (Settings.SaveWindowSizeAndPosition
                && AppWindow.Size.Width > 0
                && AppWindow.Size.Height > 0)
            {
                var rect = new RectInt32(AppWindow.Position.X, AppWindow.Position.Y, AppWindow.Size.Width, AppWindow.Size.Height);
                Settings.WindowSizeAndPosition = rect;
            }
        };

        if (Settings.SaveWindowSizeAndPosition
            && Settings.WindowSizeAndPosition.Width > 0
            && Settings.WindowSizeAndPosition.Height > 0)
        {
            AppWindow.MoveAndResize(Settings.WindowSizeAndPosition);
        }
    }

    public void Navigate(Type typeToNavigateTo, NavigationTransitionInfo transitionInfo = null)
    {
        Type typeBeforeNavigation = ContentFrame.CurrentSourcePageType;

        if (typeToNavigateTo is not null && !Type.Equals(typeBeforeNavigation, typeToNavigateTo))
        {
            if (transitionInfo == null)
                ContentFrame.Navigate(typeToNavigateTo);
            else
                ContentFrame.Navigate(typeToNavigateTo, null, transitionInfo);
        }
    }

    public bool TryGoBack()
    {
        if (!ContentFrame.CanGoBack)
            return false;

        if (MainNavigationView.IsPaneOpen && (
            MainNavigationView.DisplayMode == NavigationViewDisplayMode.Compact || MainNavigationView.DisplayMode == NavigationViewDisplayMode.Minimal))
        {
            return false;
        }

        ContentFrame.GoBack();
        return true;
    }

    private void NewTootButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        // C# code to create a new window
        var newWindow = new NewTootWindow();
        newWindow.Activate();

        // C# code to navigate in the new window
    }

    private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
        }
        else if (args.SelectedItemContainer != null)
        {
            Type pageType;
            if (NavigationPageDictionary.TryGetValue(args.SelectedItemContainer.Name, out pageType))
            {
                Navigate(pageType, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                throw new ArgumentException($"No page type matching \"{args.SelectedItemContainer.Name}\"");
            }
        }
    }

    private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        _ = TryGoBack();

    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new ArgumentException($"Failed to load page: {e.SourcePageType.FullName}");
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        MainNavigationView.IsBackEnabled = ContentFrame.CanGoBack;

        if (ContentFrame.SourcePageType == typeof(SettingsPage))
        {
            MainNavigationView.SelectedItem = MainNavigationView.SettingsItem;
        }
        else if (ContentFrame.SourcePageType is not null)
        {
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(i => i.Name == ContentFrame.SourcePageType.Name);
        }
    }
}
