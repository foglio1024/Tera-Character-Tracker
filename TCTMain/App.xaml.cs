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
using TCTParser;
using TCTUI;
using Tera.Game;
using Data;
using System.Text;
using Tera;

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
                DamageMeter.Sniffing.TeraSniffer.Instance.MessageReceived += (message) =>
                {
                    if (message.Direction == MessageDirection.ClientToServer && message.OpCode == 19900)
                    {
                        var msg = new C_CHECK_VERSION_CUSTOM(new CustomReader(message));
                        DamageMeter.Sniffing.TeraSniffer.Instance.opn = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/{msg.Versions[0]}.txt"));
                        TCTParser.DataRouter.OpCodeNamer = DamageMeter.Sniffing.TeraSniffer.Instance.opn;
                        TCTParser.DataRouter.SystemOpCodeNamer = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/smt_{msg.Versions[0]}.txt"));
                    }

                    TCTParser.DataRouter.StoreMessage(message);
                };
                DamageMeter.Sniffing.TeraSniffer.Instance.NewConnection += (server) =>
                {
                    TCTNotifier.NotificationProvider.SendNotification(String.Format("Connected to:\n{0} - {1}", server.Region, server.Name), TCTData.Enums.NotificationImage.Connected, TCTData.Enums.NotificationType.Standard, TCTData.Colors.BrightGreen, true, false, false);
                    UI.SetLogColor(TCTData.Colors.SolidGreen);
                    UI.UpdateLog(String.Format("Connected to: {0} - {1}", server.Region, server.Name));
                };
                DamageMeter.Sniffing.TeraSniffer.Instance.EndConnection += () =>
                {
                    TCTNotifier.NotificationProvider.SendNotification("Connection lost.", TCTData.Enums.NotificationImage.Connected, TCTData.Enums.NotificationType.Standard, TCTData.Colors.BrightRed, true, false, false);
                    UI.SetLogColor(TCTData.Colors.SolidRed);
                    UI.UpdateLog("Connection lost.");

                };
            }
            class C_CHECK_VERSION_CUSTOM
            {
                public C_CHECK_VERSION_CUSTOM(CustomReader reader)
                {
                    var count = reader.ReadUInt16();
                    var offset = reader.ReadUInt16();
                    for (var i = 1; i <= count; i++)
                    {
                        reader.BaseStream.Position = offset - 4;
                        var pointer = reader.ReadUInt16();
                        var nextOffset = reader.ReadUInt16();
                        var VersionKey = reader.ReadUInt32();
                        var VersionValue = reader.ReadUInt32();
                        Versions.Add(VersionKey, VersionValue);
                        offset = nextOffset;
                    }
                }

                public Dictionary<uint, uint> Versions { get; } = new Dictionary<uint, uint>();
            }
            class CustomReader : BinaryReader
            {
                public CustomReader(Tera.Message message)
                : base(GetStream(message), Encoding.Unicode)
                {
                    Message = message;
                }

                public Tera.Message Message { get; private set; }
                public string OpCodeName { get; private set; }
                public uint Version { get; private set; }
                internal OpCodeNamer SysMsgNamer { get; private set; }

                private static MemoryStream GetStream(Tera.Message message)
                {
                    return new MemoryStream(message.Payload.Array, message.Payload.Offset, message.Payload.Count, false, true);
                }

                public EntityId ReadEntityId()
                {
                    var id = ReadUInt64();
                    return new EntityId(id);
                }

                public Vector3f ReadVector3f()
                {
                    Vector3f result;
                    result.X = ReadSingle();
                    result.Y = ReadSingle();
                    result.Z = ReadSingle();
                    return result;
                }

                public Angle ReadAngle()
                {
                    return new Angle(ReadInt16());
                }

                public void Skip(int count)
                {
                    ReadBytes(count);
                }

                // Tera uses null terminated litte endian UTF-16 strings
                public string ReadTeraString()
                {
                    var builder = new StringBuilder();
                    while (true)
                    {
                        var c = ReadChar();
                        if (c == 0)
                            return builder.ToString();
                        builder.Append(c);
                    }
                }

            }


            public static void UIThread()
            {
                try
                {
                    string v = String.Empty;
                    if(Assembly.GetExecutingAssembly().GetName().Version.Minor < 10)
                    {
                        v = "v" + Assembly.GetExecutingAssembly().GetName().Version.Major + ".0" + Assembly.GetExecutingAssembly().GetName().Version.Minor;
                    }
                    else
                    {
                        v = version;
                    }
                    TCTNotifier.NotificationProvider.SendNotification("TCT " + v + " is running", TCTData.Enums.NotificationImage.Default, TCTData.Enums.NotificationType.Standard, TCTData.Colors.SolidBaseColor, true, false, false);

                    Tera.TeraMainWindow w = new Tera.TeraMainWindow();
                    w.InitializeComponent();

                    Tera.TeraLogic.TryReset();
                    w.Title = "Tera Character Tracker " + v;
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
