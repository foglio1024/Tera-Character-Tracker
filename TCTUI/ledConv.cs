using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Tera
{
    internal class ledConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isOn = (bool)value;
            var off = new SolidColorBrush( new Color { A = 30, R = 0, B = 0, G = 0 });
            var on = new SolidColorBrush( new Color { A = 150, R = 220, B = 0, G = 35 });
            if (isOn)
            {
                return on;
            }
            else
            {
                return off;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
