using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Tera.Converters
{

    public class TimeToAngle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long progress = (long)value;
            long max = 43200000;


            if(progress < max)
            {
                double v = 359.999 * ((double)progress / (double)max);
                return v;
            }
            else
            {
                return 359.99;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}