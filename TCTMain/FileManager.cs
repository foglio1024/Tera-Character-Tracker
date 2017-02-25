using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using TCTData;
using TCTData.Enums;
using TCTUI;

namespace TCTMain
{
    public static class FileManager
    {
        public static void SaveCharacters(bool log)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, TCTData.Data.CharList);
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
            xs.Serialize(fs, TCTData.Data.AccountList);
            fs.Close();
            if (log)
            {
                UI.UpdateLog("Accounts saved.");
            }

        }
        public static void SaveGuildsDB(bool log)
        {
            if (TCTData.Data.GuildDictionary.Count > 0)
            {
                string[] lines = new string[TCTData.Data.GuildDictionary.Count];
                int i = 0;
                foreach (var item in TCTData.Data.GuildDictionary)
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
            //LastClosed = DateTime.Now;

            TCTData.Data.settings =
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
                       new XElement("Notifications", new XAttribute("value", "")),
                       new XElement("DarkTheme", new XAttribute("value", ""))
                    /*new setting here*/
                    )
                );

            TCTData.Data.settings.Descendants().Where(x => x.Name == "LastClosed").FirstOrDefault().Attribute("value").Value = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "Console").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Console.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "CcbFrequency").FirstOrDefault().Attribute("value").Value = TCTData.Settings.CcbNM.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "Top").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Top.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "Left").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Left.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "Width").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Width.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "Height").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Height.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "NotificationSound").FirstOrDefault().Attribute("value").Value = TCTData.Settings.NotificationSound.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "Notifications").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Notifications.ToString();
            TCTData.Data.settings.Descendants().Where(x => x.Name == "DarkTheme").FirstOrDefault().Attribute("value").Value = TCTData.Settings.Theme.ToString();
            /*new setting here*/

            TCTData.Data.settings.Save(Environment.CurrentDirectory + "\\content/data/settings.xml");
            if (log)
            {
                UI.UpdateLog("Settings saved.");
            }
        }

        public static void LoadSettings()
        {
            TCTData.Data.settings = new XDocument();
            if (File.Exists(Environment.CurrentDirectory + "\\content/data/settings.xml"))
            {
                TCTData.Data.settings = XDocument.Load(Environment.CurrentDirectory + "\\content/data/settings.xml");
                XElement LastClosedXE = TCTData.Data.settings.Descendants().Where(x => x.Name == "LastClosed").FirstOrDefault();

                double _LastClosed = Convert.ToDouble(LastClosedXE.Attribute("value").Value.ToString());
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

                Settings.LastClosed = dtDateTime.AddSeconds(_LastClosed).ToLocalTime();


                TCTData.Settings.Top = Convert.ToDouble(TCTData.Data.settings.Descendants().Where(x => x.Name == "Top").FirstOrDefault().Attribute("value").Value);
                TCTData.Settings.Left = Convert.ToDouble(TCTData.Data.settings.Descendants().Where(x => x.Name == "Left").FirstOrDefault().Attribute("value").Value);
                TCTData.Settings.Width = Convert.ToDouble(TCTData.Data.settings.Descendants().Where(x => x.Name == "Width").FirstOrDefault().Attribute("value").Value);
                TCTData.Settings.Height = Convert.ToDouble(TCTData.Data.settings.Descendants().Where(x => x.Name == "Height").FirstOrDefault().Attribute("value").Value);



                if (TCTData.Data.settings.Descendants().Where(x => x.Name == "CcbFrequency").FirstOrDefault().Attribute("value").Value == "EverySection")
                {
                    TCTData.Settings.CcbNM = NotificationMode.EverySection;
                }
                else
                {
                    TCTData.Settings.CcbNM = NotificationMode.TeleportOnly;
                }


                if (TCTData.Data.settings.Descendants().Where(x => x.Name == "NotificationSound").FirstOrDefault() == null)
                {
                    TCTData.Settings.NotificationSound = true;
                }
                else
                {
                    if (TCTData.Data.settings.Descendants().Where(x => x.Name == "NotificationSound").FirstOrDefault().Attribute("value").Value == "False")
                    {
                        TCTData.Settings.NotificationSound = false;
                    }
                    else
                    {
                        TCTData.Settings.NotificationSound = true;
                    }
                }

                if (TCTData.Data.settings.Descendants().Where(x => x.Name == "Notifications").FirstOrDefault() == null)
                {
                    TCTData.Settings.Notifications = true;
                }
                else
                {
                    if (TCTData.Data.settings.Descendants().Where(x => x.Name == "Notifications").FirstOrDefault().Attribute("value").Value == "False")
                    {
                        TCTData.Settings.Notifications = false;
                    }
                    else
                    {
                        TCTData.Settings.Notifications = true;
                    }
                }

                if (TCTData.Data.settings.Descendants().Where(x => x.Name == "DarkTheme").FirstOrDefault() != null)
                {
                    if (TCTData.Data.settings.Descendants().Where(x => x.Name == "DarkTheme").FirstOrDefault().Attribute("value").Value == "Dark")
                    {
                        TCTData.Settings.Theme = Theme.Dark;
                    }
                    else
                    {
                        TCTData.Settings.Theme = Theme.Light;
                    }
                }


                /*new setting here*/

            }

            else
            {
                TCTData.Settings.Top = 20;
                TCTData.Settings.Left = 20;
                TCTData.Settings.Width = 1280;
                TCTData.Settings.Height = 930;
                TCTData.Settings.CcbNM = NotificationMode.EverySection;
                TCTData.Settings.Console = false;
                TCTData.Settings.NotificationSound = true;
                TCTData.Settings.Notifications = true;
                TCTData.Settings.Theme = Theme.Light;
                /*new setting here*/
            }
        }
        public static void LoadCharacters()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Character>));

            if (File.Exists(Environment.CurrentDirectory + "\\content/data/characters.xml"))
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Open, FileAccess.Read);
                TCTData.Data.CharList = xs.Deserialize(fs) as List<Character>;
                fs.Close();

            }
            else
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/characters.xml", FileMode.Create, FileAccess.Write);
                fs.Close();
                TCTData.Data.CharList = new List<Character>();
                SaveCharacters(false);
            }
        }
        public static void LoadAccounts()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Account>));

            if (File.Exists(Environment.CurrentDirectory + "\\content/data/accounts.xml"))
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/accounts.xml", FileMode.Open, FileAccess.Read);
                TCTData.Data.AccountList = xs.Deserialize(fs) as List<Account>;
                fs.Close();

            }
            else
            {
                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/data/accounts.xml", FileMode.Create, FileAccess.Write);
                fs.Close();
                TCTData.Data.AccountList = new List<Account>();
                SaveAccounts(false);
            }
        }
        public static void LoadDungeons()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Dungeon>));
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\content/tera_database/dungeons.xml", FileMode.Open, FileAccess.Read);
            TCTData.Data.DungList = xs.Deserialize(fs) as List<Dungeon>;
            fs.Close();

        }
        public static void LoadGuildsDB()
        {
            TCTData.Data.GuildDictionary = new Dictionary<uint, string>();

            if (File.Exists(Environment.CurrentDirectory + "\\content/data/guilds.txt"))
            {
                foreach (string line in File.ReadLines(Environment.CurrentDirectory + "\\content/data/guilds.txt"))
                {
                    string[] ln = line.Split(',');
                    uint gID = 0;
                    UInt32.TryParse(ln[0], out gID);
                    TCTData.Data.GuildDictionary.Add(gID, ln[1]);
                }
            }

        }
        public static void LoadData()
        {
            LoadAccounts();
            LoadCharacters();
            TCTData.Data.SortChars();
            LoadDungeons();

            if (TCTData.Data.CharList != null && TCTData.Data.DungList != null)
            {
                TCTData.Data.CheckDungeonsList();
            }

            LoadGuildsDB();

            if (!TCTData.Data.GuildDictionary.ContainsKey(0))
            {
                TCTData.Data.GuildDictionary.Add(0, "No guild");
            }

        }


    }
}
