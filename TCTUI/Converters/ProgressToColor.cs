using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Tera.Converters
{
    internal class ProgressToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            object[] pars = parameter as object[];
            int max = (int)pars[0];
            int th = (int)pars[1];
            bool inv = (bool)pars[2];

            if (!inv)
            {
                if (val == max)
                {
                    return new SolidColorBrush(new Color { A = 0xa0, R = 88, G = 180, B = 91 });
                }

                else if (val < th)
                {
                    return new SolidColorBrush(TeraLogic.TCTProps.accentColor);//new Color { A = 0xa0, R = 255, G = 120, B = 42 });
                }
                else
                {
                    return SystemParameters.WindowGlassBrush;

                }
            }
            else
            {

                if (val == max)
                {
                    return new SolidColorBrush(new Color { A = 0xa0, R = 88, G = 180, B = 91 });
                }

                else if (val < th)
                {
                    return SystemParameters.WindowGlassBrush;
                }
                else
                {
                    return new SolidColorBrush(TeraLogic.TCTProps.accentColor);//new Color { A = 0xa0, R = 255, G = 120, B = 42 });
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}