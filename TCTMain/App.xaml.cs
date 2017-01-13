using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
        static string version = "v"+Assembly.GetExecutingAssembly().GetName().Version.Major+"."+Assembly.GetExecutingAssembly().GetName().Version.Minor;

        class Version
        {
            public int Primary { get; set; }
            public int Secondary { get; set; }

            public Version(int p, int s)
            {
                Primary = p;
                Secondary = s;
            }
        }

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
                    TCTNotifier.NotificationProvider.SendNotification("TCT " + version + " is running",TCTData.Enums.NotificationImage.Default,TCTData.Enums.NotificationType.Standard, Tera.UI.Colors.SolidBaseColor,true,false,false);

                    Tera.TeraMainWindow w = new Tera.TeraMainWindow();
                    w.InitializeComponent();

                    Tera.TeraLogic.TryReset();
                    w.Title = "Tera Character Tracker " + version;
                    w.ShowDialog();

                    Tera.TeraLogic.SaveSettings(false);
                    Environment.Exit(0);


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                    using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "\\error.txt", true))
                    {
                        writer.WriteLine("Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                           "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                    Tera.TeraLogic.SaveAccounts(false);
                    Tera.TeraLogic.SaveSettings(false);
                    Tera.TeraLogic.SaveCharacters(false);
                    MessageBox.Show("An error occured. Check error.txt for more info");
                    Environment.Exit(-1);
                }
            }
            public static void LoadDatabases()
            {
                Tera.TeraLogic.LoadTeraDB();
                Tera.TeraLogic.LoadAccounts();
                Tera.TeraLogic.LoadCharacters();
                Tera.TeraLogic.SortChars();
                Tera.TeraLogic.LoadDungeons();

                if (Tera.TeraLogic.CharList != null && Tera.TeraLogic.DungList != null)
                {
                    Tera.TeraLogic.CheckDungeonsList(); 
                }

                Tera.TeraLogic.LoadGuildsDB();

                if (!Tera.TeraLogic.GuildDictionary.ContainsKey(0))
                {
                    Tera.TeraLogic.GuildDictionary.Add(0, "No guild");
                }

            }
        }
            

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        static Mutex m;

        static void AppStartup()
        {
            bool isNewInstance = false;
            m = new Mutex(true, "TeraCharacterTracker.exe", out isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("A TCT instance is already running.","Warning");
                App.Current.Shutdown();
                   
            }
        }
        static void DeleteOldExe()
        {
            if (File.Exists(Environment.CurrentDirectory + "\\Tera Character Tracker.exe"))
            {
                File.Delete(Environment.CurrentDirectory + "\\Tera Character Tracker.exe");
            }
        }

        static void CheckForUpdates()
        {
            var currentVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor);

            string newVersionAsString = "";
            using (var client = new WebClient())
            {
                client.DownloadFile("http://tct.000webhostapp.com/tct-ver.txt", "tct-last-ver");
            }
            using (StreamReader sr = new StreamReader("tct-last-ver"))
            {
                newVersionAsString = sr.ReadToEnd();
            }
            var newVersion = new Version(Convert.ToInt32(newVersionAsString.Split('.')[0]), Convert.ToInt32(newVersionAsString.Split('.')[1]));

            if (currentVersion.Primary == newVersion.Primary)
            {
                if (currentVersion.Secondary == newVersion.Secondary)
                {
                    //Do nothing                  
                }
                else if (currentVersion.Secondary < newVersion.Secondary)
                {
                    AskForUpdate();
                }
            }
            else if (currentVersion.Primary < newVersion.Primary)
            {
                AskForUpdate();
            }
        }
        static void AskForUpdate()
        {
            var result = System.Windows.MessageBox.Show("New version available. Do you want to update?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start("TCTUpdater.exe");
                Environment.Exit(0);
            }
            else if (result == MessageBoxResult.No)
            {
                //do nothing
            }

        }

        [STAThread]
        public static void Main()
        {

            AppStartup();
            DeleteOldExe();

            CheckForUpdates();

            //load settings
            Tera.TeraLogic.LoadSettings();
            Tera.TeraLogic.ResetCheck();

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
