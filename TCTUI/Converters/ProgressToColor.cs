using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCTUI;

namespace TCTUI.Converters
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
                    return new SolidColorBrush(TCTData.Colors.SolidAccentColor);
                }
                else
                {
                    return new SolidColorBrush(TCTData.Colors.SolidBaseColor);

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
                    return new SolidColorBrush(TCTData.Colors.SolidBaseColor);
                }
                else
                {
                    return new SolidColorBrush(TCTData.Colors.SolidAccentColor);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}