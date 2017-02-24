using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TCTUI;

namespace Tera.Converters
{
    internal class Dungeon_RunsToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int max = 0;
            if (parameter != null)
            {
                max = (int)parameter;
            }
            int runs = 0;
            if (value != null)
            {
                runs = (int)value;
            }
            Color col = new Color();

            if (runs == 0)
            {
               return new SolidColorBrush(col = TCTData.Colors.SolidRed);
            }

            else if(runs > 0 && runs < max)
            {
               return new SolidColorBrush(col = TCTData.Colors.SolidYellow);
            }

            else if (runs == max)
            {
                return new SolidColorBrush(col = TCTData.Colors.SolidGreen);
            }

            else
            {
                return new SolidColorBrush(col = TCTData.Colors.SolidGray);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}