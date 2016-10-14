using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera
{
    internal class DailiesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //int y;
            var x = new System.Windows.Visibility();
            if ((int)value == 9)
            {
               // y = 1;
                 x = System.Windows.Visibility.Visible;
            }
            else
            {
                 x = System.Windows.Visibility.Hidden;
               // y = 0; ;
            }
            //return y;

                return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}