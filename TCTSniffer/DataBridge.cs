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
using TCTSniffer;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Tera.Converters;

namespace PacketViewer
{
    enum Class
    {
        Warrior = 0,
        Lancer = 1,
        Slayer = 2,
        Berserker = 3,
        Sorcerer = 4,
        Archer = 5,
        Priest = 6,
        Mystic = 7,
        Reaper = 8,
        Gunner = 9,
        Brawler = 10,
        Ninja = 11,

    }
    enum Laurel
    {
        None,
        Bronze,
        Silver,
        Gold,
        Diamond,
        Champion
    }



    public static class DataBridge
    {
        const int VANGUARD_REP_ID = 609;

        static S_GET_USER_GUILD_LOGO mess;

        private static string wCforDungeons;
        private static string wCforEngage;
        private static string wCforVanguardCompleted;
        private static string wCforUpdatedCreditsAfterPurchase;
        private static string wCforEarnedLaurel;
        private static List<string> wCforCcb = new List<string>();
        private static string currentCharName;
        private static string currentCharId;
        static BasicTeraData btd = new BasicTeraData();
        static OpCodeNamer opn = new OpCodeNamer(Path.Combine(btd.ResourceDirectory, string.Format("opcodes-{0}.txt", "3907eu")));


        static CharListProcessor cp = new CharListProcessor();
        static CharLoginProcessor clp = new CharLoginProcessor();
        static VanguardWindowProcessor vwp = new VanguardWindowProcessor();
        static InventoryProcessor ip = new InventoryProcessor();
        static SectionProcessor sp = new SectionProcessor();
        static CrystalbindProcessor cbp = new CrystalbindProcessor();
        static AccountLoginProcessor alp = new AccountLoginProcessor();

