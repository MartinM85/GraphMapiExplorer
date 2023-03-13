using System.Globalization;

namespace MapiExplorer.UI.Converters
{
    public class CalendarColorToMauiColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string color && !string.IsNullOrEmpty(color))
            {
                return color.ToLower() switch
                {
                    "auto" => Colors.WhiteSmoke,
                    "lightblue" => Colors.LightBlue,
                    "lightgreen" => Colors.LightGreen,
                    "lightorange" => Colors.Orange,
                    "lightgray" => Colors.LightGray,
                    "lightyellow" => Colors.LightYellow,
                    "lightteal" => Colors.Teal,
                    "lightpink" => Colors.LightPink,
                    "lightbrown" => Colors.Brown,
                    "lightred" => Colors.MediumVioletRed,
                    _ => Colors.Transparent,
                };
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
