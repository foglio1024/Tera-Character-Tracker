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
               return new SolidColorBrush(col = UI.Colors.FadedRed);
            }

            else if(runs > 0 && runs < max)
            {
               return new SolidColorBrush(col = UI.Colors.FadedYellow);
            }

            else if (runs == max)
            {
                return new SolidColorBrush(col = UI.Colors.FadedGreen);
            }

            else
            {
                return new SolidColorBrush(col = UI.Colors.FadedGray);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}