using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Tera
{
    internal class DgGaugeImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var runs = (int)value;
            var max = (int)parameter;
            if (runs != 0 && runs<=max)
            {
                return new BitmapImage(new Uri(@"C:/Users/Vincenzo1/OneDrive/Tera/Tera/resources/dg/dg" + max + "_" + runs + ".png", UriKind.Absolute));
            }
            else
            {
                return new BitmapImage(new Uri(@"C:/Users/Vincenzo1/OneDrive/Tera/Tera/resources/dg/d0.png", UriKind.Absolute));

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}