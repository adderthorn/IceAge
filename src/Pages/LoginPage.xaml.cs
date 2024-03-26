using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
using Mastonet;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;
using HtmlAgilityPack;
using System.Diagnostics;
using Microsoft.Windows.ApplicationModel.Resources;
using IceAge.ViewModels;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;



/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; }

    public LoginPage()
    {
        this.InitializeComponent();
        this.ViewModel = App.Current.Services.GetService<LoginViewModel>();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e) =>
        await ViewModel.AttemptLoginAsync(HandleTextBox.Text.Trim());

    private async void SignupButton_Click(object sender, RoutedEventArgs e) =>
        await Windows.System.Launcher.LaunchUriAsync(new Uri(IceAgeHelper.MastodonUrl));

    private void HandleTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.WaitingOnAuthCode)
                AuthCodeButton_Click(sender, e);
            else
                LoginButton_Click(sender, e);
        }
    }

    private async void AuthCodeButton_Click(object sender, RoutedEventArgs e) =>
        await ViewModel.AuthenticateAsync(AuthCodeTextBox.Text.Trim());
}
