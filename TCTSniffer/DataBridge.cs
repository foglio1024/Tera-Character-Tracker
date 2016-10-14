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
        private static string wCforNewRegion;
        private static string wCforUpdatedCreditsAfterPurchase;
        private static string wCforEarnedLaurel;
        private static List<string> wCforCcb = new List<string>();
        private static string wCforEndingAbnormality;
        private static string ccbString;
        private static bool ccb;
        private static int ccbArraySize = 0;
        private static string currentCharName;
        private static string currentCharId;
        private static bool ccbEnding = false;
        static BasicTeraData btd = new BasicTeraData();
        static OpCodeNamer opn = new OpCodeNamer(Path.Combine(btd.ResourceDirectory, string.Format("opcodes-{0}.txt", "3907eu")));


        static CharListProcessor cp = new CharListProcessor();
        static CharLoginProcessor clp = new CharLoginProcessor();
        static VanguardWindowProcessor vwp = new VanguardWindowProcessor();
        static InventoryProcessor ip = new InventoryProcessor();
        static SectionProcessor sp = new SectionProcessor();
        static CrystalbindProcessor cbp = new CrystalbindProcessor();

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

            #region CHARLIST
            if (opn.GetName(lp.OpCode) == "S_GET_USER_LIST") 
            {
                wCforCcb.Clear();
                SetCharList(lp.HexShortText);
            }
            #endregion
            #region  SELECT ON LOGIN
            if (opn.GetName(lp.OpCode) == "S_LOGIN")
            {
                cbp.Clear();
                LoginChar(lp.HexShortText);
            }
            #endregion
            #region VANGUARD_WINDOW
            if (opn.GetName(lp.OpCode) == "S_AVAILABLE_EVENT_MATCHING_LIST")
            {
                SetVanguardData(lp.HexShortText);
            }
            #endregion
            #region INVENTORY
            if (opn.GetName(lp.OpCode) == "S_INVEN")
            {
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
            }

            #endregion
            #region DUNGEONS
            if (opn.GetName(lp.OpCode) == "S_DUNGEON_COOL_TIME_LIST")
            {
                //Console.WriteLine("Received Dungeons");
                Tera.UI.win.updateLog(currentCharName + " > received dungeons data.");

                wCforDungeons = lp.HexShortText;
                setDungs();
            }
            #endregion
            #region DUNGEON_ENGAGED, VANGUARD_COMPLETED, LAUREL_EARNED
            if (opn.GetName(lp.OpCode) == "S_SYSTEM_MESSAGE")
            {   /*dungeon engaged*/
                if(lp.HexShortText.Contains("0B00440075006E00670065006F006E00"))
                {
                    wCforEngage = lp.HexShortText;
                    wCforEngage = wCforEngage.Substring(120);
                    setEngagedDung();

                }
                /*vanguard completed*/
                else if (lp.HexShortText.Contains("0B0071007500650073007400540065006D0070006C0061007400650049006400"))
                {
                    wCforVanguardCompleted = lp.HexShortText;
                    wCforVanguardCompleted = wCforVanguardCompleted.Substring(100);
                    setCompletedVanguard();
                }
                /*earned laurel*/
                else if (lp.HexShortText.Contains("0B00670072006100640065000B00400041006300680069006500760065006D0065006E0074004700720061006400650049006E0066006F003A00"))
                {
                   wCforEarnedLaurel = lp.HexShortText;
                   updateLaurel();
                }


            }
            #endregion
            #region NEW_SECTION
            if(opn.GetName(lp.OpCode) == "S_VISIT_NEW_SECTION")
            {
                NewSection(lp.HexShortText);
            }
            #endregion
            #region VANGUARD_CREDITS
            if (opn.GetName(lp.OpCode) == "S_UPDATE_NPCGUILD")
            {
                wCforUpdatedCreditsAfterPurchase = lp.HexShortText;
                updateCreditsAfterPurchase();
            }
            #endregion
            #region CRYSTALBIND
            if (opn.GetName(lp.OpCode) == "S_ABNORMALITY_BEGIN")
            {
                cbp.ParseNewBuff(lp.HexShortText, currentCharId);
            }
            if (opn.GetName(lp.OpCode) == "S_ABNORMALITY_END")
            {
                cbp.ParseEndingBuff(lp.HexShortText, currentCharId);
            }
            if(opn.GetName(lp.OpCode) == "S_CLEAR_ALL_HOLDED_ABNORMALITY")
            {
                cbp.CancelDeletion();
            }
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
                UI.win.Dispatcher.Invoke(new Action(() => Tera.TeraLogic.newChar(charList[i])));
            }
            cp.Clear();

            UI.win.updateLog("Found " + charList.Count + " characters.");
            TCTNotifier.NotificationProvider.NS.sendNotification("Found " + charList.Count + " characters.");
        }
        private static void LoginChar(string p)
        {
            currentCharName = clp.getName(p);
            currentCharId = clp.getId(p);
            
            Tera.UI.win.updateLog(currentCharName + " logged in.");
            Tera.UI.win.Dispatcher.Invoke(new Action(()=> Tera.TeraLogic.selectChar(currentCharName)));

            TeraMainWindow.cvcp.SelectedChar.LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            TCTNotifier.NotificationProvider.NS.sendNotification(currentCharName + " logged in.");
        }
        private static void SetTokens()
        {
            ip.FastMergeInventory();
            CurrentChar().MarksOfValor = ip.GetMarksFast(ip.inv);
            CurrentChar().GoldfingerTokens = ip.GetGoldfingerFast(ip.inv);
            ip.Clear();

            Tera.UI.win.updateLog(currentCharName + " > received inventory data (" + CurrentChar().MarksOfValor + " Elleon's Marks of Valor, " + CurrentChar().GoldfingerTokens + " Goldfinger Tokens).");
            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private static void SetVanguardData(string p)
        {
            int weekly = vwp.getWeekly(p); //Convert.ToInt32(wCforVGData[7 * 8].ToString() + wCforVGData[7 * 8 + 1].ToString(), 16);
            int credits = vwp.getCredits(p); //Convert.ToInt32(wCforVGData[8 * 8 + 2].ToString() + wCforVGData[8 * 8 + 3].ToString()+ wCforVGData[8 * 8 + 0].ToString() + wCforVGData[8 * 8 + 1].ToString() , 16);
            int completed_dailies = vwp.getDaily(p); //Convert.ToInt32(wCforVGData.Substring(3 * 8, 2).ToString(), 16);
            int remaining_dailies = Tera.TeraLogic.MAX_DAILY - completed_dailies;
            Tera.UI.win.updateLog(currentCharName + " > received vanguard data (" + credits + " credits, " + weekly + " weekly quests done, "+ remaining_dailies + " dailies left).");

            CurrentChar().Weekly = weekly;
            CurrentChar().Credits = credits;
            CurrentChar().Dailies = remaining_dailies;
            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }
        private static void NewSection(string p)
        {
            CurrentChar().LocationId = sp.GetLocationId(p);
            cbp.CheckCcb(CurrentChar().LocationId);

            UI.win.updateLog(CurrentChar().Name + " moved to " + sp.GetLocationName(p) + ".");

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
                Tera.UI.win.updateLog(currentCharName + " earned a " + ((Laurel)laurId).ToString() + " laurel.");

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
                Tera.UI.win.updateLog(currentCharName + " > " + cr + " Vanguard credits left.");
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
                            Tera.UI.win.updateLog(currentCharName + " > earned " + addedCredits.ToString() + " Vanguard credits for completing " + questname + ".");
                            TCTNotifier.NotificationProvider.NS.sendNotification("Earned " + addedCredits.ToString() + " Vanguard credits for completing " + questname + ".", TCTNotifier.NotificationType.Credits, Colors.LightGreen);
                        }

                        else
                        {
                            Tera.UI.win.updateLog(currentCharName + " > earned " + addedCredits.ToString() + " Vanguard credits for completing a quest. (ID: " + nameId + ")");
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
                    Tera.UI.win.updateLog(currentCharName + " > " + Tera.TeraLogic.DungList.Find(x => x.Hex.Equals(sb.ToString())).FullName + " engaged.");
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
            int tc = 1;
            if (Tera.TeraLogic.isTC)
            {
                tc = 2;
            }
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
        #region OldMethods
        //private static void updateRegion()
        //{
        //    int location_start = 18*2;

        //    var chName = StringUtils.GetStringFromHex(wCforNewRegion, 112,"0000");
        //    if(chName == currentCharName)
        //    {
        //        var sb = new StringBuilder();
        //        var sb2 = new StringBuilder();
        //        for (int i = 0; i < 8; i++)
        //        {
        //            sb.Append(wCforNewRegion[location_start + i]);
        //        }

        //        sb2.Append(sb[6].ToString());
        //        sb2.Append(sb[7].ToString());
        //        sb2.Append(sb[4].ToString());
        //        sb2.Append(sb[5].ToString());
        //        sb2.Append(sb[2].ToString());
        //        sb2.Append(sb[3].ToString());
        //        sb2.Append(sb[0].ToString());
        //        sb2.Append(sb[1].ToString());
        //        uint locationId = Convert.ToUInt32(sb2.ToString(), 16);

        //        Tera.TeraLogic.CharList[Tera.TeraLogic.CharList.IndexOf(Tera.TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)))].LocationId = locationId;
        //        string locationName = "unknown region (" + locationId + ")";

        //        var c = new LocationConverter();

        //        locationName = (string)c.Convert(locationId, null, null, null);


        //        Tera.UI.win.updateLog(currentCharName + " moved to " + locationName + ".");
        //        Tera.TeraLogic.CharList[Tera.TeraLogic.CharList.IndexOf(Tera.TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)))].LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        //        if (locationId == 9067 || locationId == 9068 || locationId == 9768 || locationId == 9070 || locationId == 9611 /*|| locationId == 183002*/)
        //        {
        //            if (!ccb)
        //            {
        //                TCTNotifier.NotificationProvider.NS.sendNotification("Your CCB is off.", TCTNotifier.NotificationType.Crystalbind, Colors.Red);
        //            }

                        
        //            else if (TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)).Crystalbind < 3600000)
        //            {
        //                TCTNotifier.NotificationProvider.NS.sendNotification("Your CCB will expire in less than one hour.", TCTNotifier.NotificationType.Crystalbind, Colors.Red);
        //            }
                    
        //        }
        //    }
        //}
        //private static void ccbEndToFalse()
        //{
        //    ccbEnding = false;
        //    Console.WriteLine("ccbEnding set to false");
        //}
        //private static void setCcb()
        //{
        //    var tmp = ccbString;
        //    var timeLeft = StringUtils.Hex4BStringToInt(tmp.Substring(48, 8));
        //    var ts = TimeSpan.FromMilliseconds(timeLeft);
        //    TeraLogic.CharList.Find(x => x.Name.Equals(currentCharName)).Crystalbind = timeLeft;
        //    Tera.UI.win.updateLog(currentCharName + " > " + ts.Hours + " hours " + ts.Minutes + " minutes of Complete Crystalbind left.");
        //}
        //private static void ccbEnd()
        //{
        //    ccbEnding = true;
        //    Console.WriteLine("Wait start");
        //    Thread.Sleep(2000);
        //    Console.WriteLine("Wait done");
        //    if (ccbEnding)
        //    {
        //        Console.WriteLine("ccbEnding = " + ccbEnding.ToString());
        //        TeraLogic.CharList.Find(x => x.Name == currentCharName).Crystalbind = 0;
        //        Console.WriteLine("Sending expired notification");
        //        TCTNotifier.NotificationProvider.NS.sendNotification("Your ccb has expired.", TCTNotifier.NotificationType.Crystalbind, Colors.Red);
        //    }


        //}
        #endregion
    }
    public class CharListProcessor
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

        LastOnlineConverter lc = new LastOnlineConverter();
        LocationConverter lcc = new LocationConverter();

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
                                _lastOnline: lastOn
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

                UI.win.updateLog("Found character: " + c.Name + " lv." + c.Level + " " + c.CharClass.ToLower() + ", logged out in " + lcc.Convert(c.LocationId, null, null, null) + " on " + lc.Convert(c.LastOnline, null, null, null) + ".");
            }


            var charList = sortChars(_charList);
            return charList;
        }
        public void Clear()
        {
            indexesArray.Clear();
            charStrings.Clear();
        }
        List<Tera.Character> getAllChars(string p)
        {
            List<Tera.Character> charList = new List<Character>();
            List<string> stringList = new List<string>();
            int startIndex = 0;
            int charLength = 0;
            string _char = "";
            bool lastChar = false;
            bool eof = false;
            while (!eof)
            {
                if (!lastChar)
                {
                    string header = p.Substring(0, 4);
                    charLength = p.IndexOf(header, 4);
                    _char = p.Substring(0, charLength + 4);
                    stringList.Add(_char);
                    //Console.WriteLine("Added char");

                    //Check if next is last char
                    string nextCharHeader = "";
                    for (int i = 0; i < 4; i++)
                    {
                        nextCharHeader = nextCharHeader + p[charLength + 4 + i];
                    }
                    if (nextCharHeader == "0000")
                    {
                        lastChar = true;
                        //Console.WriteLine("Last char");
                    }

                    p = p.Substring(charLength + 4);
                }
                else
                {
                    _char = p;
                    stringList.Add(_char);
                    //Console.WriteLine("Found {0} chars", stringList.Count);
                    eof = true;
                }
            }
            foreach (string s in stringList)
            {
                uint guildId = getGuildId(s);
                uint pos = getPosition(s);
                string name = getName(s);
                string charClass = getClass(s);
                uint level = getLevel(s);
                uint loc = getLocationId(s);
                long lastOn = getLastOnline(s);
                string laurel = getLaurel(s);


                charList.Add(new Character(_index: pos,
                                           _name: name,
                                           _class: charClass,
                                           _laurel: laurel,
                                           _lvl: level,
                                           _guildId: guildId,
                                           _locationId: loc,
                                           _lastOnline: lastOn
                                           ));
                LastOnlineConverter lc = new LastOnlineConverter();
                Tera.LocationConverter lcc = new LocationConverter();
                Tera.UI.win.updateLog("Found character: " + name + " lv." + level + " " + charClass.ToLower() + ", logged out in " + lcc.Convert(loc, null, null, null) + " on " + lc.Convert(lastOn, null, null, null) + ".");

                if (!Tera.TeraLogic.GuildDictionary.ContainsKey(guildId))
                {
                    TeraLogic.GuildDictionary.Add(guildId, getGuildName(s));
                }

            }

            var orderedCharList = sortChars(charList);



            return orderedCharList;
        } //old
    }
    public class CharLoginProcessor
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
    public class VanguardWindowProcessor
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
    public class InventoryProcessor
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
    public class SectionProcessor
    {
        const int SECTION_ID_OFFSET = 13 * 2;
        
        public uint GetLocationId(string p)
        {
            return Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));
        }
        public string GetLocationName(string p)
        {
            var locId = GetLocationId(p);
            var c = new LocationConverter();
            return (string)c.Convert(locId, null, null, null);
        }
    }
    public class CrystalbindProcessor
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
            else if(Time <= 3600000)
            {
                TCTNotifier.NotificationProvider.NS.sendNotification("Your Complete Crystalbind will expire soon.", TCTNotifier.NotificationType.Crystalbind, Colors.Orange);
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
                    Console.WriteLine("Starting deletion");
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
            Console.WriteLine("Cancelling deletion");
            ccbEnding = false;
        }

        async Task WaitCancel()
        {
            Console.WriteLine("Starting waiting");
            await Task.Delay(5000);
            Console.WriteLine("Waiting ended");
        }
        async void StartDeletion()
        {
            ccbEnding = true;
            Console.WriteLine("Deletion armed");
            await WaitCancel();
            if (ccbEnding)
            {
                Console.WriteLine( "Deletion confirmed" );
                EndCcb();
            }
            else
            {
                Console.WriteLine("Deletion canceled");
            }
        }
        void EndCcb()
        {
            Console.WriteLine("Deleting");
            Status = false;
            Time = 0;
            DataBridge.CurrentChar().Crystalbind = Time;
            TCTNotifier.NotificationProvider.NS.sendNotification("Your Complete Crystalbind expired.", TCTNotifier.NotificationType.Crystalbind, Colors.Red);
        }
        class Buff
        {
            public int Id { get; set; }
            public long TimeLeft { get; set; }
        }
    }

    public class SystemMessageProcessor { }
    public class AccountLoginProcessor { }

}

