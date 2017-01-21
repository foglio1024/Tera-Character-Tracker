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
using System.IO.Compression;
using System.Windows.Forms;

namespace TCTMain
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        static string version = "v"+Assembly.GetExecutingAssembly().GetName().Version.Major+"."+Assembly.GetExecutingAssembly().GetName().Version.Minor;


        public class Threads
        {

            public static void NetThread()
            {
                //TCTSniffer.SnifferProgram.startNewSniffingSession();
                DamageMeter.Sniffing.TeraSniffer.Instance.Enabled = true;

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
                    UpdateManager.NotifyUpdateFail();
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
                    System.Windows.MessageBox.Show("An error occured. Check error.txt for more info");
                    Environment.Exit(-1);
                }
            }

        }
            

        static Mutex m;

        static void AppStartup()
        {
            bool isNewInstance = false;
            m = new Mutex(true, "TeraCharacterTracker.exe", out isNewInstance);
            if (!isNewInstance)
            {
                System.Windows.MessageBox.Show("A TCT instance is already running.","Warning");
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



        [STAThread]
        public static void Main()
        {
            TCTData.TCTProps.CurrentVersion = version;
            UpdateManager.CheckForUpdates();
            AppStartup();
            DeleteOldExe();



            //load settings
            Tera.TeraLogic.LoadSettings();
            Tera.TeraLogic.ResetCheck();
            Tera.TeraLogic.LoadData();
            TCTData.TCTDatabase.LoadTeraDB();

            Thread uiThread = new Thread(new ThreadStart(Threads.UIThread));
            Thread netThread = new Thread(new ThreadStart(Threads.NetThread));

            uiThread.SetApartmentState(ApartmentState.STA);
            netThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            netThread.Start();

        }

    }

}
