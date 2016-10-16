using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera.Converters
{
    internal class Daily_ValueToBarWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double[] pars = parameter as double[];
            double maxLenght = pars[0];
            double weeklyLeft = pars[1];
            int dailiesLeft = (int)value;
            int dailiesDone = TeraLogic.MAX_DAILY - dailiesLeft;
            double offset = dailiesLeft - weeklyLeft;
            if(offset < 0) { offset = 0; }
            double unitLenght = maxLenght/Tera.TeraLogic.MAX_WEEKLY;

            double lenght = unitLenght * (Tera.TeraLogic.MAX_DAILY - dailiesDone - offset);
            return lenght;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}