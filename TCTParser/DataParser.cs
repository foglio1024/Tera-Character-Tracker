using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Tera;
using Tera.Game.Messages;
using Tera.Game;
using Tera.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Tera.Converters;
using System.Windows.Data;
using System.Globalization;
using TCTData.Enums;

namespace TCTParser
{
    public static class DataParser
    {
        const int VANGUARD_REP_ID = 609;

        static S_GET_USER_GUILD_LOGO mess;

        private static string wCforDungeons;
        private static string wCforUpdatedCreditsAfterPurchase;
        private static List<string> wCforCcb = new List<string>();
        private static string currentCharName;
        private static string currentCharId;
        private static bool isInCombat;
        static BasicTeraData btd = new BasicTeraData();
        static OpCodeNamer opn = new OpCodeNamer(Path.Combine(btd.ResourceDirectory, string.Format("opcodes-{0}.txt", "3907eu")));

        static CharListProcessor charListProcessor = new CharListProcessor();
        static CharLoginProcessor charLoginProcessor = new CharLoginProcessor();
        static VanguardWindowProcessor vanguardWindowProcessor = new VanguardWindowProcessor();
        static InventoryProcessor inventoryProcessor = new InventoryProcessor();
        static SectionProcessor sectionProcessor = new SectionProcessor();
        static CrystalbindProcessor crystalbindProcessor = new CrystalbindProcessor();
        static AccountLoginProcessor accountLoginProcessor = new AccountLoginProcessor();
        static GuildQuestListProcessor guildQuestListProcessor = new GuildQuestListProcessor();
        static BankProcessor bankProcessor = new BankProcessor();
        static SystemMessageProcessor sysMsgProcessor = new SystemMessageProcessor();
        static CombatParser combatParser = new CombatParser();

