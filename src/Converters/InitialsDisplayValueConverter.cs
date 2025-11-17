using Microsoft.UI.Xaml.Data;
using System;

namespace IceAge.Converters;
public class InitialsDisplayValueConverter : IValueConverter
{
    /*
     * Hack to hide the initials for loaded profile pictures so they don't appear behind the picture,
     * this can cause them to display behind if there is an alpha channel, which makes for awkward visibility.
     */
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value == null ? null : "  ";

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException();
}
