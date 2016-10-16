using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Tera;
namespace Tera.Converters
{
    internal class ClassToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _class = (string)value;
            _class.ToLower();
            object imagePath = "";
            if (_class.Length != 0 && _class.ToLower()!="none")
            {
                if (parameter as string == "hd")
                {
                    return new BitmapImage(new Uri("pack://application:,,,/resources/classes_hd/class_" + _class + ".png", UriKind.Absolute));
                }
                else
                {
                    return new BitmapImage(new Uri("pack://application:,,,/resources/classes_black_32x32/class_" + _class + ".png", UriKind.Absolute));
                }
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