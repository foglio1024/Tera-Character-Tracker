using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTData.Enums;
using System.Windows.Forms;
using Tera;

namespace TCTUI
{
    public static class UI
    {
        public static TeraMainWindow MainWin;
        public static CharView CharView;
        internal static AccountContainer CharListContainer;
        public static CharViewContentProvider cvcp = new CharViewContentProvider();

        public static NotifyIcon NotifyIcon;

        public static void UpdateLog(string data)
        {
            MainWin.UpdateLog(data);
        }

        public static void SendNotification(string content, NotificationImage img, NotificationType t, Color col, bool repeat, bool sound, bool right)
        {
            TCTNotifier.NotificationProvider.SendNotification(content, img, t, col, repeat, sound, right);
        }
        public static void SendDefaultNotification(string content)
        {
            TCTNotifier.NotificationProvider.SendNotification(content, TCTData.Colors.SolidBaseColor);
        }

        public static void SetLogColor(Color c)
        {
            MainWin.Dispatcher.Invoke(() =>
            {
                MainWin.Log.Background = new SolidColorBrush(c);
            });
        }




    }
}
