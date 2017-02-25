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
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using TCTUI.Converters;
using System.Windows.Data;
using System.Globalization;
using TCTData.Enums;
using TCTParser.Processors;
using Data;
using TCTUI;
using TCTData;

namespace TCTParser
{
    public static class DataRouter
    {

        internal static string currentCharName;
        internal static string currentCharEntityId;
        internal static long currentCharId;
        public static uint Version { get; set; }


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
        static DungeonClearsProcessor dungeonClearsProcessor = new DungeonClearsProcessor();
        static DungeonRunsProcessor dungeonRunsProcessor = new DungeonRunsProcessor();
        static CreditsUpdateProcessor creditsUpdateProcessor = new CreditsUpdateProcessor();
        static GuildLogoProcessor guildLogoProcessor = new GuildLogoProcessor();

        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemOpCodeNamer;

        public static Character CurrentChar
        { get
            {
                return TCTData.Data.CharList[TCTData.Data.CharList.IndexOf(TCTData.Data.CharList.Find(c => c.Name == currentCharName))];
            }
        }


        public static void StoreMessage(Message msg)
        {
            byte[] data = new byte[msg.Data.Count];
            Array.Copy(msg.Data.Array, 0, data, 2, msg.Data.Count - 2);
            data[0] = (byte)(((short)msg.Data.Count) & 255);
            data[1] = (byte)(((short)msg.Data.Count) >> 8);

            ParseLastMessage(OpCodeNamer.GetName(msg.OpCode), StringUtils.ByteArrayToString(data).ToUpper(), msg);

        }

