using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TCTData
{
    public class ZoneIdToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int zoneId = System.Convert.ToInt32(value);
            return TCTDatabase.StrSheet_ZoneName.Descendants().Where(x => x.Name == "String").Where(x => System.Convert.ToInt32(x.Attribute("id").Value) == zoneId).First().Attribute("string").Value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
