using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera
{
    internal class barConverter100 : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = (int)value;
            x = x * 180 / 100;
            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}