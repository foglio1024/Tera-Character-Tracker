using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TCTUI.Converters
{
    class ValueToBarLenght : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            double baseLenght = (parameter as double[])[0];
            double max = (parameter as double[])[1];
            if(val > max)
            {
                return baseLenght;
            }
            else
            {
                return val * baseLenght / max;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
