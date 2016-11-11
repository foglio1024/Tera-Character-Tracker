using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTBridge
{
    public static class DataBridge
    {
        public static ushort LastPacketOpCode;
        public static string LastPacketData;
        public static void StoreLastPacket(ushort opCode, string data)
        {
            LastPacketData = data;
            LastPacketOpCode = opCode;
        }
    }
}
