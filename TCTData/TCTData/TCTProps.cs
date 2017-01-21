using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTData.Enums;

namespace TCTData
{
    public static class TCTProps
    {
        public static bool Reset { get; set; }
        public static bool FirstLaunchAfterReset { get; set; }
        public static DateTime LastClosed { get; set; }
        public static double Top { get; set; }
        public static double Left { get; set; }
        public static double Width { get; set; }
        public static double Height { get; set; }
        public static bool Console { get; set; }
        public static CcbNotificationMode CcbNM { get; set; } = CcbNotificationMode.TeleportOnly;
        public static bool NotificationSound { get; set; }
        public static bool Notifications { get; set; }

        public static string CurrentVersion { get; set; }
    }

}
