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
            int weeklies = (int)value;
            int dailies = (int)parameter;
            bool bonus = true;

            if (((DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && DateTime.Now.Hour >= TCTConstants.DAILY_RESET_HOUR) ||
                (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour < TCTConstants.DAILY_RESET_HOUR)) &&
                weeklies + dailies < TCTConstants.MAX_WEEKLY)
                    {
                        bonus = false;
                    }

            if (weeklies == TCTConstants.MAX_WEEKLY)
            {
                return new SolidColorBrush(TCTData.Colors.SolidGreen);
            }
            else
            {
                if (bonus)
                {
                    if ( //if time is between MON 5AM and TUE 5AM (one day left before reset)
                        ((DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour >= TCTData.TCTConstants.DAILY_RESET_HOUR) ||
                        (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && DateTime.Now.Hour < TCTData.TCTConstants.DAILY_RESET_HOUR)) &&
                        weeklies < TCTConstants.MAX_WEEKLY - TCTConstants.MAX_DAILY
                       )
                            {
                                return new SolidColorBrush(TCTData.Colors.SolidAccentColor); //warning, last chance to do weekly bonus
                            }
                    else
                            {
                                return new SolidColorBrush(TCTData.Colors.SolidBaseColor); //ok, still few days left to do weekly bonus
                            }
                }
                else
                {
                        return new SolidColorBrush(TCTData.Colors.SolidRed); //no bonus
                }
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
