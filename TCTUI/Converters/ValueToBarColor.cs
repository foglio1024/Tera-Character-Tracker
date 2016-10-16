using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Tera.Converters
{
    internal class ValueToBarColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (int)value;
            var m = (int)parameter;
            if(v > m)
            {
                return new SolidColorBrush(TeraLogic.TCTProps.accentColor);
            }
            else
            {
                return SystemParameters.WindowGlassBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}