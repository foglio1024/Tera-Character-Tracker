using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tera.Converters;

namespace TCTParser.Processors
{
    internal class SectionProcessor
    {
        const int SECTION_ID_OFFSET = 13 * 2;

        public uint GetLocationNameId(string p)
        {
            uint id = GetLocationId(p); /*Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));*/
            XElement s = TCTData.TCTDatabase.NewWorldMapData.Descendants("Section").Where(x => (string)x.Attribute("id") == id.ToString()).FirstOrDefault();
            if (s != null)
            {
                id = Convert.ToUInt32(s.Attribute("nameId").Value);
            }
            return id;
        }
        public uint GetLocationId(string p)
        {
            return Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));
        }
        public string GetLocationName(string p)
        {
            var locId = GetLocationId(p);
            var c = new Location_IdToName();
            return (string)c.Convert(locId, null, null, null);
        }
    }
}