        public static void StoreLastMessage(Message msg)
        {
            if (opn.GetName(msg.OpCode) == "S_GET_USER_GUILD_LOGO")
            {
                TeraMessageReader tmr = new TeraMessageReader(msg, opn);
                mess = new S_GET_USER_GUILD_LOGO(tmr);
                var t = new Thread(new ThreadStart(SetLogo));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }
        /*****/
        public static void StoreLastPacket(ushort opCode, string data)
        {
            switch (opn.GetName(opCode))
            {
                case "S_GET_USER_LIST":
                    #region LIST
                    crystalbindProcessor.Clear();
                    if (!TeraLogic.AccountList.Contains(TeraLogic.AccountList.Find(x => x.Id == accountLoginProcessor.id)))
                    {
                        TeraLogic.AccountList.Add(new Account(accountLoginProcessor.id, accountLoginProcessor.tc, accountLoginProcessor.vet, accountLoginProcessor.tcTime));
                    }
                    charListProcessor.CurrentAccountId = accountLoginProcessor.id;
                    SetCharList(data);
                    TeraLogic.SaveAccounts(true);
                    TeraLogic.SaveCharacters(true);

                    Tera.UI.UpdateLog("Data saved.");

                    break;
                #endregion

                case "S_LOGIN":
                    #region LOGIN
                    crystalbindProcessor.Clear();
                    LoginChar(data);
                    break;
                #endregion

                case "S_AVAILABLE_EVENT_MATCHING_LIST":
                    #region VANGUARD WINDOW
                    SetVanguardData(data, vanguardWindowProcessor.justLoggedIn);
                    vanguardWindowProcessor.justLoggedIn = false;
                    break;
                #endregion

                case "S_INVEN":
                    if (!isInCombat)
                    {
                        if (data[53].ToString() == "1") /*wait next packet*/
                        {
                            inventoryProcessor.multiplePackets = true;
                            inventoryProcessor.p1 = data;
                        }
                        else if (data[53].ToString() == "0")/*is last/unique packet*/
                        {
                            if (inventoryProcessor.multiplePackets)
                            {
                                inventoryProcessor.p2 = data;
                                inventoryProcessor.multiplePackets = false;
                            }
                            else
                            {
                                inventoryProcessor.p1 = data;
                            }

                            SetTokens(inventoryProcessor.justLoggedIn);
                            inventoryProcessor.justLoggedIn = false;
                        }
                        CurrentChar().Ilvl = inventoryProcessor.GetItemLevel(data); 
                    }
                    break;

                case "S_DUNGEON_COOL_TIME_LIST":
                    #region DUNGEONS RUNS
                    Tera.UI.UpdateLog(currentCharName + " > received dungeons data.");
                    wCforDungeons = data;
                    setDungs();
                    break;
                #endregion

                case "S_SYSTEM_MESSAGE":
                    #region SYSTEM MESSAGE
                    sysMsgProcessor.ParseSystemMessage(data);
                    //#region DUNGEON ENGAGED
                    ///*dungeon engaged*/
                    //if (data.Contains("40003200320031003400"))
                    //{
                    //    wCforEngage = data;
                    //    wCforEngage = wCforEngage.Substring(120);
                    //    //setEngagedDung();
                    //    newEngDung();
                    //} 
                    //#endregion
                    //#region VANGUARD QUEST COMPLETED
                    ///*vanguard completed*/
                    //else if (data.Contains("0B0071007500650073007400540065006D0070006C0061007400650049006400"))
                    //{
                    //    wCforVanguardCompleted = data;
                    //    wCforVanguardCompleted = wCforVanguardCompleted.Substring(100);
                    //    setCompletedVanguard();
                    //} 
                    //#endregion
                    //#region LAUREL
                    ///*earned laurel*/
                    //else if (data.Contains("0B00670072006100640065000B00400041006300680069006500760065006D0065006E0074004700720061006400650049006E0066006F003A00"))
                    //{
                    //    wCforEarnedLaurel = data;
                    //    //updateLaurel();
                    //}
                    //#endregion
                    break;
                #endregion

                case "S_VISIT_NEW_SECTION":
                    #region SECTION
                    NewSection(data);
                    break;
                #endregion

                case "S_UPDATE_NPCGUILD":
                    #region CREDITS UPDATE
                    wCforUpdatedCreditsAfterPurchase = data;
                    UpdateVanguardCredits();
                    break;
                #endregion

                #region CCB
                case "S_ABNORMALITY_BEGIN":
                    #region CCB START
                    crystalbindProcessor.ParseNewBuff(data, currentCharId);
                    break;
                #endregion
                case "S_ABNORMALITY_END":
                    #region CCB END
                    crystalbindProcessor.ParseEndingBuff(data, currentCharId);
                    break;
                #endregion
                case "S_CLEAR_ALL_HOLDED_ABNORMALITY":
                    #region CCB HOLD
                    crystalbindProcessor.CancelDeletion();
                    break;
                #endregion
                #endregion

                case "S_LOGIN_ACCOUNT_INFO":
                    accountLoginProcessor.ParseLoginInfo(data);
                    break;

                case "S_ACCOUNT_PACKAGE_LIST":
                    accountLoginProcessor.ParsePackageInfo(data);
                    break;

                case "S_RETURN_TO_LOBBY":
                    TeraLogic.SaveAccounts(true);
                    TeraLogic.SaveCharacters(true);
                    inventoryProcessor.justLoggedIn = true;
                    vanguardWindowProcessor.justLoggedIn = true;
                    break;

                case "S_GUILD_QUEST_LIST":
                    guildQuestListProcessor.ParseGuildListPacket(data);
                    Tera.UI.UpdateLog("Received guild quests list.");
                    break;

                case "S_FINISH_GUILD_QUEST":
                    Tera.UI.UpdateLog("Guild quest completed.");
                    break;
                case "S_START_GUILD_QUEST":
                    guildQuestListProcessor.TakeQuest(data);
                    Tera.UI.UpdateLog("Guild quest accepted.");
                    break;
                case "S_VIEW_WARE_EX":
                    if (bankProcessor.IsOpenAction(data) == true && bankProcessor.GetBankType(data) == 1)
                    {
                        UI.UpdateLog("Received bank data: " + bankProcessor.GetGoldAmount(data).ToString() + " banked gold.");
                        /*
                         * add saving to file with timestamp
                         *
                         *
                         */
                    }
                    break;
                case "S_USER_STATUS":
                    if(combatParser.GetUserId(data) == currentCharId)
                    {
                        combatParser.SetUserStatus(data);
                    }
                    break;
                default:
                    break;
            }

            #region old
            //#region CHARLIST
            //if (opn.GetName(lp.OpCode) == "S_GET_USER_LIST") 
            //{
            //    wCforCcb.Clear();
            //    SetCharList(lp.HexShortText);
            //}
            //#endregion
            //#region  SELECT ON LOGIN
            //if (opn.GetName(lp.OpCode) == "S_LOGIN")
            //{
            //    cbp.Clear();
            //    LoginChar(lp.HexShortText);
            //}
            //#endregion
            //#region VANGUARD_WINDOW
            //if (opn.GetName(lp.OpCode) == "S_AVAILABLE_EVENT_MATCHING_LIST")
            //{
            //    SetVanguardData(lp.HexShortText);
            //}
            //#endregion
            //#region INVENTORY
            //if (opn.GetName(lp.OpCode) == "S_INVEN")
            //{
            //    if (lp.HexShortText[53].ToString() == "1") /*wait next packet*/
            //    {
            //        ip.multiplePackets = true;
            //        ip.p1 = lp.HexShortText;
            //    }
            //    else if (lp.HexShortText[53].ToString() == "0")/*is last/unique packet*/
            //    {
            //        if (ip.multiplePackets)
            //        {
            //            ip.p2 = lp.HexShortText;
            //            ip.multiplePackets = false;
            //        }
            //        else
            //        {
            //            ip.p1 = lp.HexShortText;
            //        }

            //        SetTokens();
            //    }
            //}

            //#endregion
            //#region DUNGEONS
            //if (opn.GetName(lp.OpCode) == "S_DUNGEON_COOL_TIME_LIST")
            //{
            //    //Console.WriteLine("Received Dungeons");
            //    Tera.UI.win.updateLog(currentCharName + " > received dungeons data.");

            //    wCforDungeons = lp.HexShortText;
            //    setDungs();
            //}
            //#endregion
            //#region DUNGEON_ENGAGED, VANGUARD_COMPLETED, LAUREL_EARNED
            //if (opn.GetName(lp.OpCode) == "S_SYSTEM_MESSAGE")
            //{   
            //    /*dungeon engaged*/
            //    if(lp.HexShortText.Contains("0B00440075006E00670065006F006E00"))
            //    {
            //        wCforEngage = lp.HexShortText;
            //        wCforEngage = wCforEngage.Substring(120);
            //        setEngagedDung();

            //    }
            //    /*vanguard completed*/
            //    else if (lp.HexShortText.Contains("0B0071007500650073007400540065006D0070006C0061007400650049006400"))
            //    {
            //        wCforVanguardCompleted = lp.HexShortText;
            //        wCforVanguardCompleted = wCforVanguardCompleted.Substring(100);
            //        setCompletedVanguard();
            //    }
            //    /*earned laurel*/
            //    else if (lp.HexShortText.Contains("0B00670072006100640065000B00400041006300680069006500760065006D0065006E0074004700720061006400650049006E0066006F003A00"))
            //    {
            //       wCforEarnedLaurel = lp.HexShortText;
            //       updateLaurel();
            //    }


            //}
            //#endregion
            //#region NEW_SECTION
            //if(opn.GetName(lp.OpCode) == "S_VISIT_NEW_SECTION")
            //{
            //    NewSection(lp.HexShortText);
            //}
            //#endregion
            //#region VANGUARD_CREDITS
            //if (opn.GetName(lp.OpCode) == "S_UPDATE_NPCGUILD")
            //{
            //    wCforUpdatedCreditsAfterPurchase = lp.HexShortText;
            //    updateCreditsAfterPurchase();
            //}
            //#endregion
            //#region CRYSTALBIND
            //if (opn.GetName(lp.OpCode) == "S_ABNORMALITY_BEGIN")
            //{
            //    cbp.ParseNewBuff(lp.HexShortText, currentCharId);
            //}
            //if (opn.GetName(lp.OpCode) == "S_ABNORMALITY_END")
            //{
            //    cbp.ParseEndingBuff(lp.HexShortText, currentCharId);
            //}
            //if(opn.GetName(lp.OpCode) == "S_CLEAR_ALL_HOLDED_ABNORMALITY")
            //{
            //    cbp.CancelDeletion();
            //}
            //
            #endregion
            #region CCB_OLD

            //if (opn.GetName(lp.OpCode) == "*")  //old
            //{

            //    if (lp.HexShortText.Contains(currentCharId))
            //    {
            //        bool found = false;
            //        foreach (var s in wCforCcb)
            //        {
            //            if (s.Substring(40, 8).Equals(lp.HexShortText.Substring(40, 8)))
            //            {
            //                if (s.Substring(40, 8).Equals("02120000"))
            //                {
            //                    var i = wCforCcb.IndexOf(s);
            //                    wCforCcb[i] = lp.HexShortText;
            //                }
            //                found = true;
            //                break;
            //            }
            //        }
            //        if (!found)
            //        {
            //            wCforCcb.Add(lp.HexShortText);
            //        }



            //        if(wCforCcb.Count == ccbArraySize)
            //        {
            //            bool ccbFound = false;

            //            foreach (var s in wCforCcb)
            //            {
            //                if (s.Substring(40, 8).Equals("02120000"))
            //                {
            //                    ccb = true;
            //                    ccbFound = true;
            //                    ccbString = s;
            //                    setCcb();
            //                    break;
            //                }
            //            }
            //            if (!ccbFound)
            //            {
            //                ccb = false;
            //            }
            //        }

            //        ccbArraySize = wCforCcb.Count;


            //    }
            //}

            //if(opn.GetName(lp.OpCode) == "*")
            //{
            //    if(lp.HexShortText.Substring(8,12) == currentCharId)
            //    {
            //        wCforEndingAbnormality = lp.HexShortText;
            //        if (wCforEndingAbnormality.Substring(24,8) == "02120000")
            //        {
            //            Console.WriteLine("Received abnormality end, starting thread");
            //            ccbEnd();
            //        }
            //    }
            //}

            //if(opn.GetName(lp.OpCode) == "*")
            //{
            //    ccbEndToFalse();
            //}
            #endregion
        }

        #region Methods
        public static Tera.Character CurrentChar()
        {
            return TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(c => c.Name == currentCharName))];
        }
        private static void SetCharList(string p)
        {
            var charList = charListProcessor.ParseCharacters(p);
            for (int i = 0; i < charList.Count; i++)
            {
                Tera.UI.MainWin.Dispatcher.Invoke(new Action(() => Tera.TeraLogic.AddCharacter(charList[i])));
            }
            charListProcessor.Clear();

            Tera.UI.UpdateLog("Found " + charList.Count + " characters.");
            //TCTNotifier.NotificationProvider.NS.sendNotification("Found " + charList.Count + " characters.");
        }
        private static void LoginChar(string p)
        {
            currentCharName = charLoginProcessor.getName(p);
            currentCharId = charLoginProcessor.getId(p);

            Tera.UI.UpdateLog(currentCharName + " logged in.");
            Tera.UI.MainWin.Dispatcher.Invoke(new Action(() => Tera.TeraLogic.SelectCharacter(currentCharName)));

            TeraLogic.cvcp.SelectedChar.LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            CurrentChar().GoldfingerTokens = 0;
            CurrentChar().MarksOfValor = 0;
            //TCTNotifier.NotificationProvider.NS.sendNotification(currentCharName + " logged in.");
        }
        private static void SetTokens(bool forceLog)
        {
            /*
            inventoryProcessor.MergeInventory();
            var tokens = inventoryProcessor.GetTokensAmounts(inventoryProcessor.inv);

            var newMarks =         tokens[0]; //inventoryProcessor.GetMarksFast(inventoryProcessor.inv);
            var newGoldfinger =    tokens[1]; //inventoryProcessor.GetGoldfingerFast(inventoryProcessor.inv);
            var newDragonScales =  tokens[2]; //inventoryProcessor.GetDragonwingScaleFast(inventoryProcessor.inv);
            */

            inventoryProcessor.FastMergeInventory();
            var newMarks = inventoryProcessor.GetTokenAmountFast(InventoryProcessor.MARK_ID);
            var newGoldfinger = inventoryProcessor.GetTokenAmountFast(InventoryProcessor.GFIN_ID);
            var newDragonScales = inventoryProcessor.GetTokenAmountFast(InventoryProcessor.SCALE_ID);

            bool marks = false;
            bool gft = false;
            bool scales = false;

            if (CurrentChar().MarksOfValor != newMarks)
            {
                marks = true;
                CurrentChar().MarksOfValor = newMarks;
                if (CurrentChar().MarksOfValor > 82)
                {
                    UI.UpdateLog("You've almost reached the maximum amount of Elleon's Marks of Valor.");
                    TCTNotifier.NotificationProvider.SendNotification("Your Elleon's Marks of Valor amount is close to the maximum (" + CurrentChar().MarksOfValor + ").", NotificationType.Marks, Colors.Orange, true, true);
                }
            }

            if (CurrentChar().GoldfingerTokens != newGoldfinger)
            {
                gft = true;
                CurrentChar().GoldfingerTokens = newGoldfinger;
                if (CurrentChar().GoldfingerTokens >= 80)
                {
                    Tera.UI.UpdateLog("You have " + newGoldfinger + " Goldfinger Tokens.");
                    TCTNotifier.NotificationProvider.SendNotification("You have " + CurrentChar().GoldfingerTokens + " Goldfinger Tokens. You can buy a Laundry Box.", NotificationType.Goldfinger, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true, true);
                }
            }
            if (CurrentChar().DragonwingScales != newDragonScales)
            {
                scales = true;
                CurrentChar().DragonwingScales = newDragonScales;
                if (CurrentChar().DragonwingScales >= 50)
                {
                    Tera.UI.UpdateLog("You have " + newDragonScales + " Dragonwing Scales.");
                    TCTNotifier.NotificationProvider.SendNotification("You have " + CurrentChar().DragonwingScales + " Dragonwing Scales. You can buy a Dragon Egg.", NotificationType.Default, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true, true);
                }
            }
            inventoryProcessor.Clear();

            if(marks || gft || scales || forceLog)
            {
                Tera.UI.UpdateLog(currentCharName + " > inventory data updated (" + CurrentChar().MarksOfValor + " Elleon's Marks of Valor, " + CurrentChar().GoldfingerTokens + " Goldfinger Tokens, "+CurrentChar().DragonwingScales+" Dragonwing Scales).");
            }

            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private static void SetVanguardData(string p, bool forceLog)
        {
            int weekly = vanguardWindowProcessor.getWeekly(p); //Convert.ToInt32(wCforVGData[7 * 8].ToString() + wCforVGData[7 * 8 + 1].ToString(), 16);
            int credits = vanguardWindowProcessor.getCredits(p); //Convert.ToInt32(wCforVGData[8 * 8 + 2].ToString() + wCforVGData[8 * 8 + 3].ToString()+ wCforVGData[8 * 8 + 0].ToString() + wCforVGData[8 * 8 + 1].ToString() , 16);
            int completed_dailies = vanguardWindowProcessor.getDaily(p); //Convert.ToInt32(wCforVGData.Substring(3 * 8, 2).ToString(), 16);
            int remaining_dailies = Tera.TeraLogic.MAX_DAILY - completed_dailies;

            if(CurrentChar().Weekly != weekly ||
                CurrentChar().Credits != credits ||
                CurrentChar().Dailies != remaining_dailies ||
                forceLog)
                {

                    UI.UpdateLog(CurrentChar().Name + " > vanguard data updated (" + credits + " credits, " + weekly + " weekly quests done, " + remaining_dailies + " dailies left).");
                }

                CurrentChar().Weekly = weekly;
                CurrentChar().Credits = credits;
                CurrentChar().Dailies = remaining_dailies;
            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }
        private static void NewSection(string p)
        {
            if (CurrentChar().LocationId != sectionProcessor.GetLocationId(p))
            {
                CurrentChar().LocationId = sectionProcessor.GetLocationId(p);
                if (TeraLogic.TCTProps.CcbNM == CcbNotificationMode.TeleportOnly)
                {

                    crystalbindProcessor.CheckCcb(sectionProcessor.GetLocationId(p), sectionProcessor.GetLocationNameId(p));
                }

                Tera.UI.UpdateLog(CurrentChar().Name + " moved to " + sectionProcessor.GetLocationName(p) + ".");
                guildQuestListProcessor.CheckQuestStatus(CurrentChar().LocationId);
            }

            if (TeraLogic.TCTProps.CcbNM == CcbNotificationMode.EverySection)
            {
                crystalbindProcessor.CheckCcb(sectionProcessor.GetLocationId(p), sectionProcessor.GetLocationNameId(p));

            }

            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private static void SetLogo()
        {
            if (mess != null)
            {
                Bitmap logo = mess.GuildLogo;
                uint guildId = mess.GuildId;

                try
                {
                    logo.Save(Environment.CurrentDirectory + "\\content/data/guild_images/" + guildId.ToString() + ".bmp");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        #endregion

        private static void updateLaurel()
        {
            //string tmp = wCforEarnedLaurel;
            //var _chName = tmp.Substring(56, tmp.IndexOf("0B00670072006100640065000B00400041006300680069006500760065006D0065006E0074004700720061006400650049006E0066006F003A00"));
            //var chName = StringUtils.GetStringFromHex(_chName, 0,"0B00");

            //if (currentCharName == chName)
            //{
            //    int laurId = Convert.ToInt32(tmp.Substring(200,2)) - 30;
            //    TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)))].Laurel = ((Laurel)laurId).ToString();
            //    Tera.UI.UpdateLog(currentCharName + " earned a " + ((Laurel)laurId).ToString() + " laurel.");

            //}

        }
        private static void UpdateVanguardCredits()
        {
            string _repId = wCforUpdatedCreditsAfterPurchase.Substring(40, 4);
            var repId = StringUtils.Hex2BStringToInt(_repId);
            if (repId == VANGUARD_REP_ID)
            {
                string _cr = wCforUpdatedCreditsAfterPurchase.Substring(64, 8);
                var cr = StringUtils.Hex4BStringToInt(_cr);
                if(CurrentChar().Credits != cr)
                {
                    var diff = cr - CurrentChar().Credits;

                    CurrentChar().Credits = cr;

                    if(diff > 0) //earned
                    {
                        UI.UpdateLog(currentCharName + " > " + "gained " + diff + " Vanguard credits. Current amount: " + cr + ".");
                        if(CurrentChar().Credits < 8500)
                        {
                            TCTNotifier.NotificationProvider.SendNotification(CurrentChar().Name + " gained "+diff+" Vanguard Credits."+"\n"+"Current amount: " + CurrentChar().Credits + ".", NotificationType.Credits, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true, false);
                        }
                        else
                        {
                            TCTNotifier.NotificationProvider.SendNotification(CurrentChar().Name + " gained " + diff + " Vanguard Credits." + "\n" + "Current amount: " + CurrentChar().Credits + ", you've almost reached your maximum credits.", NotificationType.Credits, Colors.Orange, true, true);
                        }
                    }
                    else //spent
                    {
                        diff = -diff;
                        UI.UpdateLog(currentCharName + " > " + "spent " + diff + " Vanguard credits. Current amount: " + cr + ".");
                        TCTNotifier.NotificationProvider.SendNotification(CurrentChar().Name + " spent " + diff + " Vanguard Credits."+"\n"+"Current amount: " + CurrentChar().Credits + ".", NotificationType.Credits, Colors.Red, true, false);
                    }
                }
            }
        }
        private static void setCompletedVanguard()
        {
            //if (wCforVanguardCompleted.Substring(wCforVanguardCompleted.Length - 8 ,2) =="32") {
            //    StringBuilder sb0 = new StringBuilder();
            //    for (int i = 0; i < 24; i = i + 2)
            //    {
            //        sb0.Append(wCforVanguardCompleted[i]);
            //        sb0.Append(wCforVanguardCompleted[i + 1]);
            //    }
            //    sb0.Replace("00", "");

            //    var questIdAsByteArray = StringUtils.StringToByteArray(sb0.ToString());
            //    var questIdAsString = Encoding.UTF7.GetString(questIdAsByteArray);

            //    var groupId = questIdAsString.Substring(0, 4);
            //    var questId = questIdAsString.Substring(4);
            //    if(questId.Length > 1 && questId[0] == '0') { questId = questId[1].ToString(); }
            //    var query = from Quest in TeraLogic.DailyPlayGuideQuest.Descendants("Quest")
            //                where Quest.Attribute("id").Value.Equals(questId) &&
            //                      Quest.Attribute("groupId").Value.Equals(groupId)
            //                select Quest;

            //    if(query.Count() >= 1)
            //    {
            //        var nameId = query.First().Attribute("name").Value.Substring(21);
            //        var correctedQuestId = questId;
            //        if (correctedQuestId.Length < 2)
            //        {
            //            correctedQuestId = 0 + correctedQuestId;
            //        }
            //        XElement s = Tera.TeraLogic.EventMatching.Descendants().Where(x => (string)x.Attribute("questId") == groupId + correctedQuestId).FirstOrDefault();
            //        var d = s.Descendants().Where(x => (string)x.Attribute("type") == "reputationPoint").FirstOrDefault();

            //        if (d != null)
            //        {

            //            int addedCredits = 0;
            //            Int32.TryParse(d.Attribute("amount").Value, out addedCredits);
            //            addedCredits = addedCredits * 2;

            //            Tera.TeraLogic.CharList.Find(ch => ch.Name.Equals(currentCharName)).Credits += addedCredits;



            //            XElement t = TeraLogic.StrSheet_DailyPlayGuideQuest.Descendants().Where(x => (string)x.Attribute("id") == nameId).FirstOrDefault();
            //            if (t != null)
            //            {
            //                var questname = t.Attribute("string").Value;
            //                Tera.UI.UpdateLog(currentCharName + " > earned " + addedCredits.ToString() + " Vanguard credits for completing \"" + questname + "\".");
            //                TCTNotifier.NotificationProvider.SendNotification("Earned " + addedCredits.ToString() + " Vanguard credits for completing " + questname + ".", NotificationType.Credits, System.Windows.Media.Color.FromArgb(255, 0, 255, 100),true);
            //            }

            //            else
            //            {
            //                Tera.UI.UpdateLog(currentCharName + " > earned " + addedCredits.ToString() + " Vanguard credits for completing a quest. (ID: " + nameId + ")");
            //                TCTNotifier.NotificationProvider.SendNotification("Earned " + addedCredits.ToString() + " Vanguard credits for completing a quest. (ID: " + nameId + ")", NotificationType.Credits, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true);
            //            }
            //        }
            //    }
            //}            
        }
        //private static void setEngagedDung()
        //{
        //    try
        //    {
        //        StringBuilder sb0 = new StringBuilder();
        //        for (int i = 0; i < wCforEngage.Length; i = i + 2)
        //        {
        //            sb0.Append(wCforEngage[i]);
        //            sb0.Append(wCforEngage[i + 1]);
        //        }
        //        sb0.Replace("00", "");
        //        var decIndexAsByteArray = TCTSniffer.StringUtils.StringToByteArray(sb0.ToString());
        //        var decIndexAsString = Encoding.UTF7.GetString(decIndexAsByteArray);
        //        int decIndex = 0;
        //        Int32.TryParse(decIndexAsString, out decIndex);

        //        //string hexIndex = decIndex.ToString("X");
        //        //StringBuilder sb = new StringBuilder();
        //        //sb.Append(hexIndex[2]);
        //        //sb.Append(hexIndex[3]);
        //        //sb.Append(hexIndex[0]);
        //        //sb.Append(hexIndex[1]);
        //        if (TeraLogic.DungList.Find(x => x.Id == decIndex) != null)
        //        {
        //            Tera.UI.UpdateLog(currentCharName + " > " + Tera.TeraLogic.DungList.Find(x => x.Hex.Equals(sb.ToString())).FullName + " engaged.");
        //            TCTNotifier.NotificationProvider.NS.sendNotification(TeraLogic.DungList.Find(x => x.Hex.Equals(sb.ToString())).FullName + " engaged.");

        //            Tera.TeraLogic.CharList.Find(
        //                x => x.Name.Equals(currentCharName)
        //                ).Dungeons.Find(
        //                d => d.Name.Equals(TeraLogic.DungList.Find(
        //                    dg => dg.Id ==decIndex))
        //                    ).ShortName
        //                )).Runs--;
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        Console.WriteLine(e.ToString());
        //    }
        //}
        private static void newEngDung()
        {
            //string stringId = StringUtils.GetStringFromHex(wCforEngage, 0, "0000");
            //int id = 0;
            //Int32.TryParse(stringId, out id);

            //XElement dgNameEl = TeraLogic.StrSheet_Dungeon.Descendants().Where(x => (string)x.Attribute("id") == id.ToString()).FirstOrDefault();

            //if(dgNameEl != null)
            //{
            //    string dgName = dgNameEl.Attribute("string").Value;
            //    Tera.UI.UpdateLog(currentCharName + " > " + dgName + " engaged.");
            //    TCTNotifier.NotificationProvider.SendNotification(dgName + " engaged.");
            //    try
            //    {
            //        CurrentChar().Dungeons.Find(d => d.Name.Equals(TeraLogic.DungList.Find(dg => dg.Id == id).ShortName)).Runs--;
            //    }
            //    catch
            //    {
            //        Tera.UI.UpdateLog("Dungeon not found. Can't subtract run from entry counter.");
            //    }
            //}
        }
        private static void setDungs()
        {
            var temp = wCforDungeons.Substring(24);
            List<string> dgList = new List<string>();
            for (int i = 0; i < temp.Length / 36; i++)
            {
                dgList.Add(temp.Substring(36 * i, 36));
            }
            foreach (var ds in dgList)
            {
                int dgId = StringUtils.Hex2BStringToInt(ds.Substring(8, 4));

                if (Tera.TeraLogic.DungList.Find(d => d.Id == dgId) != null)
                {
                    var dgName = Tera.TeraLogic.DungList.Find(d => d.Id == dgId).ShortName;
                    CurrentChar().Dungeons.Find(d => d.Name == dgName).Runs = Convert.ToInt32(ds.Substring(32, 2));
                }
            }




            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        class CharListProcessor
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
            Character stringToCharacter(string s)
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
                    var c = stringToCharacter(str);
                    _charList.Add(c);

                    if (!Tera.TeraLogic.GuildDictionary.ContainsKey(c.GuildId))
                    {
                        TeraLogic.GuildDictionary.Add(c.GuildId, getGuildName(str));
                    }

                    //Tera.UI.UpdateLog("Found character: " + c.Name + " lv." + c.Level + " " + c.CharClass.ToLower() + ", logged out in " + lcc.Convert(c.LocationId, null, null, null) + " on " + timeConverter.Convert(c.LastOnline, null, null, null) + ".");
                }


                var charList = sortChars(_charList);
                return charList;
            }
            public void Clear()
            {
                indexesArray.Clear();
                charStrings.Clear();
            }
        }
        class CharLoginProcessor
        {
            const int NAME_OFFSET_FROM_START = 290 * 2;
            const int ID_OFFSET_FROM_START = 18 * 2;
            const int ID_LENGHT = 12;

            public string getName(string content)
            {
                return StringUtils.GetStringFromHex(content, NAME_OFFSET_FROM_START, "0000");
            }
            public string getId(string content)
            {
                return content.Substring(ID_OFFSET_FROM_START, ID_LENGHT);
            }
        }
        class VanguardWindowProcessor
        {
            const int WEEKLY_OFFSET = 28 * 2;
            const int CREDITS_OFFSET = 32 * 2;
            const int DAILY_OFFSET = 12 * 2;
            public bool justLoggedIn = true;

            public int getWeekly(string content)
            {
                int w = StringUtils.Hex4BStringToInt(content.Substring(WEEKLY_OFFSET));
                if (w > TeraLogic.MAX_WEEKLY) { w = TeraLogic.MAX_WEEKLY; }
                return w;
            }
            public int getCredits(string content)
            {
                return StringUtils.Hex4BStringToInt(content.Substring(CREDITS_OFFSET));
            }
            public int getDaily(string content)
            {
                return StringUtils.Hex4BStringToInt(content.Substring(DAILY_OFFSET));
            }
        }
        class InventoryProcessor
        {
            const int FIRST_POINTER = 6;
            const int ID_OFFSET = 8 * 2;
            const int POSITION_OFFSET = 32 * 2;
            const int AMOUNT_OFFSET = 36 * 2;
            const int AMOUNT_OFFSET_FROM_ID = 56;
            const int MULTIPLE_FLAG = 25 * 2;
            const int HEADER_LENGHT = 61 * 2;
            const int ILVL_OFFSET = 35 * 2;
            public const string MARK_ID = "5B500200";
            public const string GFIN_ID = "36020000";
            public const string SCALE_ID = "A2B10000";

            List<string> itemStrings = new List<string>();
            List<int> indexesArray = new List<int>();
            List<InventoryItem> itemsList = new List<InventoryItem>();

            public bool multiplePackets = false;
            public bool justLoggedIn = true;
            public string p1;
            public string p2;
            public string inv;

            public void Clear()
            {
                multiplePackets = false;
                p1 = null;
                p2 = null;
                itemsList.Clear();
                itemStrings.Clear();
                indexesArray.Clear();
            }
            public void MergeInventory()
            {
                if (p2 != null)
                {
                    fillItemList(p1);
                    indexesArray.Clear();
                    itemStrings.Clear();
                    fillItemList(p2);
                }
                else
                {
                    fillItemList(p1);
                }
            }
            public void FastMergeInventory()
            {
                if (p2 != null)
                {
                    inv = p1 + p2;
                    fillItemStrings(p1);
                    indexesArray.Clear();
                    fillItemStrings(p2);
                }
                else
                {
                    inv = p1;
                    fillItemStrings(p1);
                }
            }

            public int GetTokenAmountFast(string id)
            {
                int amount = 0;
                foreach (string item in itemStrings)
                {
                    if (item.Substring(16,8) == id)
                    {
                        amount = GetItemFastById(item, id);
                    }
                }
                return amount;
            }

            int GetMarks(string content)
            {
                fillItemList(content);
                if (itemsList.Find(x => x.Name == "Elleon's Mark of Valor") != null)
                {
                    return itemsList.Find(x => x.Name == "Elleon's Mark of Valor").Amount;
                }
                else return 0;


            }
            int GetGoldfinger(string content)
            {
                fillItemList(content);
                if (itemsList.Find(x => x.Name == "Goldfinger Token") != null)
                {
                    return itemsList.Find(x => x.Name == "Goldfinger Token").Amount;
                }
                else return 0;

            }
            int GetMarksFast(string content)
            {
                return GetItemFastById(content, MARK_ID);
            }
            int GetGoldfingerFast(string content)
            {
                return GetItemFastById(content, GFIN_ID);
            }
            int GetDragonwingScaleFast(string content)
            {
                return GetItemFastById(content, SCALE_ID);
            }
            int GetItemFastById(string content, string id)
            {
                if (content.Contains(id))
                {
                    return StringUtils.Hex4BStringToInt(content.Substring(content.IndexOf(id) + AMOUNT_OFFSET_FROM_ID, 8));
                }
                else return 0;

            }
            public int[] GetTokensAmounts(string content)
            {
                int marks = 0;
                int gft = 0;
                int dragonwing = 0;


                if (itemsList.Find(x => x.Name == "Goldfinger Token") != null)
                {
                    gft = itemsList.Find(x => x.Name == "Goldfinger Token").Amount;
                }

                if (itemsList.Find(x => x.Name == "Elleon's Mark of Valor") != null)
                {
                    marks= itemsList.Find(x => x.Name == "Elleon's Mark of Valor").Amount;
                }

                if (itemsList.Find(x => x.Name == "Dragonwing Scale") != null)
                {
                    dragonwing = itemsList.Find(x => x.Name == "Dragonwing Scale").Amount;
                }

                int[] amount = { marks, gft, dragonwing };
                return amount;
            }

            public int GetItemLevel(string content)
            {
                return StringUtils.Hex2BStringToInt(content.Substring(ILVL_OFFSET, 4));
            }
            void fillIndexesArray(string content)
            {
                int currentPointer = FIRST_POINTER;

                do
                {
                    if(currentPointer < content.Length)
                    {
                        int lastPointer = readPointer(content, currentPointer * 2);
                        indexesArray.Add(lastPointer);
                        currentPointer = readPointer(content, lastPointer * 2 + 4);
                    }
                    else
                    {
                        currentPointer = 0;
                    }
                }

                while (currentPointer != 0);
            }
            void fillItemStrings(string p)
            {
                fillIndexesArray(p);
                int itemLenght = 0;
                for (int i = 0; i < indexesArray.Count; i++)
                {
                    if (i != indexesArray.Count - 1)
                    {
                        itemLenght = indexesArray[i + 1] - indexesArray[i];
                        itemStrings.Add(p.Substring(indexesArray[i] * 2 + 4, itemLenght * 2));
                    }
                    else
                    {
                        itemStrings.Add(p.Substring(indexesArray[i] * 2 + 4));
                    }
                }
            }
            InventoryItem stringToItem(string s)
            {
                int itemId = StringUtils.Hex4BStringToInt(s.Substring(ID_OFFSET, 8));
                int amount = StringUtils.Hex4BStringToInt(s.Substring(AMOUNT_OFFSET, 8));
                string name = "Unknown";
                XElement e;
                foreach (var doc in TeraLogic.StrSheet_Item_List)
                {
                    e = doc.Descendants().Where(x => (string)x.Attribute("id") == itemId.ToString()).FirstOrDefault();
                    if (e != null)
                    {
                        name = e.Attribute("string").Value;
                        break;
                    }
                }


                return new InventoryItem(itemId, amount, name);
            }
            void fillItemList(string content)
            {
                fillItemStrings(content);
                foreach (var str in itemStrings)
                {
                    itemsList.Add(stringToItem(str));
                }
            }
            int readPointer(string content, int start)
            {

                return StringUtils.Hex2BStringToInt(content.Substring(start, 4));         
            }
            class InventoryItem
            {
                public int Id { get; set; }
                public int Amount { get; set; }
                public string Name { get; set; }
                public InventoryItem(int _itemId, int _amount, string _name)
                {
                    Id = _itemId;
                    Amount = _amount;
                    Name = _name;
                }

                public override string ToString()
                {
                    return Name + " id:" + Id + " (" + Amount + ")";
                }
            }

        }
        class SectionProcessor
        {
            const int SECTION_ID_OFFSET = 13 * 2;

            public uint GetLocationNameId(string p)
            {
                uint id = GetLocationId(p); /*Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));*/
                XElement s = Tera.TeraLogic.NewWorldMapData.Descendants("Section").Where(x => (string)x.Attribute("id") == id.ToString()).FirstOrDefault();
                if (s != null)
                {
                    id = Convert.ToUInt32(s.Attribute("nameId").Value);
                }
                return id;
            }
            public uint GetLocationId(string p)
            {
                return Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));
            }
            public string GetLocationName(string p)
            {
                var locId = GetLocationId(p);
                var c = new Location_IdToName();
                return (string)c.Convert(locId, null, null, null);
            }
        }
        class CrystalbindProcessor
        {
            const int CCB_ID = 4610;
            const int CHAR_ID_OFFSET = 4 * 2;
            const int CHAR_ID_LENGHT = 12;
            const int B_BUFF_ID_OFFSET = 20 * 2;
            const int E_BUFF_ID_OFFSET = 12 * 2;
            const int TIME_OFFSET = 24 * 2;
            bool ccbEnding = false;
            public bool Status { get; set; } = false;
            public long Time { get; set; } = 0;
            List<Buff> BuffList = new List<Buff>();

            public void Clear()
            {
                Status = false;
                Time = 0;
                BuffList.Clear();
                ccbEnding = false;
            }
            public void CheckCcb(uint locId, uint locNameId)
            {
                bool found = false;
                foreach (var buff in BuffList)
                {
                    if (buff.Id == CCB_ID)
                    {
                        found = true;
                        Status = true;
                        break;
                    }
                }

                if (!found)
                {
                    Status = false;
                }

                if (!Status)
                {
                    if (TeraLogic.DungList.Find(x => x.Id == locId) != null)
                    {
                        if (TeraLogic.DungList.Find(x => x.Id == locId).Tier >= DungeonTier.Tier3)
                        {
                            TCTNotifier.NotificationProvider.SendNotification("Your Complete Crystalbind is off.", NotificationType.Crystalbind, Colors.Red, true, true);
                        }
                    }
                    else if (TeraLogic.DungList.Find(x => x.Id == locNameId) != null)
                    {
                        if (TeraLogic.DungList.Find(x => x.Id == locNameId).Tier >= DungeonTier.Tier3)
                        {
                            TCTNotifier.NotificationProvider.SendNotification("Your Complete Crystalbind is off.", NotificationType.Crystalbind, Colors.Red, true, true);
                        }
                    }
                }

                else if (Time <= 1800000 && Time > 0)
                {
                    if (TeraLogic.DungList.Find(x => x.Id == locId) != null)
                    {
                        if (TeraLogic.DungList.Find(x => x.Id == locId).Tier >= DungeonTier.Tier3)
                        {
                            TCTNotifier.NotificationProvider.SendNotification("Your Complete Crystalbind will expire soon.", NotificationType.Crystalbind, Colors.Orange, true, true);
                        }
                    }
                    else if (TeraLogic.DungList.Find(x => x.Id == locNameId) != null)
                    {
                        if (TeraLogic.DungList.Find(x => x.Id == locNameId).Tier >= DungeonTier.Tier3)
                        {
                            TCTNotifier.NotificationProvider.SendNotification("Your Complete Crystalbind will expire soon.", NotificationType.Crystalbind, Colors.Orange, true, true);
                        }
                    }
                }
            }

            public void ParseNewBuff(string p, string currentCharId)
            {
                if (p.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGHT) == currentCharId)
                {
                    var b = new Buff();
                    b.Id = StringUtils.Hex4BStringToInt(p.Substring(B_BUFF_ID_OFFSET, 8));
                    b.TimeLeft = StringUtils.Hex4BStringToInt(p.Substring(TIME_OFFSET, 8));
                    BuffList.Add(b);

                    if (b.Id == CCB_ID)
                    {
                        Status = true;
                        Time = b.TimeLeft;
                        ccbEnding = false;
                        DataParser.CurrentChar().Crystalbind = Time;
                    }
                }
            }
            public void ParseEndingBuff(string p, string currentCharId)
            {
                if (p.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGHT) == currentCharId)
                {
                    var b = new Buff();
                    b.Id = StringUtils.Hex4BStringToInt(p.Substring(E_BUFF_ID_OFFSET, 8));
                    b.TimeLeft = 0;

                    if (b.Id == CCB_ID)
                    {
                        StartDeletion();
                    }
                    else
                    {
                        BuffList.Remove(BuffList.Find(x => x.Id == b.Id));
                    }
                }
            }
            public void CancelDeletion()
            {
                ccbEnding = false;
            }

