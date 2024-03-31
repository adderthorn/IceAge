using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IceAge.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public Settings Settings => App.Current.Settings;

    public SettingsPage()
    {
        this.InitializeComponent();
    }

    private void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var button = sender as RadioButton;
        switch (button.Tag.ToString())
        {
            case "Default":
                Settings.ElementTheme = ElementTheme.Default;
                break;
            case "Light":
                Settings.ElementTheme = ElementTheme.Light;
                break;
            case "Dark":
                Settings.ElementTheme = ElementTheme.Dark;
                break;
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        string selectedTag;
        switch (Settings.ElementTheme)
        {
            case ElementTheme.Light:
                selectedTag = "Light";
                break;
            case ElementTheme.Dark:
                selectedTag = "Dark";
                break;
            default:
                selectedTag = "Default";
                break;
        }
        ThemeSettingsCardButtons.SelectedItem = ThemeSettingsCardButtons.Items
            .FirstOrDefault(i => (i as RadioButton).Tag.ToString() == selectedTag);
    }
}
