using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCTMain
{
    internal static class UpdateManager
    {
        static bool failedUpdate = false;

        static string versionPath = "http://tct.000webhostapp.com/tct-ver.txt";
        static string versionNumberFileName = "tct-last-ver";

        internal static void CheckForUpdates()
        {


            var currentVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor);


            string newVersionAsString = "";

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(versionPath, versionNumberFileName);
                }
            }
            catch (Exception)
            {
                failedUpdate = true;
            }



            if (File.Exists(versionNumberFileName))
            {
                using (StreamReader sr = new StreamReader(versionNumberFileName))
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
        }
        internal static void AskForUpdate()
        {
            var result = System.Windows.MessageBox.Show("New version available. Do you want to update?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("http://tct.000webhostapp.com/TCTUpdater.zip", "updater.zip");
                }
                if (File.Exists("TCTUpdater.exe"))
                {
                    File.Delete("TCTUpdater.exe");
                }
                ZipFile.ExtractToDirectory("updater.zip", Environment.CurrentDirectory);
                Process.Start("TCTUpdater.exe");
                Environment.Exit(0);
            }
            else if (result == MessageBoxResult.No)
            {
                File.Delete("tct-last-ver");
            }

        }
        internal static void NotifyUpdateFail()
        {
            if (failedUpdate)
            {
                TCTNotifier.NotificationProvider.SendNotification("Failed to check for updates", TCTData.Enums.NotificationImage.Connected, TCTData.Enums.NotificationType.Standard, Tera.UI.Colors.SolidRed, true, true, false);

            }
        }

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

    }
}
