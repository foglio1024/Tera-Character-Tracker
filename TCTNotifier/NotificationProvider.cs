using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCTData.Enums;

namespace TCTNotifier
{

    internal delegate void NotificationAddedEventHandler(object sender, EventArgs e, NotificationInfo ni);
    internal delegate void NotificationEndedEventHandler(object sender, EventArgs e);
    public static class NotificationProvider
    {

        internal static Dictionary<NotificationImage, string> imagesDictionary = new Dictionary<NotificationImage, string>()
        {
            { NotificationImage.Default, "default" },
            { NotificationImage.Marks, "marks" },
            { NotificationImage.Crystalbind, "ccbBuff" },
            { NotificationImage.Goldfinger, "gfin" },
            { NotificationImage.Scales, "scale" },
            { NotificationImage.Connected, "connected" },
            { NotificationImage.Credits, "rep" }
        };

        internal static NotificationQueue NQ = new NotificationQueue();
        internal static NotificationListener NL = new NotificationListener(NQ);
        internal static NotificationDeployer NotificationDeployer = new NotificationDeployer();
        internal static List<NotificationInfo> deliveredNotifications = new List<NotificationInfo>();

        internal static double VerticalOffset { get; set; } = SystemParameters.FullPrimaryScreenHeight * .4;

        static void AddNotification(string _content, NotificationImage _img, NotificationType type, Color col, bool rep, bool snd, bool right)
        {
            if (TCTData.TCTProps.Notifications)
            {
                var n = new NotificationInfo(_content, _img, type, col, rep, snd, right);
                NQ.Add(n);
            }
        }

        public static void SendNotification(string _content, NotificationImage img, NotificationType t, Color col, bool rep, bool snd, bool right)
        {
            AddNotification(_content, img, t, col, rep, snd, right);
        }

        public static void SendNotification(string _content, Color c)
        {
            AddNotification(_content, NotificationImage.Default, NotificationType.Standard, c, true, true, false);
        }
    }
}

