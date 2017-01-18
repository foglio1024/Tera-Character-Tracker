using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTData
{
    public static class TCTDatabase
    {
        public static XDocument EventMatching;
        public static XDocument DailyPlayGuideQuest;
        public static XDocument StrSheet_DailyPlayGuideQuest;
        public static XDocument StrSheet_Region;
        public static XDocument StrSheet_Dungeon;
        public static XDocument StrSheet_ZoneName;
        public static XDocument NewWorldMapData;
        public static XDocument ContinentData;
        public static List<XDocument> StrSheet_Item_List;

        private static XDocument LoadXDocument(string fileName)
        {
            XDocument doc = new XDocument();
            return XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/" + fileName + ".xml");
        }

        public static void LoadTeraDB()
        {
            DailyPlayGuideQuest = LoadXDocument("DailyPlayGuideQuest");
            EventMatching = LoadXDocument("EventMatching");
            StrSheet_DailyPlayGuideQuest = LoadXDocument("StrSheet_DailyPlayGuideQuest");
            StrSheet_Region = LoadXDocument("StrSheet_Region");
            NewWorldMapData = LoadXDocument("NewWorldMapData");
            StrSheet_Dungeon = LoadXDocument("StrSheet_Dungeon-0");
            ContinentData = LoadXDocument("ContinentData");
            StrSheet_ZoneName = LoadXDocument("StrSheet_ZoneName");

            StrSheet_Item_List = new List<XDocument>();
            int i = 0;
            while (File.Exists(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_Item/StrSheet_Item-" + i + ".xml"))
            {
                //var doc = LoadXDocument("StrSheet_Item-" + i);
                //doc = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_Item/StrSheet_Item-" + i + ".xml");
                StrSheet_Item_List.Add(LoadXDocument("/StrSheet_Item/StrSheet_Item-" + i));
                i++;
            }
        }

    }
}
