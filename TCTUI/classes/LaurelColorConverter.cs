using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tera
{
    internal class LaurelColorConverter : IValueConverter
    {
        public string color { get; set; }
        public string name { get; set; }

        private List<string> laurelRgbColors = new List<string>() { "ffffffff", "ffdc8f70", "ffd7d7d7", "ffffe866", "ff8bdaff", "fff7584f" };
        private List<string> laurelNames = new List<string>() { "None", "Bronze", "Silver", "Gold", "Diamond", "Champion" };
       

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] argb = new byte[4];
            var l_name = (string)value;
            var l_col = laurelRgbColors[(laurelNames.IndexOf(laurelNames.Find(x => x.Equals(l_name))))];
            for (int i=0;i<4; i++)
            {
               
                argb[i] = System.Convert.ToByte("0x" + l_col.Substring(i * 2, 2), 16);
            }
            
            var col = new SolidColorBrush();
            col.Color = Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
            return col;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}