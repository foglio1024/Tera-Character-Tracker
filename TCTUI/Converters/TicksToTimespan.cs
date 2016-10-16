using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera.Converters
{
    internal class TicksToTimespan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var unixTime = (long)value;
            var ts = TimeSpan.FromMilliseconds(unixTime);
            string format = @"hh\:mm";
            return "Crystalbind left: " +  ts.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}