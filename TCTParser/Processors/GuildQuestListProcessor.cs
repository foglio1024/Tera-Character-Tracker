using System;
using System.Collections.Generic;
using TCTData;
using TCTData.Enums;
using TCTUI;
using Tera;
using Tera.Game;
using Tera.Game.Messages;

namespace TCTParser.Processors
{
    internal class GuildQuestListProcessor : ListParser
    {
        const int GUILD_SIZE_OFFSET = 56 * 2;
        const int GUILD_QUESTS_COMPLETED_OFFSET = 67 * 2;
        const int GUILD_QUESTS_MAX_OFFSET = 71 * 2;

        //List<GuildQuest> QuestList = new List<GuildQuest>();

        GuildQuestList GuildQuests;

        public void Parse(Message m)
        {
            GuildQuests = new GuildQuestList(new TeraMessageReader(m, DataRouter.OpCodeNamer, DataRouter.Version, DataRouter.SystemOpCodeNamer));
            if (GuildQuests.DailiesDone < GuildQuests.DailiesMax)
            {
                UpdateUI();
            }
            else
            {
                ResetUI();
            }
        }

        //public void ParseGuildListPacket(string p)
        //{
        //    Console.WriteLine("Parsing guild quests...");
        //    var oldList = QuestList;
        //    QuestList.Clear();
        //    if (CompletedQuests(p) < MaxQuests(p))
        //    {
        //        //questParser = new GuildQuest(p);

        //        List<string> questStringList = ParseList(p);

        //        foreach (var quest in questStringList)
        //        {
        //            if (questParser.GetQuestSize(quest) == GetGuildSize(p) && questParser.GetZoneID(quest) != 152 && questParser.GetZoneID(quest) != 0)
        //            {
        //                try
        //                {
        //                    TCTData.ZoneToRegionID c = new TCTData.ZoneToRegionID();
        //                    QuestList.Add(new GuildQuest(questParser.GetQuestID(quest), questParser.GetStatus(quest), (int)c.Convert(questParser.GetZoneID(quest), null, null, null)));
        //                    Console.WriteLine("questID: {0} - status:{1} -- regionID:{2}", QuestList.Last().QuestID, QuestList.Last().Status, QuestList.Last().RegionID);
        //                }
        //                catch
        //                {
        //                    Console.WriteLine("Error while parsing guild quests");
        //                }
        //            }
        //        }

        //        UpdateUI(); 

        //        if(QuestList == oldList)
        //        {
        //            UI.UpdateLog("Guild quests list updated.");
        //        }
        //    }

        //    else
        //    {
        //        ResetUI();
        //    }

        //}

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
            foreach (var q in GuildQuests.Quests)
            {
                if (q.QuestType == GuildQuestType.Boss && q.QuestSize == GuildQuests.GuildSize)
                {
                    var c = new ZoneToRegionID();

                    UI.MainWin.Dispatcher.Invoke(() =>
                    {

                        try
                        {
                            string dgShortName = TCTData.Data.DungList.Find(d => d.Id == q.RegionID).ShortName;        //get dungeon short name

                            foreach (var counter in TeraMainWindow.DungeonCounters)      //search for right counter 
                            {
                                if (counter.Name == dgShortName)
                                {
                                    counter.SetGquestStatus(q.QuestStatus);     //update counter status
                                    break;
                                }
                            }

                        }
                        catch (Exception)
                        {
                            //UI.UpdateLog("Error while updating guild quests. ");
                        }

                    });

                }

        }
    }

    


        //void UpdateUI()
        //{

        //    ResetUI();
        //    //update
        //    foreach (var quest in GuildQuests.Quests)
        //    {
        //        if (TeraLogic.DungList.Find(d => d.Id == quest) != null)  //check that it is a dungeon quest
        //        {
        //            UI.MainWin.Dispatcher.Invoke(() =>
        //            {
        //                string dgShortName = TeraLogic.DungList.Find(d => d.Id == quest.RegionID).ShortName;        //get dungeon short name

        //                foreach (var counter in TeraMainWindow.DungeonCounters)      //search for right counter 
        //                {
        //                    if (counter.Name == dgShortName)
        //                    {
        //                        counter.SetGquestStatus(quest.Status);     //update counter status
        //                    }
        //                }
        //            });
        //        }
        //    }
        //}

        //string GetGuildSize(string p)
        //{
        //    int size = StringUtils.Hex4BStringToInt(p.Substring(GUILD_SIZE_OFFSET));
        //    return ((GuildSize)size).ToString();
        //}

        public void TakeQuest(string p)
        {
            var id = StringUtils.Hex2BStringToInt(p.Substring(7 * 2));
            foreach (var quest in GuildQuests.Quests)
            {
                if (quest.QuestType == GuildQuestType.Boss && quest.QuestSize == GuildQuests.GuildSize)
                {
                    if (quest.QuestId == id)
                    {
                        quest.QuestStatus = GuildQuestStatus.Taken;
                        UI.UpdateLog("Guild quest accepted.");
                        break;
                    }
                }
            }
            UpdateUI();
        }

