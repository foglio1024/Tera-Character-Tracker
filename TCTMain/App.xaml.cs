using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using TCTSniffer;

namespace TCTMain
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static XDocument settings;
        private static DateTime LastClosed;
        public TCTNotifier.NotificationSender NS = new TCTNotifier.NotificationSender();

        public class Threads
        {
            public static void NetThread()
            {
                TCTSniffer.SnifferProgram.startNewSniffingSession();

            }
            public static void UIThread()
            {
                try
                {

                    Tera.TeraMainWindow w = new Tera.TeraMainWindow();
                    w.InitializeComponent();

                    if (dailyReset)
                    {
                        Tera.UI.win.Dispatcher.Invoke(new Action(()=> Tera.UI.win.resetDailyData(new object(), new RoutedEventArgs())));
                        Tera.UI.win.updateLog("Daily data has been reset.");
                        TCTNotifier.NotificationProvider.NS.sendNotification("Daily data has been reset.", TCTNotifier.NotificationType.Default, System.Windows.Media.Color.FromArgb(255, 0, 255, 100));
                        dailyReset = false;
                    }
                    if (weeklyReset)
                    {
                        Tera.UI.win.Dispatcher.Invoke(new Action(() => Tera.UI.win.resetWeeklyData(new object(), new RoutedEventArgs())));
                        Tera.UI.win.updateLog("Weekly data has been reset.");
                        TCTNotifier.NotificationProvider.NS.sendNotification("Weekly data has been reset.", TCTNotifier.NotificationType.Default, System.Windows.Media.Color.FromArgb(255, 0, 255, 100));

                        weeklyReset = false;
                    }
                    
                    w.ShowDialog();

                    /*save settings to xml*/
                    LastClosed = DateTime.Now;
                    settings.Descendants().Where(x => x.Name == "LastClosed").FirstOrDefault().Attribute("value").Value = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
                    settings.Descendants().Where(x => x.Name == "TeraClub").FirstOrDefault().Attribute("value").Value = Tera.TeraLogic.isTC.ToString();
                    settings.Descendants().Where(x => x.Name == "Console").FirstOrDefault().Attribute("value").Value =  Tera.TeraLogic.TCTProps.Console.ToString();
                    settings.Descendants().Where(x => x.Name == "Top").FirstOrDefault().Attribute("value").Value =      Tera.TeraLogic.TCTProps.Top.ToString();
                    settings.Descendants().Where(x => x.Name == "Left").FirstOrDefault().Attribute("value").Value =     Tera.TeraLogic.TCTProps.Left.ToString();
                    settings.Descendants().Where(x => x.Name == "Width").FirstOrDefault().Attribute("value").Value =    Tera.TeraLogic.TCTProps.Width.ToString();
                    settings.Descendants().Where(x => x.Name == "Height").FirstOrDefault().Attribute("value").Value =   Tera.TeraLogic.TCTProps.Height.ToString();
                    settings.Save(Environment.CurrentDirectory + "\\content/data/settings.xml");
                    Environment.Exit(0);


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                }
            }
            public static void LoadDatabases()
            {
                Tera.TeraLogic.LoadTeraDB();
                Tera.TeraLogic.LoadAccounts();
                Tera.TeraLogic.LoadCharacters();
                Tera.TeraLogic.LoadDungeons();
                Tera.TeraLogic.LoadGuildsDB();

                if (!Tera.TeraLogic.GuildDictionary.ContainsKey(0))
                {
                    Tera.TeraLogic.GuildDictionary.Add(0, "No guild");
                }

            }
        }
            

        private const int DAILY_RESET_HOUR = 5;
        public static bool dailyReset = false;
        public static bool weeklyReset = false;

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [STAThread]
        public static void Main()
        {
            /*load settings*/
            settings = new XDocument();
            if (File.Exists(Environment.CurrentDirectory + "\\content/data/settings.xml"))
            {
                settings = XDocument.Load(Environment.CurrentDirectory + "\\content/data/settings.xml");
                XElement LastClosedXE = settings.Descendants().Where(x => x.Name == "LastClosed").FirstOrDefault();
                XElement TeraClubXE = settings.Descendants().Where(x => x.Name == "TeraClub").FirstOrDefault();

                double _LastClosed = Convert.ToDouble(LastClosedXE.Attribute("value").Value.ToString());
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

                LastClosed = dtDateTime.AddSeconds(_LastClosed).ToLocalTime();
                if (TeraClubXE.Value == "True")
                {
                    Tera.TeraLogic.isTC = true;
                }
                else
                {
                    Tera.TeraLogic.isTC = false;
                }

                Tera.TeraLogic.TCTProps.Top = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Top").FirstOrDefault().Attribute("value").Value);
                Tera.TeraLogic.TCTProps.Left = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Left").FirstOrDefault().Attribute("value").Value);
                Tera.TeraLogic.TCTProps.Width = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Width").FirstOrDefault().Attribute("value").Value);
                Tera.TeraLogic.TCTProps.Height = Convert.ToDouble(settings.Descendants().Where(x => x.Name == "Height").FirstOrDefault().Attribute("value").Value);

                if(settings.Descendants().Where(x => x.Name == "Console").FirstOrDefault().Attribute("value").Value == "True")
                {
                    Tera.TeraLogic.TCTProps.Console = true;
                    AllocConsole();
                }
                else
                {
                    Tera.TeraLogic.TCTProps.Console = false;
                }
            }




            DateTime lastReset;
            if(DateTime.Now.Hour >= DAILY_RESET_HOUR)
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
                if(DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                {
                    weeklyReset = true;
                }
            }
            Thread uiThread = new Thread(new ThreadStart(Threads.UIThread));
            Thread netThread = new Thread(new ThreadStart(Threads.NetThread));

            uiThread.SetApartmentState(ApartmentState.STA);
            netThread.SetApartmentState(ApartmentState.STA);
            Threads.LoadDatabases();
            uiThread.Start();
            netThread.Start();

        }


    }

}
