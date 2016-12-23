using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TCTNotifier
{
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
            if (nq.Count > 0)
            {
                NotifyNew(new object(), new EventArgs(), nq.First());
            }
        }
        private void NotifyNew(object sender, EventArgs e, NotificationInfo ni)
        {
            if (!nq.Busy)
            {
                nq.SetBusy();
                N.Dispatcher.Invoke(() =>
                {
                    N.NotificationHolder.Children.Clear();
                    N.Show();
                });

                N.Dispatcher.Invoke(new Action(() =>
                {
                    var sn = new StandardNotification();
                    sn.txt.Text = ni.Content;
                    sn.glowColor = ni.Color;
                    N.NotificationHolder.Children.Add(sn);
                    N.ShowInTaskbar = false;
                    ImageBrush imgB = new ImageBrush();

                    try
                    {
                        string val = "";
                        imagesDictionary.TryGetValue(ni.Image, out val);
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
