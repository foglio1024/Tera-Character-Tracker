using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
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
                    TCTNotifier.NotificationProvider.Init();

                    TCTNotifier.NotificationProvider.SendNotification("TCT " + version + " is running");

                    Tera.TeraMainWindow w = new Tera.TeraMainWindow();
                    w.InitializeComponent();

                    Tera.TeraLogic.TryReset();
                    w.Title = "Tera Character Tracker " + version;
                    w.ShowDialog();

                    Tera.TeraLogic.SaveSettings();
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
                    Tera.TeraLogic.SaveAccounts();
                    Tera.TeraLogic.SaveSettings();
                    Tera.TeraLogic.SaveCharacters();
                    MessageBox.Show("An error occured. Check error.txt for more info");
                    Environment.Exit(-1);
                }
            }
            public static void LoadDatabases()
            {
                Tera.TeraLogic.LoadTeraDB();
                Tera.TeraLogic.LoadAccounts();
                Tera.TeraLogic.LoadCharacters();
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

        [STAThread]
        public static void Main()
        {
            /*load settings*/
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
