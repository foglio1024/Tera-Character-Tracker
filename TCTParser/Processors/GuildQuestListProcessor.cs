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
        const int GUILD_QUESTS_COMPLETED_OFFSET = 67 * 2;
        const int GUILD_QUESTS_MAX_OFFSET = 71 * 2;

        List<GuildQuest> QuestList = new List<GuildQuest>();

        internal QuestParser questParser;

        public void ParseGuildListPacket(string p)
        {
            Console.WriteLine("Parsing guild quests...");
            var oldList = QuestList;
            QuestList.Clear();
            if (CompletedQuests(p) < MaxQuests(p))
            {
                questParser = new QuestParser(p);

                List<string> questStringList = ParseList(p);

                foreach (var quest in questStringList)
                {
                    if (questParser.GetQuestSize(quest) == GetGuildSize(p) && questParser.GetZoneID(quest) != 152 && questParser.GetZoneID(quest) != 0)
                    {
                        TCTData.ZoneToRegionID c = new TCTData.ZoneToRegionID();
                        QuestList.Add(new GuildQuest(questParser.GetQuestID(quest), questParser.GetStatus(quest), (int)c.Convert(questParser.GetZoneID(quest), null, null, null)));
                        Console.WriteLine("questID: {0} - status:{1} -- regionID:{2}", QuestList.Last().QuestID, QuestList.Last().Status, QuestList.Last().RegionID);
                    }
                }
                UpdateUI(); 

                if(QuestList == oldList)
                {
                    UI.UpdateLog("Guild quests list updated.");
                }
            }

            else
            {
                ResetUI();
            }

        }

        void ResetUI()
        {
            //clear all
            UI.MainWin.Dispatcher.Invoke(() =>
            {
                foreach (var counter in TeraMainWindow.DungeonCounters)
                {
                    counter.SetGquestStatus(GuildQuestStatus.NotFound);
                }
            });
        }

        void UpdateUI()
        {

            ResetUI();
            //update
            foreach (var quest in QuestList)
            {
                UI.MainWin.Dispatcher.Invoke(() =>
                {
                    string dgShortName = TeraLogic.DungList.Find(d => d.Id == quest.RegionID).ShortName;

                    foreach (var counter in TeraMainWindow.DungeonCounters)
                    {
                        if (counter.Name == dgShortName)
                        {
                            counter.SetGquestStatus(quest.Status);
                        }
                    }
                });
            }
        }

        //ok
        string GetGuildSize(string p)
        {
            int size = StringUtils.Hex4BStringToInt(p.Substring(GUILD_SIZE_OFFSET));
            return ((GuildSize)size).ToString();
        }

        public void TakeQuest(string p)
        {
            var id = StringUtils.Hex2BStringToInt(p.Substring(7 * 2));
            foreach (var quest in QuestList)
            {
                if (quest.QuestID == id)
                {
                    quest.Status = GuildQuestStatus.Taken;
                    Tera.UI.UpdateLog("Guild quest accepted.");
                }
            }
            UpdateUI();
        }

        public void CheckQuestStatus(uint locationId)
        {
            bool found = false;
            foreach (var quest in QuestList)
            {
                if (quest.RegionID == locationId)
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

        int MaxQuests(string p)
        {
            return StringUtils.Hex2BStringToInt(p.Substring(GUILD_QUESTS_MAX_OFFSET, 4));
        }

        int CompletedQuests(string p)
        {
            return StringUtils.Hex2BStringToInt(p.Substring(GUILD_QUESTS_COMPLETED_OFFSET, 4));
        }

        internal class QuestParser
        {
            const int QUEST_ID_OFFSET = 18 * 2;
            const int QUEST_SIZE_OFFSET = 26 * 2;
            const int QUEST_STATUS_OFFSET = 35 * 2;

            const int TARGET_LIST_POINTER_OFFSET = 2 * 2;

            const int ZONE_ID_OFFSET = 4 * 2;
            const int TEMPLATE_ID_OFFSET = 8 * 2;

            string wholePacket;

            public int GetQuestID(string q)
            {
                int questId = StringUtils.Hex2BStringToInt(q.Substring(QUEST_ID_OFFSET));
                return questId;
            }

            public int GetTemplateID(string q)
            {
                int targetsAddress = StringUtils.Hex2BStringToInt(q.Substring(TARGET_LIST_POINTER_OFFSET));
                string targets = wholePacket.Substring(targetsAddress * 2);
                return StringUtils.Hex4BStringToInt(targets.Substring(TEMPLATE_ID_OFFSET));
            }

            public int GetZoneID(string q)
            {
                int targetsAddress = StringUtils.Hex2BStringToInt(q.Substring(TARGET_LIST_POINTER_OFFSET));
                string targets = wholePacket.Substring(targetsAddress * 2);
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

            //ctor
            public QuestParser(string p)
            {
                wholePacket = p;
            }
        }

        private class GuildQuest
        {
            public int QuestID { get; }
            public GuildQuestStatus Status { get; set; }
            public int RegionID { get; }

            public GuildQuest(int _questId, GuildQuestStatus _status, int _rId)
            {
                QuestID = _questId;
                Status = _status;
                RegionID = _rId;
            }
        }
    }
}
