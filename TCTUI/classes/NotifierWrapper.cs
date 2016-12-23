using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTNotifier;
using TCTData.Enums;

namespace Tera
{

    public class NotifierWrapper
    {
        public static void SendNotification(string content, NotificationType nt, System.Windows.Media.Color col, bool repeat, bool sound)
        {
            NotificationProvider.SendNotification(content, nt, col, repeat, sound);
        } 
        public static void SendDefaultNotification(string content)
        {
            NotificationProvider.SendNotification(content);
        }
    }
}
