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
    internal class GuildQuestListProcessor
    {
        const int QUEST_COUNT_OFFSET = 4 * 2;
        const int FIRST_ADDRESS_OFFSET = 6 * 2;
        const int GUILD_SIZE_OFFSET = 56 * 2;

        List<GuildQuest> QuestList = new List<GuildQuest>();

        List<int> addressList = new List<int>();
        List<string> questStringList = new List<string>();
        string guildListPacket;
        public QuestParser questParser;

        void Clear()
        {
            if (questParser != null)
            {
                questParser.Clear();
            }
            addressList.Clear();
            questStringList.Clear();
            QuestList.Clear();
        }
        public void ParseGuildListPacket(string p)
        {
            Clear();
            questParser = new QuestParser(p);
            guildListPacket = p;
            FillQuestStringList();
            foreach (var item in questStringList)
            {
                if (questParser.GetQuestSize(item) == GetGuildSize() && questParser.GetZoneID(item) != 152)
                {
                    QuestList.Add(new GuildQuest(questParser.GetQuestID(item),
                                                 questParser.GetStatus(item),
                                                 questParser.GetZoneID(item)));
                }
            }
        }

        void FillAddressList()
        {
            var firstAddress = StringUtils.Hex2BStringToInt(guildListPacket.Substring(FIRST_ADDRESS_OFFSET));
            var address = firstAddress * 2;
            addressList.Add(address);
            bool end = false;
            while (!end)
            {
                address = StringUtils.Hex2BStringToInt(guildListPacket.Substring(address + 4)) * 2;
                if (address == 0)
                {
                    end = true;
                }
                else
                {
                    addressList.Add(address);
                }
            }
        }
        void FillQuestStringList()
        {
            FillAddressList();
            for (int i = 0; i < addressList.Count; i++)
            {
                if (i == addressList.Count - 1)
                {
                    questStringList.Add(guildListPacket.Substring(addressList[i]));
                }
                else
                {
                    var start = addressList[i];
                    var len = addressList[i + 1] - start;
                    questStringList.Add(guildListPacket.Substring(start, len));
                }

            }
        }
        string GetGuildSize()
        {
            int size = StringUtils.Hex4BStringToInt(guildListPacket.Substring(GUILD_SIZE_OFFSET));
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
                        TCTNotifier.NotificationProvider.SendNotification("You have available guild quests for this dungeon.", NotificationImage.Default, Colors.Red, true, true);
                    }
                    break;
                }
            }
            if (!found)
            {
                //DO NOTHING
            }
        }

        public class QuestParser
        {
            const int QUEST_ID_OFFSET = 22 * 2;
            const int TARGET_LIST_ADDRESS_OFFSET = 6 * 2;
            const int TEMPLATE_ID_OFFSET = 8 * 2;
            const int ZONE_ID_OFFSET = 4 * 2;
            const int QUEST_SIZE_OFFSET = 30 * 2;
            const int QUEST_STATUS_OFFSET = 161 * 2;
            string guildListPacket;
            public void Clear()
            {
                guildListPacket = "";
            }
            public int GetQuestID(string q)
            {
                int questId = StringUtils.Hex2BStringToInt(q.Substring(QUEST_ID_OFFSET));
                return questId;
            }
            public int GetTemplateID(string q)
            {
                int targetsAddress = StringUtils.Hex2BStringToInt(q.Substring(TARGET_LIST_ADDRESS_OFFSET));
                string targets = guildListPacket.Substring(targetsAddress * 2);

                return StringUtils.Hex4BStringToInt(targets.Substring(TEMPLATE_ID_OFFSET));

            }
            public int GetZoneID(string q)
            {
                int targetsAddress = StringUtils.Hex2BStringToInt(q.Substring(TARGET_LIST_ADDRESS_OFFSET));
                string targets = guildListPacket.Substring(targetsAddress * 2);
                return StringUtils.Hex4BStringToInt(targets.Substring(ZONE_ID_OFFSET));

            }
            public string GetQuestSize(string q)
            {
                int size = StringUtils.Hex2BStringToInt(q.Substring(QUEST_SIZE_OFFSET));
                return ((GuildSize)size).ToString();
            }
            public GuildQuestStatus GetStatus(string p)
            {
                int status = StringUtils.Hex1BStringToInt(p.Substring(QUEST_STATUS_OFFSET, 2));
                return (GuildQuestStatus)status;

            }
            public QuestParser(string p)
            {
                guildListPacket = p;
            }

        }
        class RegionToZoneID : IValueConverter
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

        class GuildQuest
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
