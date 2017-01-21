using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Tera;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Media;
using Tera.Converters;
using System.Runtime.InteropServices;
using TCTData.Enums;

namespace Tera
{

    public static class TeraLogic
    {



        public const int MAX_WEEKLY = 15;
        public const int MAX_DAILY = 8;
        public const int MAX_CREDITS = 9000;
        public const int MAX_MARKS = 100;
        public const int MAX_GF_TOKENS = 80;
        private const int DAILY_RESET_HOUR = 5;

        public static bool dailyReset = false;
        public static bool weeklyReset = false;
        public static bool IsSaved { get; set; }
        public static List<Character> CharList { get; set; }
        public static List<Character> DeletedChars = new List<Character>();
        public static Dictionary<string, int> ResettedDailies = new Dictionary<string, int>();
        public static Dictionary<string, int> ResettedWeekly = new Dictionary<string, int>();
        public static List<Dungeon> DungList{ get; set; }
        public static List<Account> AccountList { get; set; }
        public static Dictionary<uint, string> GuildDictionary { get; set; }
        private static XDocument settings;
        private static DateTime LastClosed;
        public static CharViewContentProvider cvcp = new CharViewContentProvider();

        public static List<Delegate> UndoList { get; set; }

        public static void AddCharacter(Character c)
        {
            bool found = false;
            foreach (var cl in CharList)
            {
                if(cl.Name == c.Name)
                {
                    found = true;
                    cl.Laurel = c.Laurel;
                    cl.Level = c.Level;
                    cl.CharClass = c.CharClass;
                    cl.GuildId = c.GuildId;
                    cl.LocationId = c.LocationId;
                    cl.LastOnline = c.LastOnline;
                    cl.AccountId = c.AccountId;
                    cl.Position = c.Position;

                    break;
                }

            }

            if (!found)
            {
                // add char to chList
                CharList.Add(c);

                // check for TC
                int tc = 1;
                if (AccountList.Find(a => a.Id == c.AccountId).TeraClub)
                {
                    tc = 2;
                }

                // initialize dungeons
                for (int j = 0; j < DungList.Count; j++)
                {
                    if (DungList[j].ShortName == "AH" || DungList[j].ShortName == "EA" || DungList[j].ShortName == "GL" || DungList[j].ShortName == "CA")
                    {

                        CharList.Last().Dungeons.Add(new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns,0));
                    }
                    else
                    {
                        CharList.Last().Dungeons.Add(new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns * tc,0));

                    }
                }

