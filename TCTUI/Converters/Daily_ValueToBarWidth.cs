using System;
using System.Globalization;
using System.Windows.Data;
using TCTData;

namespace TCTUI.Converters
{
    internal class Daily_ValueToBarWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double[] pars = parameter as double[];
            double maxLenght = pars[0];
            double weeklyLeft = pars[1];
            int dailiesLeft = (int)value;
            int dailiesDone = TCTConstants.MAX_DAILY - dailiesLeft;
            double offset = dailiesLeft - weeklyLeft;
            if(offset < 0) { offset = 0; }
            double unitLenght = maxLenght/ TCTConstants.MAX_WEEKLY;

            double lenght = unitLenght * (TCTConstants.MAX_DAILY - dailiesDone - offset);
            return lenght;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}