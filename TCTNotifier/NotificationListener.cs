using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TCTNotifier
{
    public class NotificationListener
    {
        private NotificationQueue queue;
        public NotificationListener(NotificationQueue _nq)
        {
            queue = _nq;
            queue.Busy = false;
            queue.Added += new NotificationAddedEventHandler(DeliverNotification    /*NotifyNew*/);
            queue.Over += new NotificationEndedEventHandler(DeliverNext /*NextNotification*/);
        }

        void DeliverNotification(object s, EventArgs e, NotificationInfo n)
        {     
            if(NotificationProvider.deliveredNotifications.Find(x => x.Content == n.Content) == null || n.IsRepeatable)
            {
                if (!queue.Busy)
                {
                    queue.SetBusy();
                    NotificationProvider.NotificationDeployer.Dispatcher.Invoke(() =>
                    {
                        NotificationProvider.NotificationDeployer.NotificationHolder.Children.Clear();
                        NotificationProvider.NotificationDeployer.Show();

                        ImageBrush imgB = new ImageBrush();

                        try
                        {
                            string val = "";
                            NotificationProvider.imagesDictionary.TryGetValue(n.Image, out val);
                            imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/" + val + ".png", UriKind.RelativeOrAbsolute));
                        }

                        catch (Exception ex)
                        {
                            imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/default.png", UriKind.RelativeOrAbsolute));
                            Console.WriteLine(ex.ToString());

                        }


                        switch (n.Type)
                        {
                            case TCTData.Enums.NotificationType.Standard:
                                var sn = new StandardNotification()
                                {
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new System.Windows.Thickness(0, NotificationProvider.VerticalOffset, 0, 0)
                                };
                                sn.txt.Text = n.Content;
                                sn.glowColor = n.Color;
                                sn.icon.Fill = imgB;
                                NotificationProvider.NotificationDeployer.NotificationHolder.Children.Add(sn);
                                break;

                            case TCTData.Enums.NotificationType.Counter:
                                var cn = new CounterNotification()
                                {
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new System.Windows.Thickness(0, NotificationProvider.VerticalOffset, 0, 0)
                                };
                                cn.amountTB.Text = n.Content;
                                //cn.icon.Stroke = new SolidColorBrush(n.Color);
                                cn.icon.Fill = imgB;
                                cn.border.BorderBrush = new SolidColorBrush(n.Color);
                                NotificationProvider.NotificationDeployer.NotificationHolder.Children.Add(cn);
                                break;

                            default:
                                break;
                        }

                        NotificationProvider.NotificationDeployer.ShowInTaskbar = false;
                        NotificationProvider.NotificationDeployer.Pop(n);
                    });
                }

            }


        }

        void DeliverNext(object s, EventArgs e)
        {
            if (queue.Count > 0)
            {
                DeliverNotification(new object(), new EventArgs(), queue.First());
            }
        }





        //private void NextNotification(object sender, EventArgs e)
        //{
        //    if (queue.Count > 0)
        //    {
        //        NotifyNew(new object(), new EventArgs(), queue.First());
        //    }
        //}
        //private void NotifyNew(object sender, EventArgs e, NotificationInfo ni)
        //{
        //    if (!queue.Busy)
        //    {
        //        queue.SetBusy();
        //        N.Dispatcher.Invoke(() =>
        //        {
        //            N.NotificationHolder.Children.Clear();
        //            N.Show();
        //        });

        //        N.Dispatcher.Invoke(new Action(() =>
        //        {
        //            var sn = new StandardNotification();
        //            sn.Margin = new System.Windows.Thickness(-200, 0, 0, 0);
        //            sn.txt.Text = ni.Content;
        //            sn.glowColor = ni.Color;
        //            N.NotificationHolder.Children.Add(sn);
        //            N.ShowInTaskbar = false;
        //            ImageBrush imgB = new ImageBrush();

        //            try
        //            {
        //                string val = "";
        //                imagesDictionary.TryGetValue(ni.Image, out val);
        //                imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/" + val + ".png", UriKind.RelativeOrAbsolute));
        //            }

        //            catch (Exception ex)
        //            {
        //                imgB.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/notifier_images/default.png", UriKind.RelativeOrAbsolute));
        //                Console.WriteLine(ex.ToString());

        //            }

        //            sn.icon.Fill = imgB;

        //            N.Pop(ni);
        //        }));
        //    }
        //}
        public void Detach()
        {
            // Detach the event and delete the list
            queue.Added -= new NotificationAddedEventHandler(DeliverNotification);
            queue = null;
        }
    }
}
