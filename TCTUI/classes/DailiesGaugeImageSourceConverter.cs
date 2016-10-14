using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tera
{
    internal class DailiesGaugeImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int r = 0;
            var d = value.ToString();
            if (int.TryParse(d, out r))
            {
                if (d.Equals(""))
                {
                    d = "0";
                }
                if ((System.Convert.ToInt32(d) > 9))
                {
                    d = "9";
                }
            }
            else
            {
                d = "0";
            }
            return  new BitmapImage(new Uri(@"C:/Users/Vincenzo1/OneDrive/Tera/Tera/resources/d_states/d"+d+".png", UriKind.Absolute));
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}