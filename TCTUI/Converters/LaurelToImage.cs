using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Tera.Converters
{
    internal class LaurelToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _laurel = (string)value;
            _laurel.ToLower();
            object imagePath = "";
            if (_laurel.Length != 0 && _laurel != "-1")
            {
                    return new BitmapImage(new Uri("pack://application:,,,/resources/biglaurels/" +_laurel+  ".png", UriKind.Absolute));
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}