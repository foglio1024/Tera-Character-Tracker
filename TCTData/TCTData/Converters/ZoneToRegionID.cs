using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;

namespace TCTData
{
    public class ZoneToRegionID : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int zoneID = (int)value;
            if(zoneID != 0)
            {
                return System.Convert.ToInt32(TCTData.TCTDatabase.ContinentData.Descendants().Where(x => x.Name == "Continent").Descendants().Where(y => (string)y.Attribute("id").Value == zoneID.ToString()).FirstOrDefault().Parent.Attribute("id").Value.ToString());

            }
            else
            {
                return 0;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
