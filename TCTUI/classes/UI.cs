using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTData.Enums;
using System.Windows.Forms;
using Tera;
using System.Windows.Controls;

namespace TCTUI
{
    public static class UI
    {

        const int LOG_CAP = 100;

        public static TeraMainWindow MainWin;
        public static CharView CharView;
        internal static AccountContainer CharListContainer;
        public static Character SelectedChar { get; set; }
        public static NotifyIcon NotifyIcon;


        public static void SendNotification(string content, NotificationImage img, NotificationType t, Color col, bool repeat, bool sound, bool right)
        {
            TCTNotifier.NotificationProvider.SendNotification(content, img, t, col, repeat, sound, right);
        }
        public static void SendDefaultNotification(string content)
        {
            TCTNotifier.NotificationProvider.SendNotification(content, TCTData.Colors.SolidBaseColor);
        }

        public static void UpdateLog(string txt)
        {
            MainWin.Dispatcher.Invoke(new Action(() =>
            {
                var li = new ListBoxItem();

                DateTime time = DateTime.Now;
                string format = "HH:mm";
                li.Focusable = false;
                li.IsTabStop = false;
                li.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                li.Content = "[" + time.ToString(format) + "]  -  " + txt;
                if (MainWin.Log.Items.Count > 0)
                {
                    ListBoxItem item = MainWin.Log.Items[MainWin.Log.Items.Count - 1] as ListBoxItem;
                    var tmp = item.Content as string;
                    tmp = tmp.Substring(12);

                    if (txt != tmp)
                    {
                        MainWin.Log.Items.Add(li);
                        MainWin.Log.ScrollIntoView(li);
                    }
                }
                else
                {
                    MainWin.Log.Items.Add(li);
                    MainWin.Log.ScrollIntoView(li);

                }
                if (MainWin.Log.Items.Count > LOG_CAP)
                {
                    MainWin.Log.Items.RemoveAt(0);
                }

            }

            ));


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
