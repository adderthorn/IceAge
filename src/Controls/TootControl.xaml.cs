using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Controls;
public sealed partial class TootControl : UserControl, INotifyPropertyChanged
{
    private bool _isFavorite;
    private bool _isBoosted;
    private int _boostedCount = 1540;

    public TootControl()
    {
        this.InitializeComponent();
    }

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

    public int BoostedCount
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
}
