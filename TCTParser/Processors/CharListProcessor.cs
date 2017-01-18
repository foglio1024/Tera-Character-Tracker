using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTData.Enums;
using Tera;
using Tera.Converters;

namespace TCTParser.Processors
{
    internal class CharListProcessor : ListParser
    {
        const int CLASS_OFFSET_FROM_START = 28*2;
        const int LEVEL_OFFSET_FROM_START = 32*2;
        const int LOCATION_OFFSET_FROM_START = 52*2;
        const int LAST_ONLINE_OFFSET_FROM_START = 56*2;
        const int LAUREL_OFFSET_FROM_START = 298*2;
        const int POSITION_OFFSET_FROM_START = 302*2;
        const int GUILD_ID_OFFSET_FROM_START = 306*2;
        const int NAME_OFFSET_FROM_START = 310*2;
        const int GUILD_NAME_OFFSET_FROM_NAME = 96*2;
        const int FIRST_POINTER = 6;

        public string CurrentAccountId { get; set; }

        UnixToDateTime timeConverter = new UnixToDateTime();
        LocationIdToName lcc = new LocationIdToName();

        List<string> charStrings = new List<string>();

        private string GetName(string s)
        {
            StringBuilder b = new StringBuilder();
            bool eos = false;
            int i = 0;
            string c = "";
            while (!eos)
            {
                c = s.Substring(NAME_OFFSET_FROM_START + 4 * i, 4);
                if (c != "0000")
                {
                    b.Append(c);
                    i++;
                }
                else
                {
                    eos = true;
                }
            }

            b.Replace("00", "");
            string name = Encoding.UTF7.GetString(StringUtils.StringToByteArray(b.ToString()));
            return name;

        }
        private uint GetPosition(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = s.Substring(POSITION_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint pos = Convert.ToUInt32(b.ToString(), 16);
            return pos;

        }
        private string GetClass(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = s.Substring(CLASS_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint classIndex = Convert.ToUInt32(b.ToString(), 16);

            string cl = ((Class)classIndex).ToString();
            return cl;

        }
        private string GetLaurel(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = s.Substring(LAUREL_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint lrIndex = Convert.ToUInt32(b.ToString(), 16);

            string lr = ((Laurel)lrIndex).ToString();
            return lr;
        }
        private uint GetLevel(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = s.Substring(LEVEL_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint lv = Convert.ToUInt32(b.ToString(), 16);
            return lv;

        }
        private uint GetGuildId(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = "";

            c = s.Substring(GUILD_ID_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint gid = Convert.ToUInt32(b.ToString(), 16);
            return gid;

        }
        private uint GetLocationId(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = "";
            c = s.Substring(LOCATION_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint loc = Convert.ToUInt32(b.ToString(), 16);
            return loc;
        }
        private long GetLastOnline(string s)
        {
            StringBuilder b = new StringBuilder();
            StringReader r = new StringReader(s);
            string c = "";
            c = s.Substring(LAST_ONLINE_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            long lastOn = Convert.ToInt64(b.ToString(), 16);
            return lastOn;
        }
        private string GetGuildName(string s)
        {
            StringBuilder b = new StringBuilder();
            StringReader r = new StringReader(s);
            bool eos = false;
            int i = 0;
            string c = "";
            while (!eos)
            {
                c = s.Substring(NAME_OFFSET_FROM_START + GetName(s).Length * 4 + GUILD_NAME_OFFSET_FROM_NAME + 4 * i, 4);
                if (c != "0000")
                {
                    b.Append(c);
                    i++;
                }
                else
                {
                    eos = true;
                }
            }

            b.Replace("00", "");
            string gname = Encoding.UTF7.GetString(StringUtils.StringToByteArray(b.ToString()));
            return gname;

        }

        Character StringToCharacter(string s)
        {
            uint guildId = GetGuildId(s);
            uint pos = GetPosition(s);
            string name = GetName(s);
            string charClass = GetClass(s);
            uint level = GetLevel(s);
            uint loc = GetLocationId(s);
            long lastOn = GetLastOnline(s);
            string laurel = GetLaurel(s);


            return new Character(_index: pos,
                                _name: name,
                                _class: charClass,
                                _laurel: laurel,
                                _lvl: level,
                                _guildId: guildId,
                                _locationId: loc,
                                _lastOnline: lastOn,
                                _accId: CurrentAccountId
                                );
        }
        public List<Character> ParseCharacters(string p)
        {
            List<Character> _charList = new List<Character>();
            charStrings = ParseList(p);
            foreach (var str in charStrings)
            {
                var c = StringToCharacter(str);
                _charList.Add(c);

                if (!Tera.TeraLogic.GuildDictionary.ContainsKey(c.GuildId))
                {
                    TeraLogic.GuildDictionary.Add(c.GuildId, GetGuildName(str));
                }

                Console.WriteLine(("Found character: " + c.Name + " lv." + c.Level + " " + c.CharClass.ToLower() + ", logged out in " + lcc.Convert(c.LocationId, null, null, null) + " on " + timeConverter.Convert(c.LastOnline, null, null, null) + "."));
            }
            return _charList;
        }
        public void Clear()
        {
            charStrings.Clear();
        }

    }
}
