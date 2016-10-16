using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Tera.Converters
{

    public class ValueToAngle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int progress = (int)value;
            double max = (double)parameter;

            if(progress < max)
            {
                return 359.999 * (progress / max);
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