        public void CheckQuestStatus(uint locationId, uint locNameId)
        {
            if (GuildQuests.DailiesDone < GuildQuests.DailiesMax)
            {
                bool found = false;
                foreach (var quest in GuildQuests.Quests)
                {
                    if (quest.QuestSize == GuildQuests.GuildSize)
                    {
                        if (quest.RegionID == locationId)
                        {
                            found = true;
                            if (quest.QuestStatus == GuildQuestStatus.Available)
                            {
                                UI.UpdateLog("You have guild quests available for this dungeon.");
                                UI.SendNotification("You have guild quests available for this dungeon.", NotificationImage.Default, NotificationType.Standard, TCTData.Colors.BrightGreen, true, true, false);
                            }
                            break;
                        }
                        else if (quest.RegionID == locNameId)
                        {
                            found = true;
                            if (quest.QuestStatus == GuildQuestStatus.Available)
                            {
                                UI.UpdateLog("You have guild quests available for this dungeon.");
                                UI.SendNotification("You have guild quests available for this dungeon.", NotificationImage.Default, NotificationType.Standard, TCTData.Colors.BrightGreen, true, true, false);
                            }
                            break;
                        }
                    }
                }
                if (!found)
                {
                    //DO NOTHING
                }
            }
        }


        class GuildQuestList : ParsedMessage
        {
            public GuildQuestList(TeraMessageReader r) : base(r)
            {
                itemsCount = r.ReadUInt16();
                firstQuestOffset = r.ReadUInt16();
                guildNameOffset = r.ReadUInt16();
                masterNameOffset = r.ReadUInt16();
                guildId = r.ReadUInt64();
                guildLvl = r.ReadUInt32();
                totalXp = r.ReadUInt32();
                unk1 =r.ReadUInt32();
                nextLvXp = r.ReadUInt32();
                unk2 = r.ReadUInt32();
                funds = r.ReadUInt32();
                unk3 = r.ReadUInt32();
                totalChars = r.ReadUInt32();
                totalAccounts = r.ReadUInt32();
                guildSize = r.ReadUInt32();
                dateCreated = r.ReadUInt64();
                dailyQuestsDone = r.ReadUInt32();
                maxDailyQuests = r.ReadUInt32();
                guildName = r.ReadTeraString();
                masterName = r.ReadTeraString();

                Quests = new List<GuildQuest>();
                for (int i = 0; i < itemsCount; i++)
                {
                    r.Skip(4);
                    var q = new GuildQuest(r);
                    Quests.Add(q);
                }
            }

            ushort itemsCount;
            ushort firstQuestOffset;
            ushort guildNameOffset;
            ushort masterNameOffset;
            ulong guildId;
            uint guildLvl;
            uint totalXp;
            uint unk1;
            uint nextLvXp;
            uint unk2;
            uint funds;
            uint unk3;
            uint totalChars;
            uint totalAccounts;
            uint guildSize;
            ulong dateCreated;
            uint dailyQuestsDone;
            uint maxDailyQuests;
            string guildName;
            string masterName;



            public ulong GuildId { get { return guildId; } }
            public string GuildName { get { return guildName; } }
            public uint GuildLevel { get { return guildLvl; } }

            public GuildSize GuildSize
            {
                get
                {
                    return ((GuildSize)guildSize);
                }
            }

            public uint DailiesDone { get { return dailyQuestsDone; } }
            public uint DailiesMax { get { return maxDailyQuests; } }

