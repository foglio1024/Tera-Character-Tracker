using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tera
{
    internal class DgEllipseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (val > 0)
            {
                return new SolidColorBrush(new Color
                {
                    A = 200,
                    R = 80,
                    G = 180,
                    B = 91
                });
            }
            else
            {
                return new SolidColorBrush(new Color
                {
                    A = 200,
                    R = 255,
                    G = 80,
                    B = 80
                });
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}