        internal static void storeMessage(Message msg)
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
        public static void storeLastPacket(Network.Packet_old lp)
        {
            switch (opn.GetName(lp.OpCode))
            {
                case "S_GET_USER_LIST": 
                    #region LIST
                    cbp.Clear();
                    if(!TeraLogic.AccountList.Contains(TeraLogic.AccountList.Find(x => x.Id == alp.id)))
                    {
                        TeraLogic.AccountList.Add(new Account(alp.id, alp.tc, alp.vet, alp.tcTime));
                    }
                    cp.CurrentAccountId = alp.id;
                    SetCharList(lp.HexShortText);
                    TeraLogic.SaveAccounts();
                    TeraLogic.SaveCharacters();
                    UI.UpdateLog("Data saved.");

                    break; 
                #endregion

                case "S_LOGIN":
                    #region LOGIN
                    cbp.Clear();
                    LoginChar(lp.HexShortText);
                    break; 
                #endregion

                case "S_AVAILABLE_EVENT_MATCHING_LIST":
                    #region VANGUARD WINDOW
                    SetVanguardData(lp.HexShortText);
                    break; 
                #endregion

                case "S_INVEN":
                    #region INVENTORY
                    if (lp.HexShortText[53].ToString() == "1") /*wait next packet*/
                    {
                        ip.multiplePackets = true;
                        ip.p1 = lp.HexShortText;
                    }
                    else if (lp.HexShortText[53].ToString() == "0")/*is last/unique packet*/
                    {
                        if (ip.multiplePackets)
                        {
                            ip.p2 = lp.HexShortText;
                            ip.multiplePackets = false;
                        }
                        else
                        {
                            ip.p1 = lp.HexShortText;
                        }

                        SetTokens();
                    }
                    break; 
                #endregion

                case "S_DUNGEON_COOL_TIME_LIST":
                    #region DUNGEONS RUNS
                    Tera.UI.UpdateLog(currentCharName + " > received dungeons data.");
                    wCforDungeons = lp.HexShortText;
                    setDungs();
                    break; 
                #endregion

                case "S_SYSTEM_MESSAGE":
                    #region SYSTEM MESSAGE
                    #region DUNGEON ENGAGED
                    /*dungeon engaged*/
                    if (lp.HexShortText.Contains("0B00440075006E00670065006F006E00"))
                    {
                        wCforEngage = lp.HexShortText;
                        wCforEngage = wCforEngage.Substring(120);
                        setEngagedDung();
                    } 
                    #endregion
                    #region VANGUARD QUEST COMPLETED
                    /*vanguard completed*/
                    else if (lp.HexShortText.Contains("0B0071007500650073007400540065006D0070006C0061007400650049006400"))
                    {
                        wCforVanguardCompleted = lp.HexShortText;
                        wCforVanguardCompleted = wCforVanguardCompleted.Substring(100);
                        setCompletedVanguard();
                    } 
                    #endregion
                    #region LAUREL
                    /*earned laurel*/
                    else if (lp.HexShortText.Contains("0B00670072006100640065000B00400041006300680069006500760065006D0065006E0074004700720061006400650049006E0066006F003A00"))
                    {
                        wCforEarnedLaurel = lp.HexShortText;
                        updateLaurel();
                    }
                    break;  
                #endregion
                #endregion

                case "S_VISIT_NEW_SECTION":
                    #region SECTION
                    NewSection(lp.HexShortText);
                    break; 
                #endregion

                case "S_UPDATE_NPCGUILD":
                    #region CREDITS UPDATE
                    wCforUpdatedCreditsAfterPurchase = lp.HexShortText;
                    updateCreditsAfterPurchase(); 
                    break;
                #endregion

                #region CCB
                case "S_ABNORMALITY_BEGIN":
                    #region CCB START
                    cbp.ParseNewBuff(lp.HexShortText, currentCharId);
                    break;
                #endregion
                case "S_ABNORMALITY_END":
                    #region CCB END
                    cbp.ParseEndingBuff(lp.HexShortText, currentCharId);
                    break;
                #endregion
                case "S_CLEAR_ALL_HOLDED_ABNORMALITY":
                    #region CCB HOLD
                    cbp.CancelDeletion();
                    break;
                #endregion
                #endregion

                case "S_LOGIN_ACCOUNT_INFO":
                    alp.ParseLoginInfo(lp.HexShortText);
                    break;

                case "S_ACCOUNT_PACKAGE_LIST":
                    alp.ParsePackageInfo(lp.HexShortText);
                    break;

                case "S_RETURN_TO_LOBBY":
                    TeraLogic.SaveAccounts();
                    TeraLogic.SaveCharacters();
                    UI.UpdateLog("Data saved.");
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
            var charList =  cp.ParseCharacters(p);
            for (int i = 0; i < charList.Count; i++)
            {
                UI.MainWin.Dispatcher.Invoke(new Action(() => Tera.TeraLogic.AddCharacter(charList[i])));
            }
            cp.Clear();

            UI.UpdateLog("Found " + charList.Count + " characters.");
            //TCTNotifier.NotificationProvider.NS.sendNotification("Found " + charList.Count + " characters.");
        }
        private static void LoginChar(string p)
        {
            currentCharName = clp.getName(p);
            currentCharId = clp.getId(p);
            
            UI.UpdateLog(currentCharName + " logged in.");
            UI.MainWin.Dispatcher.Invoke(new Action(()=> Tera.TeraLogic.SelectCharacter(currentCharName)));

            TeraLogic.cvcp.SelectedChar.LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            //TCTNotifier.NotificationProvider.NS.sendNotification(currentCharName + " logged in.");
        }
        private static void SetTokens()
        {
            ip.FastMergeInventory();
            CurrentChar().MarksOfValor = ip.GetMarksFast(ip.inv);
            CurrentChar().GoldfingerTokens = ip.GetGoldfingerFast(ip.inv);
            ip.Clear();

            UI.UpdateLog(currentCharName + " > received inventory data (" + CurrentChar().MarksOfValor + " Elleon's Marks of Valor, " + CurrentChar().GoldfingerTokens + " Goldfinger Tokens).");
            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private static void SetVanguardData(string p)
        {
            int weekly = vwp.getWeekly(p); //Convert.ToInt32(wCforVGData[7 * 8].ToString() + wCforVGData[7 * 8 + 1].ToString(), 16);
            int credits = vwp.getCredits(p); //Convert.ToInt32(wCforVGData[8 * 8 + 2].ToString() + wCforVGData[8 * 8 + 3].ToString()+ wCforVGData[8 * 8 + 0].ToString() + wCforVGData[8 * 8 + 1].ToString() , 16);
            int completed_dailies = vwp.getDaily(p); //Convert.ToInt32(wCforVGData.Substring(3 * 8, 2).ToString(), 16);
            int remaining_dailies = Tera.TeraLogic.MAX_DAILY - completed_dailies;
            UI.UpdateLog(currentCharName + " > received vanguard data (" + credits + " credits, " + weekly + " weekly quests done, "+ remaining_dailies + " dailies left).");

            CurrentChar().Weekly = weekly;
            CurrentChar().Credits = credits;
            CurrentChar().Dailies = remaining_dailies;
            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }
        private static void NewSection(string p)
        {
            if (CurrentChar().LocationId != sp.GetLocationId(p))
            {
                CurrentChar().LocationId = sp.GetLocationId(p);
                if(TeraLogic.TCTProps.CcbNM == CcbNotificationMode.TeleportOnly)
                {
                    cbp.CheckCcb(CurrentChar().LocationId);
                }
                UI.UpdateLog(CurrentChar().Name + " moved to " + sp.GetLocationName(p) + ".");
            }

            if (TeraLogic.TCTProps.CcbNM == CcbNotificationMode.EverySection)
            {
                cbp.CheckCcb(CurrentChar().LocationId);
            }

            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private static void SetLogo()
        {
            if(mess != null)
            {
                Bitmap logo = mess.GuildLogo;
                uint guildId = mess.GuildId;

                try
                {
                    logo.Save(Environment.CurrentDirectory + "\\content/data/guild_images/" + guildId.ToString() + ".bmp");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        #endregion

        private static void updateLaurel()
        {
            string tmp = wCforEarnedLaurel;
            var _chName = tmp.Substring(56, tmp.IndexOf("0B00670072006100640065000B00400041006300680069006500760065006D0065006E0074004700720061006400650049006E0066006F003A00"));
            var chName = StringUtils.GetStringFromHex(_chName, 0,"0B00");

            if (currentCharName == chName)
            {
                int laurId = Convert.ToInt32(tmp.Substring(200,2)) - 30;
                TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)))].Laurel = ((Laurel)laurId).ToString();
                Tera.UI.UpdateLog(currentCharName + " earned a " + ((Laurel)laurId).ToString() + " laurel.");

            }

        }                   
        private static void updateCreditsAfterPurchase()
        {
            string _repId = wCforUpdatedCreditsAfterPurchase.Substring(40, 4);
            var repId = TCTSniffer.StringUtils.Hex2BStringToInt(_repId);
            if(repId == VANGUARD_REP_ID)
            {
                string _cr = wCforUpdatedCreditsAfterPurchase.Substring(64, 8);
                var cr = TCTSniffer.StringUtils.Hex4BStringToInt(_cr);
                Tera.TeraLogic.CharList[Tera.TeraLogic.CharList.IndexOf(Tera.TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)))].Credits = cr;
                Tera.UI.UpdateLog(currentCharName + " > " + cr + " Vanguard credits left.");
            }
        }
        private static void setCompletedVanguard()
        {
            if (wCforVanguardCompleted.Substring(wCforVanguardCompleted.Length - 8 ,2) =="32") {
                StringBuilder sb0 = new StringBuilder();
                for (int i = 0; i < 24; i = i + 2)
                {
                    sb0.Append(wCforVanguardCompleted[i]);
                    sb0.Append(wCforVanguardCompleted[i + 1]);
                }
                sb0.Replace("00", "");

                var questIdAsByteArray = StringUtils.StringToByteArray(sb0.ToString());
                var questIdAsString = Encoding.UTF7.GetString(questIdAsByteArray);

                var groupId = questIdAsString.Substring(0, 4);
                var questId = questIdAsString.Substring(4);
                if(questId.Length > 1 && questId[0] == '0') { questId = questId[1].ToString(); }
                var query = from Quest in TeraLogic.DailyPlayGuideQuest.Descendants("Quest")
                            where Quest.Attribute("id").Value.Equals(questId) &&
                                  Quest.Attribute("groupId").Value.Equals(groupId)
                            select Quest;
                
                if(query.Count() >= 1)
                {
                    var nameId = query.First().Attribute("name").Value.Substring(21);
                    var correctedQuestId = questId;
                    if (correctedQuestId.Length < 2)
                    {
                        correctedQuestId = 0 + correctedQuestId;
                    }
                    XElement s = Tera.TeraLogic.EventMatching.Descendants().Where(x => (string)x.Attribute("questId") == groupId + correctedQuestId).FirstOrDefault();
                    var d = s.Descendants().Where(x => (string)x.Attribute("type") == "reputationPoint").FirstOrDefault();

                    if (d != null)
                    {

                        int addedCredits = 0;
                        Int32.TryParse(d.Attribute("amount").Value, out addedCredits);
                        addedCredits = addedCredits * 2;

                        Tera.TeraLogic.CharList.Find(ch => ch.Name.Equals(currentCharName)).Credits += addedCredits;



                        XElement t = TeraLogic.StrSheet_DailyPlayGuideQuest.Descendants().Where(x => (string)x.Attribute("id") == nameId).FirstOrDefault();
                        if (t != null)
                        {
                            var questname = t.Attribute("string").Value;
                            Tera.UI.UpdateLog(currentCharName + " > earned " + addedCredits.ToString() + " Vanguard credits for completing " + questname + ".");
                            TCTNotifier.NotificationProvider.NS.sendNotification("Earned " + addedCredits.ToString() + " Vanguard credits for completing " + questname + ".", TCTNotifier.NotificationType.Credits, Colors.LightGreen);
                        }

                        else
                        {
                            Tera.UI.UpdateLog(currentCharName + " > earned " + addedCredits.ToString() + " Vanguard credits for completing a quest. (ID: " + nameId + ")");
                            TCTNotifier.NotificationProvider.NS.sendNotification("Earned " + addedCredits.ToString() + " Vanguard credits for completing a quest. (ID: " + nameId + ")", TCTNotifier.NotificationType.Credits, Colors.LightGreen);
                        }
                    }
                }
            }            
        }
        private static void setEngagedDung()
        {
            try
            {
                StringBuilder sb0 = new StringBuilder();
                for (int i = 0; i < wCforEngage.Length; i = i + 2)
                {
                    sb0.Append(wCforEngage[i]);
                    sb0.Append(wCforEngage[i + 1]);
                }
                sb0.Replace("00", "");
                var decIndexAsByteArray = TCTSniffer.StringUtils.StringToByteArray(sb0.ToString());
                var decIndexAsString = Encoding.UTF7.GetString(decIndexAsByteArray);
                int decIndex = 0;
                Int32.TryParse(decIndexAsString, out decIndex);

                string hexIndex = decIndex.ToString("X");
                StringBuilder sb = new StringBuilder();
                sb.Append(hexIndex[2]);
                sb.Append(hexIndex[3]);
                sb.Append(hexIndex[0]);
                sb.Append(hexIndex[1]);
                if (Tera.TeraLogic.DungList.Find(x => x.Hex.Equals(sb.ToString())) != null)
                {
                    Tera.UI.UpdateLog(currentCharName + " > " + Tera.TeraLogic.DungList.Find(x => x.Hex.Equals(sb.ToString())).FullName + " engaged.");
                    TCTNotifier.NotificationProvider.NS.sendNotification(TeraLogic.DungList.Find(x => x.Hex.Equals(sb.ToString())).FullName + " engaged.");

                    Tera.TeraLogic.CharList.Find(
                        x => x.Name.Equals(currentCharName)
                        ).Dungeons.Find(
                        d => d.Name.Equals(Tera.TeraLogic.DungList.Find(
                            dg => dg.Hex.Equals(sb.ToString())
                            ).ShortName
                        )).Runs--;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
        private static void setDungs()
        {
            var temp = wCforDungeons.Substring(24);
            List<string> dgList = new List<string>();
            for (int i = 0; i < temp.Length / 28; i++)
            {
                dgList.Add(temp.Substring(28 * i, 28));
                if(Tera.TeraLogic.DungList.Find(d => d.Hex.Equals(dgList[i].Substring(8, 4))) != null)
                {
                    var chIndex = Tera.TeraLogic.CharList.IndexOf(Tera.TeraLogic.CharList.Find(c => c.Name.Equals(currentCharName)));
                    var dgName = Tera.TeraLogic.DungList.Find(d => d.Hex.Equals(dgList[i].Substring(8, 4))).ShortName;

                    var dgIndex = Tera.TeraLogic.CharList[chIndex].Dungeons.IndexOf(Tera.TeraLogic.CharList[chIndex].Dungeons.Find(d => d.Name.Equals(dgName)));
                    Tera.TeraLogic.CharList[chIndex].Dungeons[dgIndex].Runs = Convert.ToInt32(dgList[i].Substring(25, 1));


                }
            }
            Tera.TeraLogic.CharList[Tera.TeraLogic.CharList.IndexOf(Tera.TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)))].LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

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
                string name = Encoding.UTF7.GetString(TCTSniffer.StringUtils.StringToByteArray(b.ToString()));
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
                string gname = Encoding.UTF7.GetString(TCTSniffer.StringUtils.StringToByteArray(b.ToString()));
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

                    UI.UpdateLog("Found character: " + c.Name + " lv." + c.Level + " " + c.CharClass.ToLower() + ", logged out in " + lcc.Convert(c.LocationId, null, null, null) + " on " + timeConverter.Convert(c.LastOnline, null, null, null) + ".");
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


            public int getWeekly(string content)
            {
                int w = StringUtils.Hex4BStringToInt(content.Substring(WEEKLY_OFFSET));
                if(w > TeraLogic.MAX_WEEKLY) { w = TeraLogic.MAX_WEEKLY; }
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
            const int MULTIPLE_FLAG = 25*2;
            const int HEADER_LENGHT = 61 * 2;
            const string MARK_ID = "5B500200";
            const string GFIN_ID = "36020000";

            List<string> itemStrings = new List<string>();
            List<int> indexesArray = new List<int>();
            List<InventoryItem> itemsList = new List<InventoryItem>();

            public bool multiplePackets = false;
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
                if(p2 != null)
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
                }
                else inv = p1;
            }
            public int GetMarks(string content)
            {
                fillItemList(content);
                if (itemsList.Find(x => x.Name == "Elleon's Mark of Valor") != null)
                {
                    return itemsList.Find(x => x.Name == "Elleon's Mark of Valor").Amount;
                }
                else return 0;


            }
            public int GetGoldfinger(string content)
            {
                fillItemList(content);
                if (itemsList.Find(x => x.Name == "Goldfinger Token") != null)
                {
                    return itemsList.Find(x => x.Name == "Goldfinger Token").Amount;
                }
                else return 0;

            }
            public int GetMarksFast(string content)
            {
                if (content.Contains(MARK_ID))
                {
                        return StringUtils.Hex4BStringToInt(content.Substring(content.IndexOf(MARK_ID) + AMOUNT_OFFSET_FROM_ID, 8));
                }
                else return 0;
            }
            public int GetGoldfingerFast(string content)
            {
                if (content.Contains(GFIN_ID))
                {
                        return StringUtils.Hex4BStringToInt(content.Substring(content.IndexOf(GFIN_ID) + AMOUNT_OFFSET_FROM_ID, 8));

                }
                else return 0;
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
            void fillItemStrings(string p)
            {
                fillIndexesArray(p);
                int itemLenght = 0;
                for (int i = 0; i < indexesArray.Count; i++)
                {
                    if (i != indexesArray.Count-1)
                    {
                        itemLenght = indexesArray[i+1] - indexesArray[i];
                        itemStrings.Add(p.Substring(indexesArray[i] * 2 + 4 , itemLenght*2));
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
                    if(e != null)
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
            }

        }
        class SectionProcessor
        {
            const int SECTION_ID_OFFSET = 13 * 2;
        
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
            public void CheckCcb(uint locId)
            {
                bool found = false;
                foreach (var buff in BuffList)
                {
                    if(buff.Id == CCB_ID)
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
                    if (TeraLogic.RiskList.Contains(locId))
                    {
                        TCTNotifier.NotificationProvider.NS.sendNotification("Your Complete Crystalbind is off.", TCTNotifier.NotificationType.Crystalbind, Colors.Red);
                    }
                }
                else if (Time <= 3600000 && Time > 0)
                {
                    if (TeraLogic.RiskList.Contains(locId))
                    {
                        TCTNotifier.NotificationProvider.NS.sendNotification("Your Complete Crystalbind will expire soon.", TCTNotifier.NotificationType.Crystalbind, Colors.Orange);
                    }
                }
            }

            public void ParseNewBuff(string p, string currentCharId)
            {
                if(p.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGHT) == currentCharId)
                {
                    var b = new Buff();
                    b.Id = StringUtils.Hex4BStringToInt(p.Substring(B_BUFF_ID_OFFSET, 8));
                    b.TimeLeft = StringUtils.Hex4BStringToInt(p.Substring(TIME_OFFSET, 8));
                    BuffList.Add(b);

                    if(b.Id == CCB_ID)
                    {
                        Status = true;
                        Time = b.TimeLeft;
                        ccbEnding = false;
                        DataBridge.CurrentChar().Crystalbind = Time;
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

                    if(b.Id == CCB_ID)
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
                TCTNotifier.NotificationProvider.NS.sendNotification("Your Complete Crystalbind expired.", TCTNotifier.NotificationType.Crystalbind, Colors.Red);
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
                    tcTime = StringUtils.Hex4BStringToInt(p.Substring(p.IndexOf(tc_id),8));
                }
            }
            public void ParseLoginInfo(string p)
            {
                id = p.Substring(8, 12);
            }
        }
        class SystemMessageProcessor { }

    }
}

