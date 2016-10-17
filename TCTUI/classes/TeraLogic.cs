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

namespace Tera
{
    public enum CcbNotificationMode
    {
        TeleportOnly = 0,
        EverySection = 1
    }
    public static class TeraLogic 
    {
        public static class TCTProps
        {
            public static bool Reset { get; set; }
            public static bool FirstLaunchAfterReset { get; set; }
            public static DateTime LastClosed { get; set; }
            public static double Top { get; set; }
            public static double Left { get; set; }
            public static double Width { get; set; }
            public static double Height { get; set; }
            public static bool Console { get; set; }
            public static CcbNotificationMode CcbNM { get; set; } = CcbNotificationMode.TeleportOnly;
            public static Color baseColor = Color.FromArgb(255, 96, 125, 139);
            public static Color accentColor = Color.FromArgb(255, 255, 120, 42);
        }

        public const int MAX_WEEKLY = 15;
        public const int MAX_DAILY = 8;
        public const int MAX_CREDITS = 9000;
        public const int MAX_MARKS = 100;
        public const int MAX_GF_TOKENS = 80;
     

        public static bool IsSaved { get; set; }
        public static List<Character> CharList { get; set; }
        public static List<Dungeon> DungList{ get; set; }
        public static List<Account> AccountList { get; set; }
        public static Dictionary<uint, string> GuildDictionary { get; set; }
        public static XDocument EventMatching;
        public static XDocument DailyPlayGuideQuest;
        public static XDocument StrSheet_DailyPlayGuideQuest;
        public static XDocument StrSheet_Region;
        public static XDocument StrSheet_Dungeon;
        public static XDocument NewWorldMapData;
        public static List<XDocument> StrSheet_Item_List;
        public static CharViewContentProvider cvcp = new CharViewContentProvider();

        public static List<uint> RiskList { get; } = new List<uint> { 9070, 9767, 9068, 9067, 9768, 9611 };

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

