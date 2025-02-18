using System.Globalization;
using System.Windows.Data;

namespace Ncs.WpfApp.Converters
{
    public class BooleanToSignInTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSignedIn)
            {
                return isSignedIn ? "Sign Out" : "Sign In";
            }
            return "Sign In";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Sign Out";
        }
    }
}