                // create and add strip to list
                UI.MainWin.CreateStrip(CharList.Count - 1);

            }

        }
        public static void SelectCharacter(string name)
        {

            cvcp.SelectedChar = TeraLogic.CharList.Find(x => x.Name.Equals(name));
            var charIndex = (TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Equals(TeraLogic.cvcp.SelectedChar))));
            var w = UI.CharView;

        // set name and class
            w.charName.Text = TeraLogic.cvcp.SelectedChar.Name;
            w.charClassTB.Text = TeraLogic.cvcp.SelectedChar.CharClass;

        // create binding for class/laurel images
            DataBinder.BindParameterToImageSourceWithConverter(charIndex, "CharClass", w.classImg, "hd", new ClassToImage());
            DataBinder.BindParameterToImageSourceWithConverter(charIndex, "Laurel", w.laurelImg, "hd", new LaurelToImage());

        // create bindings for text blocks
            w.charName.SetBinding(      TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Name"));
            //w.weeklyTB.SetBinding(      TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Weekly"));
            //w.dailiesTB.SetBinding(     TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Dailies"));
            //w.creditsTB.SetBinding(     TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Credits"));
            //w.mvTB.SetBinding(          TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "MarksOfValor"));
            //w.gfTB.SetBinding(          TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "GoldfingerTokens"));
            w.guildNameTB.SetBinding(   TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "GuildId", new Guild_IdToName(), null));
            w.locationTB.SetBinding(    TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "LocationId" , new LocationIdToName(), null));
            w.lastOnlineTB.SetBinding(  TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "LastOnline", new UnixToDateTime(), null));
            w.ilvlTB.SetBinding(        TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Ilvl"));
            w.dragonScalesTB.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "DragonwingScales"));
            w.notesTB.SetBinding(       TextBox.TextProperty,   DataBinder.GenericCharBinding(charIndex, "Notes"));
            
        // create bindings for dungeon counters
            DataBinder.CreateDgBindings(charIndex, w);
            DataBinder.CreateDgClearsBindings(charIndex, w);

        // highlight character row and scroll into view
            foreach (var ns in Tera.TeraMainWindow.CharacterStrips)
            {
                if (ns.Tag != null)
                {
                    if (ns.Tag.Equals(name))
                    {
                        ns.rowSelect(true);
                        UI.CharListContainer.chContainer.ScrollIntoView(ns);
                    }
                    else
                    {
                        ns.rowSelect(false);
                    }
                }
            }

        // set guild image
            if(File.Exists(Environment.CurrentDirectory + "\\content/data/guild_images/" + CharList[charIndex].GuildId.ToString() + ".bmp"))
            {
                try
                {
                    System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\content/data/guild_images/" + CharList[charIndex].GuildId.ToString() + ".bmp");
                    UI.MainWin.SetGuildImage(bmp);
                }
                catch
                {
                    UI.MainWin.UpdateLog("Error while setting guild image. Using default image.");
                }
            }
            else
            {
                UI.MainWin.UpdateLog("Guild image not found. Using default image.");
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\content/data/guild_images/" + "0" + ".bmp");
                UI.MainWin.SetGuildImage(bmp);
            }
        }
        public static void ResetDailyData()
        {
            /*resets dungeons runs*/
            int tc = 1;
            foreach (var c in TeraLogic.CharList)
            {
                if (TeraLogic.AccountList.Find(a => a.Id == c.AccountId).TeraClub)
                {
                    tc = 2;
                }
                else
                {
                    tc = 1;
                }

                foreach (var d in c.Dungeons)
                {
                    if (d.Name.Equals("CA") || d.Name.Equals("AH") || d.Name.Equals("GL") || d.Name.Equals("EA"))
                    {
                        if (DungList.Find(x => x.ShortName == d.Name) != null)
                        {
                            d.Runs = DungList.Find(x => x.ShortName == d.Name).MaxBaseRuns;
                        }

                    }
                    else
                    {
                        if (DungList.Find(x => x.ShortName == d.Name) != null)
                        {

                            d.Runs = DungList.Find(x => x.ShortName == d.Name).MaxBaseRuns * tc;
                        }
                    }
                }
            }

            /*reset dailies*/
            foreach (var c in TeraLogic.CharList)
            {
                c.Dailies = 8;
            }

        }
        public static void ResetWeeklyData()
        {
            foreach (var c in TeraLogic.CharList)
            {
                c.Weekly = 0;
            }
        }
        public static void CheckDungeonsList()
        {
            bool found = false;

            for (int i = 0; i < CharList.Count; i++)
            {
                for (int j = 0; j < DungList.Count; j++)
                {
                    found = false;

                    for (int h = 0; h < CharList[i].Dungeons.Count; h++)
                    {
                        if (CharList[i].Dungeons[h].Name == DungList[j].ShortName)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        int tc = 1;
                        if(AccountList.Find(x => x.Id == CharList[i].AccountId).TeraClub)
                        {
                            tc = 2;
                        }
                        CharList[i].Dungeons.Insert(j, new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns * tc,0));
                    }
                }
            }

            found = false;

            for (int i = 0; i < CharList.Count; i++)
            {
                for (int h = 0; h < CharList[i].Dungeons.Count; h++)
                {
                    found = false;

                    for (int j = 0; j < DungList.Count; j++)
                    {
                        if (CharList[i].Dungeons[h].Name == DungList[j].ShortName)
                        {
                            found = true;
                            break;
                        }

                    }

                    if (!found)
                    {
                        CharList[i].Dungeons.RemoveAt(h);
                    }

                }
            }


        }
        public static void TryReset()
        {
            if (dailyReset)
            {
                ResetDailyData();
                UI.UpdateLog("Daily data has been reset.");
                UI.SendNotification("Daily data has been reset.", NotificationImage.Default, NotificationType.Standard, UI.Colors.SolidGreen,true, true, false);

                dailyReset = false;
            }
            if (weeklyReset)
            {
                ResetWeeklyData();
                UI.UpdateLog("Weekly data has been reset.");
                UI.SendNotification("Weekly data has been reset.", NotificationImage.Default, NotificationType.Standard, UI.Colors.SolidGreen, true, true, false);

                weeklyReset = false;
            }
        }
        public static void ResetCheck()
        {
            DateTime lastReset;
            if (DateTime.Now.Hour >= DAILY_RESET_HOUR)
            {
                lastReset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DAILY_RESET_HOUR, 0, 0);
            }

            else
            {
                lastReset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, DAILY_RESET_HOUR, 0, 0);
            }

            if (LastClosed < lastReset)
            {
                dailyReset = true;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                {
                    weeklyReset = true;
                }
            }

        }
        public static void SortChars()
        {
            var sortedList = new List<Character>();
            foreach (var account in AccountList)
            {
                var list = new List<Character>();
                foreach (var character in CharList)
                {
                    if(character.AccountId == account.Id)
                    {
                        list.Add(character);
                    }
                }
                list.Sort();
                foreach (var item in list)
                {
                    sortedList.Add(item);
                    Console.WriteLine(item.Name + " " + item.Position);
                }
            }
            CharList = sortedList;
        }


    #region File Management
        public static void SaveCharacters(bool log)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, CharList);
            fs.Close();

            if (log)
            {
                UI.UpdateLog("Characters saved."); 
            }
        }
        public static void SaveAccounts(bool log)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Account>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/accounts.xml", FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, AccountList);
            fs.Close();
            if (log)
            {
                UI.UpdateLog("Accounts saved."); 
            }

        }
        public static void LoadCharacters()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));

            if(File.Exists(Environment.CurrentDirectory + "\\content/data/characters.xml"))
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Open, FileAccess.Read);
                CharList = xs.Deserialize(fs) as List<Character>;
                fs.Close();

            }
            else 
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Create, FileAccess.Write);
                fs.Close();
                CharList = new List<Character>();
                SaveCharacters(false);
            }
        }
        public static void LoadAccounts()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Account>));

            if (File.Exists(Environment.CurrentDirectory + "\\content/data/accounts.xml"))
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/accounts.xml", FileMode.Open, FileAccess.Read);
                AccountList = xs.Deserialize(fs) as List<Account>;
                fs.Close();

            }
            else
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/accounts.xml", FileMode.Create, FileAccess.Write);
                fs.Close();
                AccountList = new List<Account>();
                SaveAccounts(false);
            }
        }
        public static void LoadDungeons()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Dungeon>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/tera_database/dungeons.xml", FileMode.Open, FileAccess.Read);
            DungList = xs.Deserialize(fs) as List<Dungeon>;
            fs.Close();

        }
        public static void LoadGuildsDB()
        {
            GuildDictionary = new Dictionary<uint, string>();

            if (File.Exists(Environment.CurrentDirectory + "\\content/data/guilds.txt"))
            {
                foreach (string line in File.ReadLines(Environment.CurrentDirectory + "\\content/data/guilds.txt"))
                {
                    string[] ln = line.Split(',');
                    uint gID = 0;
                    UInt32.TryParse(ln[0], out gID);
                    GuildDictionary.Add(gID, ln[1]);
                }
            }

        }
        internal static void SaveGuildsDB(bool log)
        {
            if (GuildDictionary.Count > 0)
            {
                string[] lines = new string[GuildDictionary.Count];
                int i = 0;
                foreach (var item in GuildDictionary)
                {
                    lines[i] = item.Key + "," + item.Value;
                    i++;
                }
                File.WriteAllLines(Environment.CurrentDirectory + "\\content/data/guilds.txt", lines);

                if (log)
                {
                    UI.UpdateLog("Guilds database saved."); 
                }
            }

        }
        public static void SaveSettings(bool log)
        {
            LastClosed = DateTime.Now;

            settings =
                new XDocument
                (
                    new XElement("Settings",
                       new XElement("LastClosed", new XAttribute("value", "")),
                       new XElement("Console", new XAttribute("value", "")),
                       new XElement("CcbFrequency", new XAttribute("value", "")),
                       new XElement("Top", new XAttribute("value", "")),
                       new XElement("Left", new XAttribute("value", "")),
                       new XElement("Width", new XAttribute("value", "")),
                       new XElement("Height", new XAttribute("value", "")),
                       new XElement("NotificationSound", new XAttribute("value", "")),
                       new XElement("Notifications", new XAttribute("value", ""))
                       /*new setting here*/
                    )
                );

            settings.Descendants().Where(x => x.Name == "LastClosed").FirstOrDefault().Attribute("value").Value = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            settings.Descendants().Where(x => x.Name == "Console").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.Console.ToString();
            settings.Descendants().Where(x => x.Name == "CcbFrequency").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.CcbNM.ToString();
            settings.Descendants().Where(x => x.Name == "Top").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.Top.ToString();
            settings.Descendants().Where(x => x.Name == "Left").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.Left.ToString();
            settings.Descendants().Where(x => x.Name == "Width").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.Width.ToString();
            settings.Descendants().Where(x => x.Name == "Height").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.Height.ToString();
            settings.Descendants().Where(x => x.Name == "NotificationSound").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.NotificationSound.ToString();
            settings.Descendants().Where(x => x.Name == "Notifications").FirstOrDefault().Attribute("value").Value = TCTData.TCTProps.Notifications.ToString();
            /*new setting here*/

            settings.Save(Environment.CurrentDirectory + "\\content/data/settings.xml");
            if (log)
            {
                UI.UpdateLog("Settings saved.");
            }
        }
        public static void LoadData()
        {
            LoadAccounts();
            LoadCharacters();
            SortChars();
            LoadDungeons();

            if (CharList != null && DungList != null)
            {
                CheckDungeonsList();
            }

            LoadGuildsDB();

            if (!GuildDictionary.ContainsKey(0))
            {
                GuildDictionary.Add(0, "No guild");
            }

        }


        public static void LoadSettings()
        {
            settings = new XDocument();
            if (File.Exists(Environment.CurrentDirectory + "\\content/data/settings.xml"))
            {
                settings = XDocument.Load(Environment.CurrentDirectory + "\\content/data/settings.xml");
                XElement LastClosedXE = settings.Descendants().Where(x => x.Name == "LastClosed").FirstOrDefault();

                double _LastClosed = Convert.ToDouble(LastClosedXE.Attribute("value").Value.ToString());
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

                LastClosed = dtDateTime.AddSeconds(_LastClosed).ToLocalTime();


                TCTData.TCTProps.Top = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Top").FirstOrDefault().Attribute("value").Value);
                TCTData.TCTProps.Left = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Left").FirstOrDefault().Attribute("value").Value);
                TCTData.TCTProps.Width = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Width").FirstOrDefault().Attribute("value").Value);
                TCTData.TCTProps.Height = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Height").FirstOrDefault().Attribute("value").Value);



                if (settings.Descendants().Where(x => x.Name == "CcbFrequency").FirstOrDefault().Attribute("value").Value == "EverySection")
                {
                    TCTData.TCTProps.CcbNM = CcbNotificationMode.EverySection;
                }
                else
                {
                    TCTData.TCTProps.CcbNM = CcbNotificationMode.TeleportOnly;
                }


                if (settings.Descendants().Where(x => x.Name == "NotificationSound").FirstOrDefault() == null)
                {
                    TCTData.TCTProps.NotificationSound = true;
                }

                else
                {
                    if (settings.Descendants().Where(x => x.Name == "NotificationSound").FirstOrDefault().Attribute("value").Value == "False")
                    {
                        TCTData.TCTProps.NotificationSound = false;
                    }
                    else
                    {
                        TCTData.TCTProps.NotificationSound = true;
                    }
                }
                if (settings.Descendants().Where(x => x.Name == "Notifications").FirstOrDefault() == null)
                {
                    TCTData.TCTProps.Notifications = true;
                }

                else
                {
                    if (settings.Descendants().Where(x => x.Name == "Notifications").FirstOrDefault().Attribute("value").Value == "False")
                    {
                        TCTData.TCTProps.Notifications = false;
                    }
                    else
                    {
                        TCTData.TCTProps.Notifications = true;
                    }
                }
                /*new setting here*/

            }

            else
            {
                TCTData.TCTProps.Top = 20;
                TCTData.TCTProps.Left = 20;
                TCTData.TCTProps.Width = 1280;
                TCTData.TCTProps.Height = 930;
                TCTData.TCTProps.CcbNM = CcbNotificationMode.EverySection;
                TCTData.TCTProps.Console = false;
                TCTData.TCTProps.NotificationSound = true;
                TCTData.TCTProps.Notifications = true;
                /*new setting here*/
            }
        }

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        #endregion
    }
}