            public List<GuildQuest> Quests { get; set; }
        }
        class GuildQuest : ParsedMessage
        {
            public List<GuildQuestTarget> Targets { get; set; }
            public List<GuildQuestReward> Rewards { get; set; }
            public uint QuestId { get { return questId; } }
            public GuildSize QuestSize { get { return (GuildSize)guildSize; } }
            public GuildQuestType QuestType { get { return (GuildQuestType)questType; } }
            public GuildQuestStatus QuestStatus { get { return (GuildQuestStatus)questStatus; } set { questStatus = (uint)value; } }
            public int RegionID { get { return (int)conv.Convert(Targets[0].ZoneId, null, null, null); } }
            public GuildQuest(Tera.Game.TeraMessageReader r) : base(r)
            {
                targetsCount = r.ReadUInt16();
                targetsOffset = r.ReadUInt16();
                unk1 = r.ReadUInt16();
                unk2 = r.ReadUInt16();
                rewardsCount = r.ReadUInt16();
                rewardsOffset = r.ReadUInt16();
                unkOffset = r.ReadUInt16();
                questIdOffset = r.ReadUInt16();
                guildNameOffset = r.ReadUInt16();
                questId = r.ReadUInt16();
                unk3 = r.ReadUInt16();
                questType = r.ReadUInt16();
                unk5 = r.ReadUInt16();
                guildSize = r.ReadUInt16();
                unk6 = r.ReadUInt16();
                unk7 = r.ReadUInt16();
                unk8 = r.ReadUInt16();
                unk9 = r.ReadUInt16();
                unk10 = r.ReadUInt16();
                questStatus = r.ReadUInt32();
                timeLeft = r.ReadUInt32();
                unk11 = r.ReadUInt32();
                unk12 = r.ReadUInt32();
                completed = r.ReadByte();
                s1 = r.ReadTeraString();
                s2 = r.ReadTeraString();
                guildName = r.ReadTeraString();

                Targets = new List<GuildQuestTarget>();
                
                for (int i = 0; i < targetsCount; i++)
                {
                    var t = new GuildQuestTarget();
                    r.Skip(4);
                    t.ZoneId = r.ReadUInt32();
                    t.TemplateId = r.ReadUInt32();
                    t.AmountDone = r.ReadUInt32();
                    t.AmountRequired = r.ReadUInt32();

                    Targets.Add(t);
                }

                Rewards = new List<GuildQuestReward>();

                for (int i = 0; i < rewardsCount; i++)
                {
                    var re = new GuildQuestReward();
                    r.Skip(4);
                    re.Id = r.ReadUInt32();
                    re.Amount = r.ReadUInt32();
                    re.Unk16 = r.ReadUInt32();

                    Rewards.Add(re);
                }

            }
            #region fields
            ushort targetsCount;
            ushort targetsOffset;
            public ushort unk1;
            public ushort unk2;
            ushort rewardsCount;
            ushort rewardsOffset;
            ushort unkOffset;
            ushort questIdOffset;
            ushort guildNameOffset;
            ushort questId;
            public ushort unk3;
            ushort questType;
            public ushort unk5;
            ushort guildSize;
            public ushort unk6;
            public ushort unk7;
            public ushort unk8;
            public ushort unk9;
            public ushort unk10;
            uint questStatus;
            uint timeLeft;
            public uint unk11;
            public uint unk12;
            byte completed;
            string s1;
            string s2;
            string guildName;
            ZoneToRegionID conv = new ZoneToRegionID();
            #endregion


        }
        struct GuildQuestTarget
        {
            public uint ZoneId { get; set; }
            public uint TemplateId {get;set;}
            public uint AmountDone { get; set; }
            public uint AmountRequired { get; set; }

        }
        struct GuildQuestReward
        {
            public uint Id { get; set; }
            public uint Amount { get; set; }
            public uint Unk16 { get; set; }
        }
        //internal class QuestParser
        //{
        //    const int QUEST_ID_OFFSET = 18 * 2;
        //    const int QUEST_SIZE_OFFSET = 26 * 2;
        //    const int QUEST_STATUS_OFFSET = 38 * 2;

        //    const int TARGET_LIST_POINTER_OFFSET = 2 * 2;

        //    const int ZONE_ID_OFFSET = 4 * 2;
        //    const int TEMPLATE_ID_OFFSET = 8 * 2;

        //    string wholePacket;

        //    public int GetQuestID(string q)
        //    {
        //        int questId = StringUtils.Hex2BStringToInt(q.Substring(QUEST_ID_OFFSET));
        //        return questId;
        //    }
        //    public int GetTemplateID(string q)
        //    {
        //        int targetsAddress = StringUtils.Hex2BStringToInt(q.Substring(TARGET_LIST_POINTER_OFFSET));
        //        string targets = wholePacket.Substring(targetsAddress * 2);
        //        return StringUtils.Hex4BStringToInt(targets.Substring(TEMPLATE_ID_OFFSET));
        //    }
        //    public int GetZoneID(string q)
        //    {
        //        int targetsAddress = StringUtils.Hex2BStringToInt(q.Substring(TARGET_LIST_POINTER_OFFSET));
        //        string targets = wholePacket.Substring(targetsAddress * 2);
        //        return StringUtils.Hex4BStringToInt(targets.Substring(ZONE_ID_OFFSET));
        //    }
        //    public string GetQuestSize(string q)
        //    {
        //        int size = StringUtils.Hex2BStringToInt(q.Substring(QUEST_SIZE_OFFSET));
        //        return ((GuildSize)size).ToString();
        //    }
        //    public GuildQuestStatus GetStatus(string q)
        //    {
        //        int status = StringUtils.Hex1BStringToInt(q.Substring(QUEST_STATUS_OFFSET, 2));
        //        return (GuildQuestStatus)status;
        //    }

        //    //ctor
        //    public QuestParser(string p)
        //    {
        //        wholePacket = p;
        //    }
        //}

    }
}
