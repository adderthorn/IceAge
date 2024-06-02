using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class ErrorContentDialog : ContentDialog, INotifyPropertyChanged
{
    private readonly ResourceLoader _resourceLoader;
    private string _errorMessage;

    public event PropertyChangedEventHandler PropertyChanged;

    private string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value != _errorMessage)
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }
    }

    public ErrorContentDialog(string ErrorMessage, XamlRoot XamlRoot)
    {
        this.InitializeComponent();
        this.ErrorMessage = ErrorMessage;
        this._resourceLoader = new ResourceLoader();
        this.XamlRoot = XamlRoot;
        this.Title = _resourceLoader.GetString("ErrorDialog/Title");
        this.PrimaryButtonText = _resourceLoader.GetString("ErrorDialog/PrimaryButton/Text");
        this.SecondaryButtonText = _resourceLoader.GetString("ErrorDialog/SecondaryButton/Text");
        this.CloseButtonText = _resourceLoader.GetString("ErrorDialog/CloseButton/Text");
        this.DefaultButton = ContentDialogButton.Primary;
    }

    private void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
