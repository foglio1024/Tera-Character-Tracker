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
using TCTParser.Processors;

namespace TCTParser
{
    public static class DataParser
    {
        static S_GET_USER_GUILD_LOGO mess;

        internal static string currentCharName;
        internal static string currentCharId;

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
        static CombatProcessor combatParser = new CombatProcessor();
        static NocteniumProcessor nocteniumProcessor = new NocteniumProcessor();

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

                case "S_LOGIN":
                    crystalbindProcessor.Clear();
                    LoginChar(data);
                    break;

                case "S_AVAILABLE_EVENT_MATCHING_LIST":
                    SetVanguardData(data, vanguardWindowProcessor.justLoggedIn);
                    vanguardWindowProcessor.justLoggedIn = false;
                    break;

                case "S_INVEN":
                    if (!(combatParser.IsInCombat && nocteniumProcessor.IsNocteniumOn))
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
                    else
                    {
                        UI.UpdateLog("Combat status and noctenium infusion are in effect. Inventory parsing is disabled.");
                    }
                    break;

                case "S_DUNGEON_COOL_TIME_LIST":
                    Tera.UI.UpdateLog(currentCharName + " > received dungeons data.");
                    wCforDungeons = data;
                    setDungs();
                    break;

                case "S_SYSTEM_MESSAGE":
                    sysMsgProcessor.ParseSystemMessage(data);
                    break;

                case "S_VISIT_NEW_SECTION":
                    NewSection(data);
                    break;

                case "S_UPDATE_NPCGUILD":
                    wCforUpdatedCreditsAfterPurchase = data;
                    UpdateVanguardCredits();
                    break;

                case "S_ABNORMALITY_BEGIN":
                    crystalbindProcessor.ParseNewBuff(data, currentCharId);
                    nocteniumProcessor.ParseBegin(data, currentCharId);
                    break;

                case "S_ABNORMALITY_END":
                    crystalbindProcessor.ParseEndingBuff(data, currentCharId);
                    nocteniumProcessor.ParseEnd(data, currentCharId);
                    break;

                case "S_CLEAR_ALL_HOLDED_ABNORMALITY":
                    crystalbindProcessor.CancelDeletion();
                    break;

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

        }

