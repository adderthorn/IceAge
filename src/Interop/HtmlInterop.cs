using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Mastonet.Entities;
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace IceAge.Interop;
internal class HtmlInterop : INotifyPropertyChanged
{
    private string _html;

    public Status Status { get; set; }

    public WebView2 WebView2 { get; set; }

    public HtmlInterop(Status status, WebView2 webView2)
    {
        this.Status = status;
        this.WebView2 = webView2;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public async Task NavigateToContentAsync()
    {
        if (_html == null)
        {
            var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Default.htm"));
            using var reader = new StreamReader(await htmlFile.OpenStreamForReadAsync());
            _html = await reader.ReadToEndAsync();
        }
        WebView2.NavigationCompleted += async (o, k) =>
        {
            string serializedStatusContent = Newtonsoft.Json.JsonConvert.SerializeObject(Status.Content);
            await WebView2.ExecuteScriptAsync($"setText({serializedStatusContent});");
        };
        WebView2.NavigateToString(_html);
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
