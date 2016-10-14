using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tera;

namespace Tera
{
    internal class DailiesGaugeFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int r = 0;
            var d = (string)value;
            Color col = new Color();
            if (int.TryParse(d, out r))
            {
                if (d.Length == 0) { d = "0"; }

                if (System.Convert.ToInt32(d) == 9)
                {
                    col = Color.FromArgb(0xa0, 88, 180, 91);
                }
                else if (System.Convert.ToInt32(d) < 3 && System.Convert.ToInt32(d) > 0)
                {
                    col = Color.FromArgb(0xa0, 255, 120, 42);
                }
                else if (System.Convert.ToInt32(d) > 9)
                {
                    col = Color.FromArgb(0xa0, 255, 42, 42);
                }
                else if (d.Equals("0"))
                {
                    col = Color.FromArgb(0, 0, 0, 0);
                }
                else
                {
                    col = Color.FromArgb(0xa0, 96, 125, 139);
                }

            }
            else
            {
                col = Color.FromArgb(0, 0, 0, 0);
            }
            return col;
            
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}