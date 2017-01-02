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
    internal class CharListProcessor
    {
        const int CLASS_OFFSET_FROM_START = 60;
        const int LEVEL_OFFSET_FROM_START = 68;
        const int LOCATION_OFFSET_FROM_START = 108;
        const int LAST_ONLINE_OFFSET_FROM_START = 116;
        const int LAUREL_OFFSET_FROM_START = 600;
        const int POSITION_OFFSET_FROM_START = 608;
        const int GUILD_ID_OFFSET_FROM_START = 616;
        const int NAME_OFFSET_FROM_START = 624;
        const int GUILD_NAME_OFFSET_FROM_NAME = 196;
        const int FIRST_POINTER = 6;

        public string CurrentAccountId { get; set; }

        UnixToDateTime timeConverter = new UnixToDateTime();
        Location_IdToName lcc = new Location_IdToName();

        List<string> charStrings = new List<string>();
        List<int> indexesArray = new List<int>();

        private List<Tera.Character> sortChars(List<Tera.Character> c)
        {
            List<Tera.Character> newList = new List<Tera.Character>();
            uint maxIndex = 0;
            for (int i = 0; i < c.Count; i++)
            {
                if (maxIndex <= c[i].Position)
                {
                    maxIndex = c[i].Position;
                }
            }

            if (maxIndex == 0)
            {
                return c;
            }

            else
            {
                for (int i = 0; i <= maxIndex; i++)
                {
                    int newIndex = c.IndexOf(c.Find(x => x.Position == i));
                    if (newIndex >= 0)
                    {
                        newList.Add(c[newIndex]);
                    }
                }
                return newList;
            }
        }
        private string getName(string s)
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
        private uint getPosition(string s)
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
        private string getClass(string s)
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
        private string getLaurel(string s)
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
        private uint getLevel(string s)
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
        private uint getGuildId(string s)
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
        private uint getLocationId(string s)
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
        private long getLastOnline(string s)
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
        private string getGuildName(string s)
        {
            StringBuilder b = new StringBuilder();
            StringReader r = new StringReader(s);
            bool eos = false;
            int i = 0;
            string c = "";
            while (!eos)
            {
                c = s.Substring(NAME_OFFSET_FROM_START + getName(s).Length * 4 + GUILD_NAME_OFFSET_FROM_NAME + 4 * i, 4);
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
            //Console.WriteLine(gname);
            return gname;

        }

        void fillIndexesArray(string content)
        {
            int currentPointer = FIRST_POINTER;

            do
            {
                int lastPointer = readPointer(content, currentPointer * 2);
                indexesArray.Add(lastPointer);
                currentPointer = readPointer(content, lastPointer * 2 + 4);
            }
            while (currentPointer != 0);
        }
        void fillCharStrings(string content)
        {
            fillIndexesArray(content);
            int itemLenght = 0;
            for (int i = 0; i < indexesArray.Count; i++)
            {
                if (i != indexesArray.Count - 1)
                {
                    itemLenght = indexesArray[i + 1] - indexesArray[i];
                    charStrings.Add(content.Substring(indexesArray[i] * 2 + 4, itemLenght * 2));
                }
                else
                {
                    charStrings.Add(content.Substring(indexesArray[i] * 2 + 4));
                }

            }
        }
        Character StringToCharacter(string s)
        {
            uint guildId = getGuildId(s);
            uint pos = getPosition(s);
            string name = getName(s);
            string charClass = getClass(s);
            uint level = getLevel(s);
            uint loc = getLocationId(s);
            long lastOn = getLastOnline(s);
            string laurel = getLaurel(s);


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
        int readPointer(string content, int start)
        {
            return StringUtils.Hex2BStringToInt(content.Substring(start, 4));
        }

        public List<Character> ParseCharacters(string p)
        {
            List<Character> _charList = new List<Character>();
            fillCharStrings(p);

            foreach (var str in charStrings)
            {
                var c = StringToCharacter(str);
                _charList.Add(c);

                if (!Tera.TeraLogic.GuildDictionary.ContainsKey(c.GuildId))
                {
                    TeraLogic.GuildDictionary.Add(c.GuildId, getGuildName(str));
                }

                Console.WriteLine(("Found character: " + c.Name + " lv." + c.Level + " " + c.CharClass.ToLower() + ", logged out in " + lcc.Convert(c.LocationId, null, null, null) + " on " + timeConverter.Convert(c.LastOnline, null, null, null) + "."));
            }


            // var charList = sortChars(_charList);
            // return charList;
            return _charList;
        }
        public void Clear()
        {
            indexesArray.Clear();
            charStrings.Clear();
        }

    }
}
