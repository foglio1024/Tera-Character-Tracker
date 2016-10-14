using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Tera
{
    internal class barColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (int)value;
            var m = (int)parameter;
            if(v > m)
            {
                return new SolidColorBrush(new Color { A = 255, R = 255, G = 120, B = 42 });
            }
            else
            {
                //return new SolidColorBrush(new Color { A = 255, R = 255, G = 120, B = 42 });
                return SystemParameters.WindowGlassBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}