            async Task WaitCancel()
            {
                await Task.Delay(2000);
            }
            async void StartDeletion()
            {
                ccbEnding = true;
                await WaitCancel();
                if (ccbEnding)
                {
                    EndCcb();
                }

            }
            void EndCcb()
            {
                Status = false;
                Time = 0;
                CurrentChar().Crystalbind = Time;
                BuffList.Clear();
                TCTNotifier.NotificationProvider.SendNotification("Your Complete Crystalbind expired.", NotificationType.Crystalbind, Colors.Red, true, true);
            }
            class Buff
            {
                public int Id { get; set; }
                public long TimeLeft { get; set; }
            }
        }
        class AccountLoginProcessor
        {
            const string veteran_id = "B2010000";
            const string tc_id = "B1010000";

            public string id = "0";
            public bool tc = false;
            public bool vet = false;
            public long tcTime = 0;

            public void ParsePackageInfo(string p)
            {
                if (p.Contains(veteran_id))
                {
                    vet = true;
                }
                if (p.Contains(tc_id))
                {
                    tc = true;
                    tcTime = StringUtils.Hex4BStringToInt(p.Substring(p.IndexOf(tc_id), 8));
                }
            }
            public void ParseLoginInfo(string p)
            {
                id = p.Substring(8, 12);
            }
        }
        class GuildQuestListProcessor
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
                            TCTNotifier.NotificationProvider.SendNotification("You have available guild quests for this dungeon.", NotificationType.Default, Colors.Red, true,true);
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
        class BankProcessor
        {
            private const int GOLD_OFFSET = 36 * 2;
            private const int BANK_TYPE_OFFSET = 16 * 2;
            private const int ACTION_OFFSET = 20 * 2;

            public long GetGoldAmount(string s)
            {
                return StringUtils.Hex8BStringToInt(s.Substring(GOLD_OFFSET)) / 10000;
            }
            public int GetBankType(string s)
            {
                return StringUtils.Hex1BStringToInt(s.Substring(BANK_TYPE_OFFSET, 2));
            }
            public bool IsOpenAction(string s)
            {
                if (StringUtils.Hex1BStringToInt(s.Substring(ACTION_OFFSET, 2)) == 0)
                {
                    return true;
                }
                else if (StringUtils.Hex1BStringToInt(s.Substring(ACTION_OFFSET, 2)) == 1)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
        class CombatParser
        {
            const int ID_OFFSET = 4 * 2;
            const int ID_LENGHT = 6 * 2;
            const int STATUS_OFFSET = 12 * 2;

            public string GetUserId(string p)
            {
                return p.Substring(ID_OFFSET, ID_LENGHT);
            }

            public void SetUserStatus(string p)
            {
                if (p.Substring(STATUS_OFFSET + 1, 1) == "0")
                     isInCombat = false;            
                else isInCombat = true;
            }
        }
        class SystemMessageProcessor
        {
            const int DUNGEON_ENGAGED_ID = 2229;
            const int VANGUARD_COMPLETED_ID = 2952;

            const int ID_OFFSET = 8 * 2;
            const int DUNGEON_ID_OFFSET = 60 * 2;
            const int QUEST_ID_OFFSET = 50 * 2;
            const int TASK_ID_OFFSET = 68 * 2;
            public void ParseSystemMessage(string p)
            {
                int id = 0;
                string s = "";
                try
                {
                    s = StringUtils.GetStringFromHex(p, ID_OFFSET, "0B00");
                }
                catch (Exception e)
                {
                    s = StringUtils.GetStringFromHex(p, ID_OFFSET, "0000");
                }
                Int32.TryParse(s, out id);

                switch (id)
                {
                    case DUNGEON_ENGAGED_ID:
                        EngageDungeon(p);
                        break;
                //    case VANGUARD_COMPLETED_ID:
                //        if(p.Substring(TASK_ID_OFFSET,2) == "31")
                //        {
                //            CompleteVanguard(p);
                //        }
                //        break;
                }
            }

            private void EngageDungeon(string p)
            {

                uint dungId = 0;
                UInt32.TryParse(StringUtils.GetStringFromHex(p, DUNGEON_ID_OFFSET, "0000"), out dungId);
                XElement t = TeraLogic.StrSheet_Dungeon.Descendants().Where(x => (string)x.Attribute("id") == dungId.ToString()).FirstOrDefault();

                var dgName = t.Attribute("string").Value;
                if (dgName != null)
                {

                    Tera.UI.UpdateLog(currentCharName + " > " + dgName + " engaged.");
                    TCTNotifier.NotificationProvider.SendNotification(dgName + " engaged.");

                    try
                    {
                        CurrentChar().Dungeons.Find(d => d.Name.Equals(TeraLogic.DungList.Find(dg => dg.Id == dungId).ShortName)).Runs--;
                    }
                    catch
                    {
                        Tera.UI.UpdateLog("Dungeon not found. Can't subtract run from entry counter.");
                    }

                }


            }
            private void CompleteVanguard(string p)
            {
                Console.WriteLine("Completed vanguard.");
                int questId = 0;
                Int32.TryParse(StringUtils.GetStringFromHex(p, QUEST_ID_OFFSET, "0B00"), out questId);
                XElement s = Tera.TeraLogic.EventMatching.Descendants().Where(x => (string)x.Attribute("questId") == questId.ToString()).FirstOrDefault();
                var d = s.Descendants().Where(x => (string)x.Attribute("type") == "reputationPoint").FirstOrDefault();

                if (d != null)
                {

                    int addedCredits = 0;
                    Int32.TryParse(d.Attribute("amount").Value, out addedCredits);
                    addedCredits = addedCredits * 2;

                    Tera.TeraLogic.CharList.Find(ch => ch.Name.Equals(currentCharName)).Credits += addedCredits;

                    UI.UpdateLog("Earned " + addedCredits + " Vanguard Initiative credits. Total: " + CurrentChar().Credits + ".");
                    TCTNotifier.NotificationProvider.SendNotification("Earned " + addedCredits + " Vanguard Initiative credits. \nCurrent credits: " + CurrentChar().Credits + ".",NotificationType.Credits, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true, false);
                }
            }

        }
    }
    
}

