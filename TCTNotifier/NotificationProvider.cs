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

namespace TCTNotifier
{
    public enum NotificationType
    {
        Default,
        Crystalbind,
        MarksNotification,
        Goldfinger,
        Connected,
        Credits
    }
    public class NotificationInfo
    {
        public string content;
        public NotificationType nType;
        public Color ringColor;
        public NotificationInfo(string _c, NotificationType _nt, Color _co)
        {
            content = _c;
            nType = _nt;
            ringColor = _co;
        }
    }
    public delegate void NotificationAddedEventHandler(object sender, EventArgs e, NotificationInfo ni);
    public delegate void NotificationEndedEventHandler(object sender, EventArgs e);
    public static class NotificationProvider
    {
        public static NotificationInfo lastNotification = new NotificationInfo("Null notification",NotificationType.Default, Colors.Black);
        public static Dictionary<NotificationType, string> imagesDictionary = new Dictionary<NotificationType, string>()
        {
            { NotificationType.Default, "default" },
            { NotificationType.MarksNotification, "marks" },
            { NotificationType.Crystalbind, "ccbBuff" },
            { NotificationType.Goldfinger, "gfin" },
            { NotificationType.Connected, "connected" },
            { NotificationType.Credits, "rep" }
        };

        public static NotificationQueue NQ = new NotificationQueue();
        public static NotificationListener NL = new NotificationListener(NQ);
        public static Notification N = new Notification();
        public static NotificationSender NS = new NotificationSender();
        public static void AddNotification(NotificationInfo _n)
        {
            NQ.Add(_n);
        }
        public class NotificationQueue : List<NotificationInfo>
        {
            public bool Busy { get; set; } = false;
            public event NotificationAddedEventHandler Added;
            public event NotificationEndedEventHandler Over;

            protected virtual void OnNewNotification(EventArgs e)
            {
                if (Added != null)
                    Added(this, e, this.Last());
            }
            protected virtual void OnNotificationEnded(EventArgs e)
            {
                if (Over != null)
                    Over(this, e);
            }

            public new void Add(NotificationInfo value)
            {
                //int i = Add(value);
                NQ.Insert(NQ.Count, value);
                OnNewNotification(EventArgs.Empty);
                //return i;
            }
            public void SetBusyToFalseOnEnd()
            {
                Busy = false;
                OnNotificationEnded(EventArgs.Empty);
            }
            public void SetBusy()
            {
                Busy = true;
            }
        }
        public class NotificationListener
        {
            private NotificationQueue nq;
            public NotificationListener(NotificationQueue _nq)
            {
                nq = _nq;
                nq.Added += new NotificationAddedEventHandler(NotifyNew);
                nq.Over += new NotificationEndedEventHandler(NextNotification);
            }
            private void NextNotification(object sender, EventArgs e)
            {
                if(NQ.Count > 0)
                {
                    NotifyNew(new object(), new EventArgs(), NQ.First());
                }
            }
            private void NotifyNew(object sender, EventArgs e, NotificationInfo ni)
            {
                if (!NQ.Busy)
                {
                    NQ.SetBusy();
                    N.Dispatcher.Invoke(() =>
                    {
                        N.NotificationHolder.Children.Clear();
                        N.Show();
                    });

                    N.Dispatcher.Invoke(new Action(() =>
                    {
                        var sn = new StandardNotification();
                        sn.txt.Text = ni.content;
                        sn.glowColor = ni.ringColor;
                        N.NotificationHolder.Children.Add(sn);
                        N.ShowInTaskbar = false;
                        ImageBrush imgB = new ImageBrush();

                        try
                        {
                            string val = "";
                            imagesDictionary.TryGetValue(ni.nType, out val);
                            imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/" + val + ".png", UriKind.RelativeOrAbsolute));
                        }

                        catch (Exception ex)
                        {
                            imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/default.png", UriKind.RelativeOrAbsolute));
                            Console.WriteLine(ex.ToString());
                        }

                        sn.icon.Fill = imgB;

                        N.Pop(ni);
                    }));
                }
            }
            public void Detach()
            {
                // Detach the event and delete the list
                nq.Added -= new NotificationAddedEventHandler(NotifyNew);
                nq = null;
            }
        }
    }
    public class NotificationSender
    {
        string content;
        NotificationType nType;
        Color ringColor;

        public NotificationSender()
        {
            content = "Empty";
            nType = NotificationType.Default;
            initNotification();

        }
        void initNotification()
        {
            prepareNotification(new NotificationInfo("Empty", NotificationType.Default, Colors.WhiteSmoke));
            NotificationProvider.N.Dispatcher.Invoke(() => NotificationProvider.N.Hide());
        }

        void prepareNotification(NotificationInfo _n)
        {
            content = _n.content;
            nType = _n.nType;
            ringColor = _n.ringColor;
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

                catch (Exception e)
                {
                    imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/default.png", UriKind.RelativeOrAbsolute));
                    Console.WriteLine(e.ToString());
                }

                sn.icon.Fill = imgB;
            
                NotificationProvider.N.Pop(n);
            }));
        }
                
        public void sendNotification(string _content, NotificationType _nType, Color col)
        {
            var _n = new NotificationInfo(_content, _nType, col);
            if (NotificationProvider.NQ.Count >= 0) 
            {
                if(_n.content != NotificationProvider.lastNotification.content || _n.nType == NotificationType.Crystalbind)
                {
                    NotificationProvider.AddNotification(_n);
                }
            }
            else
            {
                NotificationProvider.AddNotification(_n);
            }
            NotificationProvider.lastNotification = _n;
        }

        public void sendNotification(string _content)
        {
            var _n = new NotificationInfo(_content, NotificationType.Default, Colors.WhiteSmoke);

            NotificationProvider.AddNotification(_n);

        }
    }
}

