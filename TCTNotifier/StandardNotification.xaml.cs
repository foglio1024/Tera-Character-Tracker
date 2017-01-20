using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCTNotifier
{
    /// <summary>
    /// Logica di interazione per StandardNotification.xaml
    /// </summary>
    public partial class StandardNotification : UserControl
    {
        public StandardNotification()
        {
            InitializeComponent();
        }

        private int notificationTime = 1800*3; // notification time in milliseconds
        public Color glowColor;
        static System.Timers.Timer OpeningTimer;
        static System.Timers.Timer timer;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OpeningTimer = new Timer(350);
            OpeningTimer.Elapsed += new ElapsedEventHandler(SweepArc);
            OpeningTimer.Enabled = true;
        }
        void SweepEnded(object sender, ElapsedEventArgs ev)
        {
            timer.Stop();
            NotificationProvider.NotificationDeployer.CloseAnim();
            Dispatcher.Invoke(() =>
            {
                arc.BeginAnimation(Arc.EndAngleProperty, null);
            });
        }
        void SweepArc(object sender, ElapsedEventArgs ev)
        {
            OpeningTimer.Stop();
            timer = new System.Timers.Timer(notificationTime);
            timer.Elapsed += new ElapsedEventHandler(SweepEnded);

            Dispatcher.Invoke(() =>
            {
                arc.Stroke = new SolidColorBrush(glowColor);
                border.BorderBrush = new SolidColorBrush(glowColor);

                DoubleAnimationUsingKeyFrames end = new DoubleAnimationUsingKeyFrames();
                end.KeyFrames.Add(new SplineDoubleKeyFrame(359, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new KeySpline(.5, 0, .3, 1)));
                end.FillBehavior = FillBehavior.HoldEnd;
                DoubleAnimationUsingKeyFrames start = new DoubleAnimationUsingKeyFrames();
                start.KeyFrames.Add(new SplineDoubleKeyFrame(359, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.8)), new KeySpline(.5, 0, .3, 1)));
                start.FillBehavior = FillBehavior.Stop;



                end.Completed += (s, o) => 
                {
                    arc.BeginAnimation(Arc.StartAngleProperty, start);
                };

                start.Completed += (s, o) =>
                {
                    arc.BeginAnimation(Arc.EndAngleProperty, null);
                    arc.BeginAnimation(Arc.EndAngleProperty, end);
                };


                timer.Enabled = true;

                arc.BeginAnimation(Arc.EndAngleProperty, end);
            });

        }

        //public void GlowRing()
        //{
        //    timer = new System.Timers.Timer(notificationTime);
        //    timer.Elapsed += new ElapsedEventHandler(GlowEnded);
        //    double da = glowColor.A;
        //    da = da * .2;
        //    var a = Convert.ToByte(da);
        //    darkColor = new Color { A = a, R = glowColor.R, G = glowColor.G, B = glowColor.B };

        //    ColorAnimation unglow = new ColorAnimation(darkColor, new Duration(TimeSpan.FromMilliseconds(800)));
        //    ColorAnimation glow = new ColorAnimation(glowColor, new Duration(TimeSpan.FromMilliseconds(100)));

        //    //ColorAnimation ca1 = new ColorAnimation(glowColor, new Duration(TimeSpan.FromMilliseconds(300)));
        //    //ColorAnimation ca2 = new ColorAnimation(darkColor, new Duration(TimeSpan.FromMilliseconds(1200)));
        //    //ColorAnimation ca3 = new ColorAnimation(glowColor, new Duration(TimeSpan.FromMilliseconds(1300)));
        //    //ColorAnimation ca4 = new ColorAnimation(darkColor, new Duration(TimeSpan.FromMilliseconds(1200)));

        //    icon.Stroke = new SolidColorBrush(glowColor);
        //    //glowRect.Fill = new SolidColorBrush(darkColor);
        //    border.BorderBrush = new SolidColorBrush(darkColor);

        //    glow.Completed += (s0, o0) =>
        //    {
        //        //glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, unglow);
        //        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, unglow);
        //        icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, unglow);
        //    };

        //    unglow.Completed += (s1, o1) =>
        //    {
        //        //glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, glow);
        //        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, glow);
        //        icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, glow);
        //    };

        //    timer.Enabled = true;
        //    //glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, glow);
        //    border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, glow);
        //    icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, glow);


        //    //ca1.Completed += (s, o) =>
        //    //{
        //    //    icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
        //    //    glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
        //    //    border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
        //    //};
        //    //ca2.Completed += (s, o) =>
        //    //{
        //    //    icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca3);
        //    //    glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ca3);
        //    //    border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca3);

        //    //};
        //    //ca3.Completed += (s, o) =>
        //    //{
        //    //    icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca4);
        //    //    glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ca4);
        //    //    border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca2);

        //    //};
        //    //ca4.Completed += (s, o) =>
        //    //{
        //    //    GlowEnded();
        //    //};
        //    //icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca1);
        //    //glowRect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ca1);
        //    //border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca1);



        //}
        //void GlowEnded(object sender, ElapsedEventArgs ev)
        //{
        //    timer.Stop();
        //    NotificationProvider.N.CloseAnim();
        //    Dispatcher.Invoke(() =>
        //    {
        //        icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, null);
        //        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
        //    });
        //}

    }
}