                        CharList.Last().Dungeons.Add(new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns));
                    }
                    else
                    {
                        CharList.Last().Dungeons.Add(new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns * tc));

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
            var w = UI.MainWin.chView;

            // set name and class
            w.charName.Text = TeraLogic.cvcp.SelectedChar.Name;
            w.charClassTB.Text = TeraLogic.cvcp.SelectedChar.CharClass;

            // create binding for class/laurel images
            DataBinder.BindParameterToImageSourceWithConverter(charIndex, "CharClass", w.classImg, "hd", new ClassToImage());
            DataBinder.BindParameterToImageSourceWithConverter(charIndex, "Laurel", w.laurelImg, "hd", new LaurelToImage());

            // create bindings for text blocks
            w.charName.SetBinding(      TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Name"));
            w.weeklyTB.SetBinding(      TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Weekly"));
            w.dailiesTB.SetBinding(     TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Dailies"));
            w.creditsTB.SetBinding(     TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Credits"));
            w.mvTB.SetBinding(          TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "MarksOfValor"));
            w.gfTB.SetBinding(          TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "GoldfingerTokens"));
            w.guildNameTB.SetBinding(   TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "GuildId", new Guild_IdToName(), null));
            w.locationTB.SetBinding(    TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "LocationId" , new Location_IdToName(), null));
            w.lastOnlineTB.SetBinding(  TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "LastOnline", new UnixToDateTime(), null));
            w.notesTB.SetBinding(       TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Notes"));

            // create width bindings for bars
            w.questsBar.SetBinding(     Shape.WidthProperty, DataBinder.GenericCharBinding(charIndex, "Weekly", new ValueToBarLenght(), new double[] { UI.MainWin.chView.baseBar.ActualWidth, Convert.ToDouble(MAX_WEEKLY) }));
            w.dailiesBar.SetBinding(    Shape.WidthProperty, DataBinder.GenericCharBinding(charIndex, "Dailies", new Daily_ValueToBarWidth(), new double[] { UI.MainWin.chView.baseBar.ActualWidth, Convert.ToDouble(MAX_WEEKLY - CharList[charIndex].Weekly) }));
            w.creditsBar.SetBinding(    Shape.WidthProperty, DataBinder.GenericCharBinding(charIndex, "Credits", new ValueToBarLenght(), new double[] { UI.MainWin.chView.baseBar.ActualWidth, Convert.ToDouble(MAX_CREDITS) }));
            w.marksBar.SetBinding(      Shape.WidthProperty, DataBinder.GenericCharBinding(charIndex, "MarksOfValor", new ValueToBarLenght(), new double[] { UI.MainWin.chView.baseBar.ActualWidth, Convert.ToDouble(MAX_MARKS) }));
            w.gfBar.SetBinding(         Shape.WidthProperty, DataBinder.GenericCharBinding(charIndex, "GoldfingerTokens", new ValueToBarLenght(), new double[] { UI.MainWin.chView.baseBar.ActualWidth, Convert.ToDouble(MAX_GF_TOKENS) }));

            // create color bindings for bars
            w.questsBar.SetBinding(     Shape.FillProperty, DataBinder.GenericCharBinding(charIndex, "Weekly", new ValueToBarColor(), 7));
            w.dailiesBar.SetBinding(    Shape.FillProperty, DataBinder.GenericCharBinding(charIndex, "Weekly", new ValueToBarColor(), 7));
            w.creditsBar.SetBinding(    Shape.FillProperty, DataBinder.GenericCharBinding(charIndex, "Credits", new ValueToBarColor(), 8000));
            w.marksBar.SetBinding(      Shape.FillProperty, DataBinder.GenericCharBinding(charIndex, "MarksOfValor", new ValueToBarColor(), 80));
            w.gfBar.SetBinding(         Shape.FillProperty, DataBinder.GenericCharBinding(charIndex, "GoldfingerTokens", new ValueToBarColor(), 65));

            // create bindings for dungeon counters
            DataBinder.CreateDgBindings(charIndex, w);

            // highlight character row and scroll into view
            foreach (var ns in Tera.TeraMainWindow.CharacterStrips)
            {
                if (ns.Tag != null)
                {
                    if (ns.Tag.Equals(name))
                    {
                        ns.rowSelect(true);
                        UI.MainWin.overviewPage.tableGridContent.ScrollIntoView(ns);
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

                int i = 0;
                foreach (var d in c.Dungeons)
                {
                    if (d.Name.Equals("CA") || d.Name.Equals("AH") || d.Name.Equals("GL") || d.Name.Equals("EA"))
                    {
                        d.Runs = TeraLogic.DungList[i].MaxBaseRuns;
                    }
                    else
                    {
                        d.Runs = TeraLogic.DungList[i].MaxBaseRuns * tc;
                    }
                    i++;
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
                        CharList[i].Dungeons.Insert(j, new CharDungeon(DungList[j].ShortName, 0));
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

    #region File Management
        public static void SaveCharacters()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, CharList);
            fs.Close();
        }
        public static void SaveAccounts()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Account>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/accounts.xml", FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, AccountList);
            fs.Close();

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
                SaveCharacters();
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
                SaveAccounts();
            }
        }
        public static void LoadDungeons()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Dungeon>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/dungeons.xml", FileMode.Open, FileAccess.Read);
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
        internal static void SaveGuildsDB()
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
            }

        }
        public static void LoadTeraDB()
        {
            DailyPlayGuideQuest             = new XDocument();
            DailyPlayGuideQuest             = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/DailyPlayGuideQuest.xml");
            EventMatching                   = new XDocument();
            EventMatching                   = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/EventMatching.xml");
            StrSheet_DailyPlayGuideQuest    = new XDocument();
            StrSheet_DailyPlayGuideQuest    = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_DailyPlayGuideQuest.xml");
            StrSheet_Region                 = new XDocument();
            StrSheet_Region                 = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_Region.xml");
            NewWorldMapData                 = new XDocument();
            NewWorldMapData                 = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/NewWorldMapData.xml");
            StrSheet_Dungeon                = new XDocument();
            StrSheet_Dungeon                = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_Dungeon-0.xml");
            StrSheet_Item_List              = new List<XDocument>();
            int i = 0;
            while (File.Exists(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_Item/StrSheet_Item-" + i + ".xml"))
            {
                var doc = new XDocument();
                doc = XDocument.Load(Environment.CurrentDirectory + "\\content/tera_database/StrSheet_Item/StrSheet_Item-" + i + ".xml");
                StrSheet_Item_List.Add(doc);
                i++;
            }
        }
    #endregion
    }
}
