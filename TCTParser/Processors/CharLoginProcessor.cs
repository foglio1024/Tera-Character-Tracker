using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTParser.Processors
{
    internal class CharLoginProcessor
    {
        const int NAME_OFFSET_FROM_START = 290 * 2;
        const int ID_OFFSET_FROM_START = 18 * 2;
        const int ID_LENGHT = 12;

        public string GetName(string content)
        {
            return StringUtils.GetStringFromHex(content, NAME_OFFSET_FROM_START, "0000");
        }
        public string GetId(string content)
        {
            return content.Substring(ID_OFFSET_FROM_START, ID_LENGHT);
        }

    }
}
