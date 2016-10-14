using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using Tera.Data;
//using Tera.Game;
//using Tera.Sniffing;
using System.Text;
//using Tera.PacketLog;
using System.Globalization;

using System.IO;
using System.Reflection;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    /// 


    public partial class TCTApp : Application
    {
       public class Th
        {
           public static void ui()
            {
                //var UIApp = new TCTApp();
                //UIApp.InitializeComponent();
                //UIApp.Run();
                //TeraMainWindow mw = new Tera.TeraMainWindow();
                //mw.InitializeComponent();
                //mw.ShowDialog();
            }

            public static void net()
            {
                //AllocConsole();
                //TCTSniffer.
            }
        }
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

   //     [STAThread]
        //public static void Main()

        //{
        //    //Thread UIThread = new Thread(new ThreadStart(Th.ui));
        //    //Thread NetThread = new Thread(new ThreadStart(Th.net));

        //    //UIThread.SetApartmentState(ApartmentState.STA);
        //    //NetThread.SetApartmentState(ApartmentState.STA);

        //    //UIThread.Start();
        //    //NetThread.Start();


        //}
    }
}
