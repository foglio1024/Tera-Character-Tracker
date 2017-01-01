using SharpPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PacketDotNet;
using PacketViewer.Network;
using Tera;
using TCTData.Enums;
using TCTParser;
using System.Threading;

namespace TCTSniffer
{
    public class SnifferProgram
    {


        public static void startNewSniffingSession()
        {
            var btd = new Tera.Data.BasicTeraData();
            Tera.Sniffing.TeraSniffer sniffer = new Tera.Sniffing.TeraSniffer(btd.Servers);
            sniffer.MessageReceived += teraSniffer_MessageReceived;
            sniffer.NewConnection += _teraSniffer_NewConnection;
            sniffer.Enabled = true;
        }

        static void _teraSniffer_NewConnection(Tera.Game.Server server)
        {
            Console.WriteLine("Connected to " + server.Name);
            UI.UpdateLog("Connected to: " + server.Name);
            UI.SendNotification("Connected to: " + server.Name, NotificationImage.Connected, NotificationType.Standard, UI.Colors.SolidGreen, false, false, false);
        }
        static void teraSniffer_MessageReceived(Tera.Message message)
        {
            //Message does not contain our length, add it to see the full packet
            byte[] data = new byte[message.Data.Count];
            Array.Copy(message.Data.Array, 0, data, 2, message.Data.Count - 2);
            data[0] = (byte)(((short)message.Data.Count) & 255);
            data[1] = (byte)(((short)message.Data.Count) >> 8);

            if (message.Direction == MessageDirection.ServerToClient)
            {
                Packet_old tmpPacket = new Packet_old(Direction.SC, message.OpCode, data, false); //**********************//

                DataParser.StoreLastPacket(tmpPacket.OpCode, tmpPacket.HexShortText);
                DataParser.StoreLastMessage(message);

            }


        }
        
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var p = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
           // var tcp = (TcpPacket)p.Extract(typeof(TcpPacket));
            if(p != null)
            {
                DateTime time = e.Packet.Timeval.Date;
                int len = e.Packet.Data.Length;
                var ip = (IpPacket)p.Extract(typeof(IpPacket));

                string srcIp = ip.SourceAddress.ToString();
                string dstIp = ip.DestinationAddress.ToString();

                //Console.WriteLine("{0}:{1}:{2},{3} Len={4}", time.Hour, time.Minute, time.Second, time.Millisecond, len);
                if(ip.Extract(typeof(TcpPacket)).PayloadData.Length > 0)
                {
                    Console.WriteLine(ByteArrayToString(ip.Extract(typeof(TcpPacket)).PayloadData));
                }
            }

        }

        /*
         *             string str = "52006ACA0600400032003900330039000B0071007500650073007400540065006D0070006C00610074006500490064000B003900390039003900370037000B007400610073006B00490064000B0032000000";
            str = str.Substring(100);

            XDocument DailyPlayGuideQuest = new XDocument();
            XDocument StrSheet_DailyPlayGuideQuest = new XDocument();
            DailyPlayGuideQuest = XDocument.Load("C:\\Users\\Vincenzo1\\OneDrive\\Tera\\Tera_PacketViewer-master\\build\\DailyPlayGuideQuest.xml");
            StrSheet_DailyPlayGuideQuest = XDocument.Load("C:\\Users\\Vincenzo1\\OneDrive\\Tera\\Tera_PacketViewer-master\\build\\StrSheet_DailyPlayGuideQuest.xml");


            if (str.Substring(str.Length - 8, 2) == "32")
            {
                StringBuilder sb0 = new StringBuilder();
                for (int i = 0; i < 24; i = i + 2)
                {
                    sb0.Append(str[i]);
                    sb0.Append(str[i + 1]);
                }
                sb0.Replace("00", "");
                var questIdAsByteArray = StringToByteArray(sb0.ToString());
                var questIdAsString = Encoding.UTF8.GetString(questIdAsByteArray);
                //int questId = 0;
                //Int32.TryParse(questIdAsString, out questId);

                XElement s = DailyPlayGuideQuest.Descendants().Where(x => (string)x.Attribute("questId") == questIdAsString).FirstOrDefault();
                var c = s.Descendants().Where(x => (string)x.Attribute("type") == "reputationPoint").FirstOrDefault().Attribute("amount").Value;
                int addedCredits = 0;
                Int32.TryParse(c, out addedCredits);
                addedCredits = addedCredits * 2;
                Console.WriteLine(addedCredits);

                XElement t = StrSheet_DailyPlayGuideQuest.Descendants().Where(x => (string)x.Attribute("id") == questIdAsString +"001").FirstOrDefault();
                var questname = t.Attribute("string").Value;
                Console.WriteLine(questname);

            }*/
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }
}

#region old_stuff
//string ver = SharpPcap.Version.VersionString;
//Console.WriteLine(ver);
//CaptureDeviceList devices = CaptureDeviceList.Instance;
//if(devices.Count < 1)
//{
//    Console.WriteLine("No devices");
//}
//else
//{
//    foreach (ICaptureDevice d in devices)
//    {
//        Console.WriteLine(d.ToString());
//    }
//}
//var device = devices[0];
//device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
//int readTimeoutMilliseconds = 1000;
//device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
//Console.WriteLine("Listening on {0}", device.Description);
///*host 79.110.94.112*/
//device.Filter = "ip and tcp and host 79.110.94.212";
//device.StartCapture();
//Console.ReadLine();
//device.StopCapture();
//device.Close();

#endregion