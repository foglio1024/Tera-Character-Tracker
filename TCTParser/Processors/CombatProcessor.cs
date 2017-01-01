using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTParser.Processors
{
    class CombatProcessor
    {
        const int ID_OFFSET = 4 * 2;
        const int ID_LENGHT = 6 * 2;
        const int STATUS_OFFSET = 12 * 2;

        public bool IsInCombat { get; private set; }

        public string GetUserId(string p)
        {
            return p.Substring(ID_OFFSET, ID_LENGHT);
        }

        public void SetUserStatus(string p)
        {
            if (p.Substring(STATUS_OFFSET + 1, 1) == "0")
                IsInCombat = false;
            else IsInCombat = true;
        }
    }

}
