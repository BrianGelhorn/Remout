using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Remout.Converters
{
    public class BoolToMovieStatusString : IValueConverter
    {
        public static readonly string PauseText = "||";
        public static readonly string PlayText = "|>";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? PauseText : PlayText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == PauseText;
        }
    }
}
