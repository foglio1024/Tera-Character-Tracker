using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera
{
    internal class graphCreditsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double maxHeight = (double)parameter;
            int credits = (int)value;
            double finalHeight = credits*maxHeight/9000;
            return finalHeight;

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}