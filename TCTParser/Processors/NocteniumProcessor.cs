using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTParser.Processors
{
    internal class NocteniumProcessor
    {
        const int CHAR_ID_OFFSET = 4 * 2;
        const int CHAR_ID_LENGTH = 12;
        readonly int[] NOCT_IDS = { 902, 910, 911, 912, 913, 916, 999010000 };
        const int B_BUFF_ID_OFFSET = 20 * 2;
        const int E_BUFF_ID_OFFSET = 12 * 2;

        public bool IsNocteniumOn { get; private set; }

        internal void ParseBegin(string packet, string currentCharId)
        {
            var charId = packet.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGTH);
            if(charId == currentCharId)
            {
                var buffId = StringUtils.Hex4BStringToInt(packet.Substring(B_BUFF_ID_OFFSET, 8));
                if (NOCT_IDS.Contains(buffId))
                {
                    SetNocteniumOn();
                }
            }
        }
        
        internal void ParseEnd(string packet, string currentCharId)
        {
            var charId = packet.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGTH);
            if (charId == currentCharId)
            {
                var buffId = StringUtils.Hex4BStringToInt(packet.Substring(E_BUFF_ID_OFFSET, 8));
                if (NOCT_IDS.Contains(buffId))
                {
                    SetNocteniumOff();

                }
            }
        }

        private void SetNocteniumOn()
        {
            IsNocteniumOn = true;
        }

        private void SetNocteniumOff()
        {
            IsNocteniumOn = false;
        }
    }
}
