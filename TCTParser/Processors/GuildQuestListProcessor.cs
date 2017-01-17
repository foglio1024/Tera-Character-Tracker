using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TCTData.Enums;
using Tera;
using Tera.Converters;

namespace TCTParser.Processors
{
    internal class GuildQuestListProcessor : ListParser
    {
        const int GUILD_SIZE_OFFSET = 56 * 2;

        List<GuildQuest> QuestList = new List<GuildQuest>();

        internal QuestParser questParser;

        void Clear()
        {
            QuestList.Clear();
        }
        public void ParseGuildListPacket(string p)
        {
            Console.WriteLine("Parsing guild quests...");

            questParser = new QuestParser();

            List<string> questStringList = ParseList(p);

            foreach (var item in questStringList)
            {
                if (questParser.GetQuestSize(item) == GetGuildSize(p) && questParser.GetZoneID(p) != 152)
                {
                    QuestList.Add(new GuildQuest(questParser.GetQuestID(p),
                                                 questParser.GetStatus(item),
                                                 questParser.GetZoneID(p)));
                    Console.WriteLine("id: {0} - status: {1} - zone: {2} ", QuestList.Last().ID, QuestList.Last().Status, QuestList.Last().ZoneId);
                }
            }
        }

        //ok
        string GetGuildSize(string p)
        {
            int size = StringUtils.Hex4BStringToInt(p.Substring(GUILD_SIZE_OFFSET));
            return ((GuildSize)size).ToString();
        }

        public void RemoveQuest(string p)
        {
            int id = StringUtils.Hex2BStringToInt(p.Substring(10));
            foreach (var quest in QuestList)
            {
                if (quest.ID == id)
                {
                    QuestList.Remove(quest);
                }
            }
        }

        public void TakeQuest(string p)
        {
            var id = StringUtils.Hex2BStringToInt(p.Substring(7 * 2));
            foreach (var quest in QuestList)
            {
                if (quest.ID == id)
                {
                    quest.Status = GuildQuestStatus.Taken;
                }
            }
        }

        public void CheckQuestStatus(uint locationId)
        {
            RegionToZoneID regionConverter = new RegionToZoneID();
            int zoneID = (int)regionConverter.Convert(locationId, null, null, null);
            bool found = false;
            foreach (var quest in QuestList)
            {
                if (quest.ZoneId == zoneID)
                {
                    found = true;
                    if (quest.Status == GuildQuestStatus.Available)
                    {
                        UI.UpdateLog("You have available guild quests for this dungeon.");
                        UI.SendNotification("You have available guild quests for this dungeon.", NotificationImage.Default, NotificationType.Standard, UI.Colors.SolidGreen, true, true, false);
                    }
                    break;
                }
            }
            if (!found)
            {
                //DO NOTHING
            }
        }

        internal class QuestParser
        {
            const int QUEST_ID_OFFSET = 18 * 2;
            const int QUEST_SIZE_OFFSET = 26 * 2;
            const int QUEST_STATUS_OFFSET = 35 * 2;

            const int TARGET_LIST_POINTER_OFFSET = 2 * 2;

            const int ZONE_ID_OFFSET = 4 * 2;
            const int TEMPLATE_ID_OFFSET = 8 * 2;

            public int GetQuestID(string q)
            {
                int questId = StringUtils.Hex2BStringToInt(q.Substring(QUEST_ID_OFFSET));
                return questId;
            }

            public int GetTemplateID(string p)
            {
                int targetsAddress = StringUtils.Hex2BStringToInt(p.Substring(TARGET_LIST_POINTER_OFFSET));
                string targets = p.Substring(targetsAddress * 2);
                return StringUtils.Hex4BStringToInt(targets.Substring(TEMPLATE_ID_OFFSET));
            }

            public int GetZoneID(string p)
            {
                int targetsAddress = StringUtils.Hex2BStringToInt(p.Substring(TARGET_LIST_POINTER_OFFSET));
                string targets = p.Substring(targetsAddress * 2);
                return StringUtils.Hex4BStringToInt(targets.Substring(ZONE_ID_OFFSET));
            }

            public string GetQuestSize(string q)
            {
                int size = StringUtils.Hex2BStringToInt(q.Substring(QUEST_SIZE_OFFSET));
                return ((GuildSize)size).ToString();
            }

            public GuildQuestStatus GetStatus(string q)
            {
                int status = StringUtils.Hex1BStringToInt(q.Substring(QUEST_STATUS_OFFSET, 2));
                return (GuildQuestStatus)status;
            }
        }


        private class RegionToZoneID : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                uint regionID = (uint)value;
                var regionToName = new Location_IdToName();
                string regionName = (string)regionToName.Convert(regionID, null, null, null);
                var el = TeraLogic.StrSheet_ZoneName.Descendants().Where(x => (string)x.Attribute("string") == regionName).FirstOrDefault();
                string zoneID = "";
                if (el != null)
                {
                    zoneID = el.Attribute("id").Value;
                    return System.Convert.ToInt32(zoneID);
                }
                else
                {
                    return -1;
                }
            }


            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        private class GuildQuest
        {
            public int ID { get; }
            public GuildQuestStatus Status { get; set; }
            public int ZoneId { get; }

            public GuildQuest(int _id, GuildQuestStatus _status, int _zId)
            {
                ID = _id;
                Status = _status;
                ZoneId = _zId;
            }
        }
    }
}
