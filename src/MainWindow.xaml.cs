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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public App ThisApp => App.Current as App;

    public MainWindow()
    {
        this.InitializeComponent();
        ThisApp.HttpClient = new HttpClient();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SystemBackdrop = new MicaBackdrop();
        if (ThisApp.Settings.Auth != null)
        {
            ContentFrame.Navigate(typeof(TimelinePage));
        }
        else
        {
            ContentFrame.Navigate(typeof(Pages.LoginPage));
        }
    }

    private void NewTootButton_Tapped(object sender, TappedRoutedEventArgs e)
    {
        // C# code to create a new window
        var newWindow = new NewTootWindow();
        newWindow.Activate();

        // C# code to navigate in the new window
    }
}
