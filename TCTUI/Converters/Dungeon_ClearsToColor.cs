using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tera
{
    internal class Dungeon_ClearsToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value >= 5)
            {
                return new SolidColorBrush(UI.Colors.FadedYellow);
            }
            else
            {
                return new SolidColorBrush(UI.Colors.FadedGreen);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}