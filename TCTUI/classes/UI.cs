using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTData.Enums;
using System.Windows.Forms;

namespace Tera
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
            TCTNotifier.NotificationProvider.SendNotification(content, Colors.SolidBaseColor);
        }

        public static void SetLogColor(Color c)
        {
            MainWin.Dispatcher.Invoke(() =>
            {
                MainWin.Log.Background = new SolidColorBrush(c);
            });
        }


        public static class Colors
        {
            public static Color SolidBaseColor = Color.FromArgb(255, 0, 123, 206);
            public static Color SolidAccentColor = Color.FromArgb(255, 255, 120, 42);
            public static Color SolidYellow = Color.FromArgb(255, 255, 166, 77);
            public static Color SolidGreen = Color.FromArgb(255, 88, 180, 91);
            public static Color SolidRed = Color.FromArgb(255, 255, 80, 80);
            public static Color SolidGray = Color.FromArgb(255, 200, 200, 200);

            public static Color FadedBaseColor = Color.FromArgb(150, SolidBaseColor.R, SolidBaseColor.G, SolidBaseColor.B);
            public static Color FadedAccentColor = Color.FromArgb(150, SolidAccentColor.R, SolidAccentColor.G, SolidAccentColor.B);
            public static Color FadedYellow = Color.FromArgb(150, SolidYellow.R, SolidYellow.G, SolidYellow.B);
            public static Color FadedGreen = Color.FromArgb(150, SolidGreen.R, SolidGreen.G, SolidGreen.B);
            public static Color FadedRed = Color.FromArgb(150, SolidRed.R, SolidRed.G, SolidRed.B);
            public static Color FadedGray = Color.FromArgb(150, SolidGray.R, SolidGray.G, SolidGray.B);
            
        }


    }
}
