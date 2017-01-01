using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTParser.Processors
{
    internal class AccountLoginProcessor
    {
        const string veteran_id = "B2010000";
        const string tc_id = "B1010000";

        public string id = "0";
        public bool tc = false;
        public bool vet = false;
        public long tcTime = 0;

        public void ParsePackageInfo(string p)
        {
            if (p.Contains(veteran_id))
            {
                vet = true;
            }
            if (p.Contains(tc_id))
            {
                tc = true;
                tcTime = StringUtils.Hex4BStringToInt(p.Substring(p.IndexOf(tc_id), 8));
            }
        }
        public void ParseLoginInfo(string p)
        {
            id = p.Substring(8, 12);
        }
    }

}
