using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
               return new SolidColorBrush(col = Color.FromArgb(0xa0, 255, 80, 80));
            }

            else if(runs > 0 && runs < max)
            {
               return new SolidColorBrush(col = Color.FromArgb(0xa0, 255, 166, 77));
            }

            else if (runs == max)
            {
                return new SolidColorBrush(col = Color.FromArgb(0xa0, 88, 180, 91));
            }

            else
            {
                return new SolidColorBrush(col = Color.FromArgb(0xa0, 96, 125, 139));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}