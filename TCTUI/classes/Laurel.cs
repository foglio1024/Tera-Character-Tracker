using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera
{
    public class Laurel
    {
        public string color { get; set; }
        public string name { get; set; }

        private readonly List<string> laurelRgbColors = new List<string>() { "ffffff","dc8f70", "d7d7d7", "ffe866", "8bdaff", "f7584f" };
        private readonly List<string> laurelNames = new List<string>() { "None", "Bronze", "Silver", "Gold", "Diamond", "Champion" };


        public Laurel() { color = "ffffff"; name = "None"; }

        public string getHexColorFromName(string n)
        {
            int i;
            return laurelRgbColors[i = laurelNames.IndexOf(n)];
        }
        public byte Hex2Byte(string rgb, string p)
        {
            byte v=0;
            switch (p){
                case "r":
                    v = Convert.ToByte("0x" + rgb.Substring(0, 2),16);
                    break;   
                case "g":
                    v = Convert.ToByte("0x" + rgb.Substring(2, 2),16);
                    break;
                case "b":
                    v = Convert.ToByte("0x" + rgb.Substring(4, 2),16);
                    break;
            }
            return v;
        }

    }
}
