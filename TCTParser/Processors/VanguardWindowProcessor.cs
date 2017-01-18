using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera;

namespace TCTParser.Processors
{
    internal class VanguardWindowProcessor
    {
        const int WEEKLY_OFFSET = 28 * 2;
        const int CREDITS_OFFSET = 32 * 2;
        const int DAILY_OFFSET = 12 * 2;

        public bool justLoggedIn = true;

        public int GetWeekly(string content)
        {
            int w = StringUtils.Hex4BStringToInt(content.Substring(WEEKLY_OFFSET));
            if (w > TeraLogic.MAX_WEEKLY) { w = TeraLogic.MAX_WEEKLY; }
            return w;
        }
        public int GetCredits(string content)
        {
            return StringUtils.Hex4BStringToInt(content.Substring(CREDITS_OFFSET));
        }
        public int GetDaily(string content)
        {
            return StringUtils.Hex4BStringToInt(content.Substring(DAILY_OFFSET));
        }
    }

}
