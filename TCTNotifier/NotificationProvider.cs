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

    public delegate void NotificationAddedEventHandler(object sender, EventArgs e, NotificationInfo ni);
    public delegate void NotificationEndedEventHandler(object sender, EventArgs e);
    public static class NotificationProvider
    {
        public static NotificationInfo[] lastNotification = new NotificationInfo[2] { new NotificationInfo("Null notification", NotificationImage.Default, Colors.Black, false, false), new NotificationInfo("Null notification", NotificationImage.Default, Colors.Black, false, false) };

        public static Dictionary<NotificationImage, string> imagesDictionary = new Dictionary<NotificationImage, string>()
        {
            { NotificationImage.Default, "default" },
            { NotificationImage.Marks, "marks" },
            { NotificationImage.Crystalbind, "ccbBuff" },
            { NotificationImage.Goldfinger, "gfin" },
            { NotificationImage.Scales, "scale" },
            { NotificationImage.Connected, "connected" },
            { NotificationImage.Credits, "rep" }
        };

        public static NotificationQueue NQ = new NotificationQueue();
        public static NotificationListener NL = new NotificationListener(NQ);
        public static Notification N = new Notification();
        static NotificationSender NS;
        static void AddNotification(NotificationInfo _n)
        {
            NQ.Add(_n);
        }
        public static void SendNotification(string _content, NotificationImage _nType, Color col, bool rep, bool snd)
        {
            NS.sendNotification(_content, _nType, col, rep, snd);
        }
        public static void SendNotification(string _content)
        {
            NS.sendNotification(_content);
        }
        public static void Init()
        {
            NS = new NotificationSender();
        }

        class NotificationSender
        {
            string content;
            NotificationImage nType;
            Color ringColor;

            public NotificationSender()
            {
                content = "Empty";
                nType = NotificationImage.Default;
                initNotification();

            }
            void initNotification()
            {
                prepareNotification(new NotificationInfo("Empty", NotificationImage.Default, Colors.WhiteSmoke, false, false));
                NotificationProvider.N.Dispatcher.Invoke(() => NotificationProvider.N.Hide());
            }

            void prepareNotification(NotificationInfo _n)
            {
                content = _n.Content;
                nType = _n.Image;
                ringColor = _n.Color;
                NotificationProvider.N.Dispatcher.Invoke(() =>
                {
                    NotificationProvider.N.NotificationHolder.Children.Clear();
                    NotificationProvider.N.ShowActivated = false;
                    NotificationProvider.N.Show();
                });
            }
            void notify(NotificationInfo n)
            {
                NotificationProvider.N.Dispatcher.Invoke(new Action(() =>
                {
                    var sn = new StandardNotification();
                    sn.txt.Text = content;
                    sn.glowColor = ringColor;
                    NotificationProvider.N.NotificationHolder.Children.Add(sn);
                    NotificationProvider.N.ShowInTaskbar = false;
                    NotificationProvider.N.ShowActivated = false;
                    ImageBrush imgB = new ImageBrush();

                    try
                    {
                        string val = "";
                        NotificationProvider.imagesDictionary.TryGetValue(nType, out val);
                        imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/" + val + ".png", UriKind.RelativeOrAbsolute));
                    }

                    catch (Exception ex)
                    {
                        imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/default.png", UriKind.RelativeOrAbsolute));
                        Console.WriteLine(ex.ToString());

                    }

                    sn.icon.Fill = imgB;

                    NotificationProvider.N.Pop(n);
                }));
            }

            public void sendNotification(string _content, NotificationImage _nType, Color col, bool rep, bool snd)
            {
                var _n = new NotificationInfo(_content, _nType, col, rep, snd);
                if (NotificationProvider.NQ.Count >= 0)
                {
                    if (_n.Content == NotificationProvider.lastNotification[0].Content || _n.Content == NotificationProvider.lastNotification[1].Content)
                    {
                        if (_n.IsRepeatable)
                        {
                            NotificationProvider.AddNotification(_n);
                        }
                    }
                    else
                    {
                        NotificationProvider.AddNotification(_n);
                    }
                }
                else
                {
                    NotificationProvider.AddNotification(_n);
                }
                NotificationProvider.lastNotification[1] = NotificationProvider.lastNotification[0];
                NotificationProvider.lastNotification[0] = _n;
            }

            public void sendNotification(string _content)
            {
                var _n = new NotificationInfo(_content, NotificationImage.Default, Color.FromArgb(255, 0, 123, 206), true, true);

                NotificationProvider.AddNotification(_n);

            }
        }
    }
}