        static void ParseLastMessage(string opCodeName, string data, Message m)
        {
            switch (opCodeName)
            {
                case "S_GET_USER_LIST":
                    crystalbindProcessor.Clear();
                    if (!TCTData.Data.AccountList.Contains(TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id)))
                    {
                        TCTData.Data.AccountList.Add(new TCTData.Account(accountLoginProcessor.id, accountLoginProcessor.tc, accountLoginProcessor.vet, accountLoginProcessor.tcTime));
                    }
                    else
                    {
                        TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id).TeraClub = accountLoginProcessor.tc;
                        TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id).Veteran = accountLoginProcessor.vet;
                        TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id).TeraClubDate = accountLoginProcessor.tcTime;
                    }
                    charListProcessor.CurrentAccountId = accountLoginProcessor.id;
                    SetCharList(data);
                    TCTData.FileManager.SaveAccounts();
                    TCTData.FileManager.SaveCharacters();
                    UI.UpdateLog("Data saved.");
                    break;

                case "S_GET_USER_GUILD_LOGO": 
                    SetLogo(data);
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
                        CurrentChar.Ilvl = inventoryProcessor.GetItemLevel(data); 
                    }
                    else
                    {
                        UI.UpdateLog("Combat status and Noctenium Infusion are in effect. Inventory parsing disabled.");
                    }
                    break;

                case "S_DUNGEON_COOL_TIME_LIST":
                    dungeonRunsProcessor.UpdateCoolTimes(data);
                    break;
                case "S_DUNGEON_CLEAR_COUNT_LIST":
                    dungeonClearsProcessor.UpdateClears(data);
                    break;
                case "S_SYSTEM_MESSAGE":
                    sysMsgProcessor.ParseSystemMessage(data);
                    break;

                case "S_VISIT_NEW_SECTION":
                    NewSection(data);
                    break;

                case "S_UPDATE_NPCGUILD":
                    creditsUpdateProcessor.UpdateCredits(data);
                    UpdateLastOnline();
                    break;

                case "S_ABNORMALITY_BEGIN":
                    crystalbindProcessor.ParseNewBuff(data, currentCharEntityId);
                    nocteniumProcessor.ParseBegin(data, currentCharEntityId);
                    break;

                case "S_ABNORMALITY_END":
                    crystalbindProcessor.ParseEndingBuff(data, currentCharEntityId);
                    nocteniumProcessor.ParseEnd(data, currentCharEntityId);
                    break;

                case "S_CLEAR_ALL_HOLDED_ABNORMALITY":
                    crystalbindProcessor.CancelDeletion();
                    break;

                case "S_LOGIN_ACCOUNT_INFO":
                    accountLoginProcessor.ParseLoginInfo(data);

                    break;

                case "S_ACCOUNT_PACKAGE_LIST":
                    accountLoginProcessor.ParsePackageInfo(data);
                    if (!TCTData.Data.AccountList.Contains(TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id)))
                    {
                        TCTData.Data.AccountList.Add(new TCTData.Account(accountLoginProcessor.id, accountLoginProcessor.tc, accountLoginProcessor.vet, accountLoginProcessor.tcTime));
                    }
                    else
                    {
                        TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id).TeraClub = accountLoginProcessor.tc;
                        TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id).Veteran = accountLoginProcessor.vet;
                        TCTData.Data.AccountList.Find(x => x.Id == accountLoginProcessor.id).TeraClubDate = accountLoginProcessor.tcTime;
                    }

                    break;

                case "S_RETURN_TO_LOBBY":
                    TCTData.FileManager.SaveAccounts();
                    TCTData.FileManager.SaveCharacters();
                    UI.UpdateLog("Data saved.");
                    inventoryProcessor.justLoggedIn = true;
                    vanguardWindowProcessor.justLoggedIn = true;
                    break;

                case "S_GUILD_QUEST_LIST":
                    guildQuestListProcessor.Parse(m);
                    break;

                case "S_FINISH_GUILD_QUEST":
                    UI.UpdateLog("Guild quest completed.");
                    break;

                case "S_START_GUILD_QUEST":
                    guildQuestListProcessor.TakeQuest(data);
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
                    if(combatParser.GetUserId(data) == currentCharEntityId)
                    {
                        combatParser.SetUserStatus(data);
                    }
                    break;

                default:
                    break;
            }

        }

        //Methods
        private static void SetCharList(string p)
        {
            var charList = charListProcessor.ParseCharacters(p);
            //for (int i = 0; i < charList.Count; i++)
            //{
            //    UI.MainWin.Dispatcher.Invoke(new Action(() => TCTData.Data.AddCharacter(charList[i])));
            //}

            foreach (var c in charList)
            {
                UI.MainWin.Dispatcher.Invoke(new Action(() => TCTData.Data.AddCharacter(c)));
            }
            charListProcessor.Clear();

            UI.UpdateLog("Found " + charList.Count + " characters.");
        }
        private static void LoginChar(string p)
        {
            currentCharName = charLoginProcessor.GetName(p);
            currentCharEntityId = charLoginProcessor.GetEntityId(p);
            currentCharId = charLoginProcessor.GetCharId(p);

            UI.UpdateLog(currentCharName + " logged in.");
            UI.MainWin.Dispatcher.Invoke(new Action(() => UIManager.SelectCharacter(currentCharName)));

            UpdateLastOnline();
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

            if (CurrentChar.MarksOfValor != newMarks)
            {
                marks = true;
                var col = GetColor(CurrentChar.MarksOfValor, newMarks);

                CurrentChar.MarksOfValor = newMarks;

                if (CurrentChar.MarksOfValor > 82)
                {
                    UI.UpdateLog("You've almost reached the maximum amount of Elleon's Marks of Valor.");
                    UI.SendNotification("Your Elleon's Marks of Valor amount is close to the maximum (" + CurrentChar.MarksOfValor + ").", NotificationImage.Marks, NotificationType.Standard, TCTData.Colors.SolidOrange, true, true, false);
                }
                else
                {
                    UI.SendNotification(newMarks.ToString(), NotificationImage.Marks, NotificationType.Counter, col, true, false, true);
                }
            }

            if (CurrentChar.GoldfingerTokens != newGoldfinger)
            {
                gft = true;
                var col = GetColor(CurrentChar.GoldfingerTokens, newGoldfinger);

                CurrentChar.GoldfingerTokens = newGoldfinger;

                if (CurrentChar.GoldfingerTokens >= 80)
                {
                    UI.UpdateLog("You have " + newGoldfinger + " Goldfinger Tokens.");
                    UI.SendNotification("You have " + CurrentChar.GoldfingerTokens + " Goldfinger Tokens. You can buy a Laundry Box.", NotificationImage.Goldfinger, NotificationType.Standard, TCTData.Colors.BrightGreen, true, true, false);
                }
                else
                {
                    UI.SendNotification(newGoldfinger.ToString(), NotificationImage.Goldfinger, NotificationType.Counter, col, true, false, true);
                }
            }
            if (CurrentChar.DragonwingScales != newDragonScales)
            {
                scales = true;
                var col = GetColor(CurrentChar.DragonwingScales, newDragonScales);

                CurrentChar.DragonwingScales = newDragonScales;

                if (CurrentChar.DragonwingScales >= 50)
                {
                    UI.UpdateLog("You have " + newDragonScales + " Dragonwing Scales.");
                    UI.SendNotification("You have " + CurrentChar.DragonwingScales + " Dragonwing Scales. You can buy a Dragon Egg.", NotificationImage.Scales, NotificationType.Standard, TCTData.Colors.BrightGreen, true, true, false);
                }
                else
                {
                    UI.SendNotification(newDragonScales.ToString(), NotificationImage.Scales, NotificationType.Counter, col, true, false, true);
                }
            }
            inventoryProcessor.Clear();

            if(marks || gft || scales || forceLog)
            {
                UI.UpdateLog(currentCharName + " > inventory data updated (" + CurrentChar.MarksOfValor + " Elleon's Marks of Valor, " + CurrentChar.GoldfingerTokens + " Goldfinger Tokens, "+CurrentChar.DragonwingScales+" Dragonwing Scales).");
            }

            UpdateLastOnline();

        }
        static System.Windows.Media.Color GetColor(int oldVal, int newVal)
        {
            if (oldVal > newVal)
            {
                return TCTData.Colors.BrightRed;
            }
            else
            {
                return TCTData.Colors.BrightGreen;
            }
        }
        private static void SetVanguardData(string p, bool forceLog)
        {
            int weekly = vanguardWindowProcessor.GetWeekly(p); //Convert.ToInt32(wCforVGData[7 * 8].ToString() + wCforVGData[7 * 8 + 1].ToString(), 16);
            int credits = vanguardWindowProcessor.GetCredits(p); //Convert.ToInt32(wCforVGData[8 * 8 + 2].ToString() + wCforVGData[8 * 8 + 3].ToString()+ wCforVGData[8 * 8 + 0].ToString() + wCforVGData[8 * 8 + 1].ToString() , 16);
            int completed_dailies = vanguardWindowProcessor.GetDaily(p); //Convert.ToInt32(wCforVGData.Substring(3 * 8, 2).ToString(), 16);
            int remaining_dailies = TCTData.TCTConstants.MAX_DAILY - completed_dailies;

            if (CurrentChar.Weekly != weekly ||
                CurrentChar.Credits != credits ||
                CurrentChar.Dailies != remaining_dailies ||
                forceLog)
            {

                UI.UpdateLog(CurrentChar.Name + " > vanguard data updated (" + credits + " credits, " + weekly + " weekly quests done, " + remaining_dailies + " dailies left).");
            }

            CurrentChar.Weekly = weekly;
            CurrentChar.Credits = credits;
            CurrentChar.Dailies = remaining_dailies;

            UpdateLastOnline();
        }
        private static void NewSection(string p)
        {
            if (CurrentChar.LocationId != sectionProcessor.GetLocationId(p))
            {
                CurrentChar.LocationId = sectionProcessor.GetLocationId(p);
                if (TCTData.Settings.CcbNM == NotificationMode.TeleportOnly)
                {

                    crystalbindProcessor.CheckCcb(sectionProcessor.GetLocationId(p), sectionProcessor.GetLocationNameId(p));
                    guildQuestListProcessor.CheckQuestStatus(sectionProcessor.GetLocationId(p), sectionProcessor.GetLocationNameId(p));
                }
                                                                                            
                UI.UpdateLog(CurrentChar.Name + " moved to " + sectionProcessor.GetLocationName(p) + ".");




            }

            if (TCTData.Settings.CcbNM == NotificationMode.EverySection)
            {
                crystalbindProcessor.CheckCcb(sectionProcessor.GetLocationId(p), sectionProcessor.GetLocationNameId(p));
                guildQuestListProcessor.CheckQuestStatus(sectionProcessor.GetLocationId(p), sectionProcessor.GetLocationNameId(p));

            }

            UpdateLastOnline();
        }
        private static void SetLogo(string p)
        {

            Bitmap logo = guildLogoProcessor.GetLogo(p);
            uint guildId = guildLogoProcessor.GetGuildID(p);

            try
            {
                logo.Save(Environment.CurrentDirectory + "\\content/data/guild_images/" + guildId.ToString() + ".bmp");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }
        
        internal static void UpdateLastOnline()
        {
            CurrentChar.LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}

