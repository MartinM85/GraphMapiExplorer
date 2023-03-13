using MapiExplorer.Models;
using System.Globalization;

namespace MapiExplorer.UI.Converters
{
    public class UserAvailabilityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserAvailability availability)
            {
                return availability switch
                {
                    UserAvailability.Available => Colors.Green,
                    UserAvailability.AvailableIdle => Colors.LightGreen,
                    UserAvailability.Away => Colors.Yellow,
                    UserAvailability.BeRightBack => Colors.LightYellow,
                    UserAvailability.Busy => Colors.Red,
                    UserAvailability.BusyIdle => Colors.OrangeRed,
                    UserAvailability.DoNotDisturb => Colors.Purple,
                    UserAvailability.Offline => Colors.DarkRed,
                    UserAvailability.PresenceUnknown => Colors.Gray,
                    _ => throw new NotImplementedException(),
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
