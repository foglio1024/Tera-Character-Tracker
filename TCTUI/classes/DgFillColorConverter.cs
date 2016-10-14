using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tera
{
    internal class DgFillColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int max = 0;
            if (parameter != null)
            {
                max = (int)parameter;
            }
            int runs = 0;
            if (value != null)
            {
                runs = (int)value;
            }
            Color col = new Color();

            if (runs == 0)
            {
               return new SolidColorBrush(col = Color.FromArgb(0xa0, 255, 80, 80));
            }

            else if(runs > 0 && runs < max)
            {
               return new SolidColorBrush(col = Color.FromArgb(0xa0, 255, 166, 77));
            }

            else if (runs == max)
            {
                return new SolidColorBrush(col = Color.FromArgb(0xa0, 88, 180, 91));
            }

            else
            {
                return new SolidColorBrush(col = Color.FromArgb(0xa0, 96, 125, 139));
            }
            /*           int r = 0;
                       var d = (int)value;
                       var m = (int)parameter;
                       Color col = new Color();
                       if (int.TryParse(d, out r))
                       {
                           if (d.Length == 0) { d = "0"; }
                           if (System.Convert.ToInt32(d) == System.Convert.ToInt32(m))
                           {
                               col = Color.FromArgb(0xa0, 88, 180, 91);
                           }
                           else if (System.Convert.ToInt32(d) < System.Convert.ToInt32(m) && System.Convert.ToInt32(d) > 0)
                           {
                               col = Color.FromArgb(0xa0, 255, 120, 42);
                           }

                           else if (d.Equals("0"))
                           {
                               col = Color.FromArgb(20, 0, 0, 0);
                           }
                           else
                           {
                               col = Color.FromArgb(0xa0, 96, 125, 139);
                           }

                       }
                       else
                       {
                           col = Color.FromArgb(20, 0, 0, 0);
                       }
                       return col;
           */

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}