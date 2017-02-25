using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TCTData;

namespace TCTUI.Converters
{
    class WeeklyBonusToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool canGetBonus = (bool)value;
            int weeklies = (int)parameter;
            if (weeklies == TCTConstants.MAX_WEEKLY)
            {
                return new SolidColorBrush(TCTData.Colors.SolidGreen);
            }
            else
            {
                if (canGetBonus)
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
