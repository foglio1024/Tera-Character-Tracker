using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera
{
    internal class barConverter9000 : IValueConverter
    {
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = (int)value;
            x = x*180 / 9000;
            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}