using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTParser.Processors
{
    internal class BankProcessor
    {
        private const int GOLD_OFFSET = 36 * 2;
        private const int BANK_TYPE_OFFSET = 16 * 2;
        private const int ACTION_OFFSET = 20 * 2;

        public long GetGoldAmount(string s)
        {
            return StringUtils.Hex8BStringToInt(s.Substring(GOLD_OFFSET)) / 10000;
        }
        public int GetBankType(string s)
        {
            return StringUtils.Hex1BStringToInt(s.Substring(BANK_TYPE_OFFSET, 2));
        }
        public bool IsOpenAction(string s)
        {
            if (StringUtils.Hex1BStringToInt(s.Substring(ACTION_OFFSET, 2)) == 0)
            {
                return true;
            }
            else if (StringUtils.Hex1BStringToInt(s.Substring(ACTION_OFFSET, 2)) == 1)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
