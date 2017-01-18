using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCTData;

namespace TCTData
{
    public class RegionIDToName
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            string locationName = "Unknown location";

            XElement s = TCTDatabase.NewWorldMapData.Descendants("Section").Where(x => (string)x.Attribute("id") == id.ToString()).FirstOrDefault();
            if (s != null)
            {
                XElement t = TCTDatabase.StrSheet_Region.Descendants().Where(x => (string)x.Attribute("id") == s.Attribute("nameId").Value).FirstOrDefault();

                if (t != null)
                {
                    locationName = t.Attribute("string").Value;
                }
            }



            return locationName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
