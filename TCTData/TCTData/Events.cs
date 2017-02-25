using System;
using System.Windows.Media;
using TCTData.Enums;

namespace TCTData.Events
{

    public static class EventsManager
    {
        public static event Action<LogEntryEventArgs> LogEntryEvent;
        public static event Action<NotificationEventArgs> NotificationEvent;

        public static void SendLogEntry(LogEntryEventArgs e)
        {
            var handler = LogEntryEvent;
            handler?.Invoke(e);
        }
        public static void SendNotification(NotificationEventArgs e)
        {
            var handler = NotificationEvent;
            handler?.Invoke(e);
        }
    }
    public class LogEntryEventArgs : EventArgs
    {
        public string LogData;
        public LogEntryEventArgs(string logData)
        {
            LogData = logData;
        }
    }
    public class NotificationEventArgs : EventArgs
    {
        public string Content;
        public NotificationImage Image;
        public NotificationType Type;
        public Color Color;
        public bool Sound;
        public bool Repeat;
        public bool Right;

        public NotificationEventArgs(string con, NotificationImage i, NotificationType t, Color c, bool s, bool re, bool r)
        {
            Content = con;
            Image = i;
            Type = t;
            Color = c;
            Sound = s;
            Repeat = re;
            Right = r;
        }
    }
}
