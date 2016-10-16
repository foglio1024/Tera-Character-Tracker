using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tera.Converters
{
    internal class Laurel_GradeToColor : IValueConverter
    {
        private Dictionary<string, Color> LaurelDict = new Dictionary<string, Color>
        {
           { "None",     Colors.White },
           { "Bronze",   Color.FromRgb(0xdc, 0x8f, 0x70)},
           { "Silver" ,  Color.FromRgb(0xcc, 0xcc, 0xcc)},
           { "Gold",     Color.FromRgb(0xff, 0xdd, 0x00)},
           { "Diamond",  Color.FromRgb(0x8b, 0xda, 0xff)},
           { "Champion", Color.FromRgb(0xf7, 0x58, 0x4f)}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var grade = (string)value;          
            var col = new Color();
            LaurelDict.TryGetValue(grade, out col);
            return new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}