        //Methods
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
        }
        private static void LoginChar(string p)
        {
            currentCharName = charLoginProcessor.GetName(p);
            currentCharId = charLoginProcessor.GetId(p);

            Tera.UI.UpdateLog(currentCharName + " logged in.");
            Tera.UI.MainWin.Dispatcher.Invoke(new Action(() => Tera.TeraLogic.SelectCharacter(currentCharName)));

            TeraLogic.cvcp.SelectedChar.LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private static void SetTokens(bool forceLog)
        {

            inventoryProcessor.ParseInventory();
            var newMarks = inventoryProcessor.MarksOfValor;
            var newGoldfinger = inventoryProcessor.GoldfingerTokens;
            var newDragonScales = inventoryProcessor.DragonwingScales;

            bool marks = false;
            bool gft = false;
            bool scales = false;

            if (CurrentChar().MarksOfValor != newMarks)
            {
                marks = true;
                var col = GetColor(CurrentChar().MarksOfValor, newMarks);

                CurrentChar().MarksOfValor = newMarks;

                if (CurrentChar().MarksOfValor > 82)
                {
                    UI.UpdateLog("You've almost reached the maximum amount of Elleon's Marks of Valor.");
                    UI.SendNotification("Your Elleon's Marks of Valor amount is close to the maximum (" + CurrentChar().MarksOfValor + ").", NotificationImage.Marks, NotificationType.Standard, Colors.Orange, true, true, false);
                }
                else
                {
                    UI.SendNotification(newMarks.ToString(), NotificationImage.Marks, NotificationType.Counter, col, true, false, true);
                }
            }

            if (CurrentChar().GoldfingerTokens != newGoldfinger)
            {
                gft = true;
                var col = GetColor(CurrentChar().GoldfingerTokens, newGoldfinger);

                CurrentChar().GoldfingerTokens = newGoldfinger;

                if (CurrentChar().GoldfingerTokens >= 80)
                {
                    Tera.UI.UpdateLog("You have " + newGoldfinger + " Goldfinger Tokens.");
                    UI.SendNotification("You have " + CurrentChar().GoldfingerTokens + " Goldfinger Tokens. You can buy a Laundry Box.", NotificationImage.Goldfinger, NotificationType.Standard, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true, true, false);
                }
                else
                {
                    UI.SendNotification(newGoldfinger.ToString(), NotificationImage.Goldfinger, NotificationType.Counter, col, true, false, true);
                }
            }
            if (CurrentChar().DragonwingScales != newDragonScales)
            {
                scales = true;
                var col = GetColor(CurrentChar().DragonwingScales, newDragonScales);

                CurrentChar().DragonwingScales = newDragonScales;

                if (CurrentChar().DragonwingScales >= 50)
                {
                    Tera.UI.UpdateLog("You have " + newDragonScales + " Dragonwing Scales.");
                    UI.SendNotification("You have " + CurrentChar().DragonwingScales + " Dragonwing Scales. You can buy a Dragon Egg.", NotificationImage.Default, NotificationType.Standard, UI.Colors.SolidGreen, true, true, false);
                }
                else
                {
                    UI.SendNotification(newDragonScales.ToString(), NotificationImage.Scales, NotificationType.Counter, col, true, false, true);
                }
            }
            inventoryProcessor.Clear();

            if(marks || gft || scales || forceLog)
            {
                Tera.UI.UpdateLog(currentCharName + " > inventory data updated (" + CurrentChar().MarksOfValor + " Elleon's Marks of Valor, " + CurrentChar().GoldfingerTokens + " Goldfinger Tokens, "+CurrentChar().DragonwingScales+" Dragonwing Scales).");
            }

            CurrentChar().LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }
        static System.Windows.Media.Color GetColor(int oldVal, int newVal)
        {
            if (oldVal > newVal)
            {
                return UI.Colors.SolidRed;
            }
            else
            {
                return UI.Colors.SolidGreen;
            }
        }
        private static void SetVanguardData(string p, bool forceLog)
        {
            int weekly = vanguardWindowProcessor.GetWeekly(p); //Convert.ToInt32(wCforVGData[7 * 8].ToString() + wCforVGData[7 * 8 + 1].ToString(), 16);
            int credits = vanguardWindowProcessor.GetCredits(p); //Convert.ToInt32(wCforVGData[8 * 8 + 2].ToString() + wCforVGData[8 * 8 + 3].ToString()+ wCforVGData[8 * 8 + 0].ToString() + wCforVGData[8 * 8 + 1].ToString() , 16);
            int completed_dailies = vanguardWindowProcessor.GetDaily(p); //Convert.ToInt32(wCforVGData.Substring(3 * 8, 2).ToString(), 16);
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
                                                                                            
                Tera.UI.UpdateLog(CurrentChar().Name + " moved to " + sectionProcessor.GetLocationId(p).ToString()/*GetLocationName(p)*/ + ".");
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

        //to be refactored
        const int VANGUARD_REP_ID = 609;

        private static string wCforDungeons;
        private static string wCforUpdatedCreditsAfterPurchase;

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
                            UI.SendNotification(CurrentChar().Name + " gained "+diff+" Vanguard Credits."+"\n"+"Current amount: " + CurrentChar().Credits + ".", NotificationImage.Credits, NotificationType.Standard, UI.Colors.SolidGreen, true, false, false);
                        }
                        else
                        {
                            UI.SendNotification(CurrentChar().Name + " gained " + diff + " Vanguard Credits." + "\n" + "Current amount: " + CurrentChar().Credits + ", you've almost reached your maximum credits.", NotificationImage.Credits, NotificationType.Standard, Colors.Orange, true, true, false);
                        }
                    }
                    else //spent
                    {
                        diff = -diff;
                        UI.UpdateLog(currentCharName + " > " + "spent " + diff + " Vanguard credits. Current amount: " + cr + ".");
                        UI.SendNotification(CurrentChar().Name + " spent " + diff + " Vanguard Credits."+"\n"+"Current amount: " + CurrentChar().Credits + ".", NotificationImage.Credits, NotificationType.Standard, Colors.Red, true, false, false);
                    }
                }
            }
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
    }
}

