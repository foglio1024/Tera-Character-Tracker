using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private int notificaitonTime = 4000; // notification time in milliseconds
        public Color glowColor;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GlowRing();
        }

        void GlowEnded()
        {
            NotificationProvider.N.CloseAnim();
        }

        public void GlowRing()
        {
            double da = glowColor.A;
            da = da * .2;
            var a = Convert.ToByte(da);
            Color darkColor = new Color { A = a, R = glowColor.R, G = glowColor.G, B = glowColor.B };
            ColorAnimation ca1 = new ColorAnimation(glowColor, new Duration(TimeSpan.FromMilliseconds(300)));
            ColorAnimation ca2 = new ColorAnimation(darkColor, new Duration(TimeSpan.FromMilliseconds(1200)));
            ColorAnimation ca3 = new ColorAnimation(glowColor, new Duration(TimeSpan.FromMilliseconds(1300)));
            ColorAnimation ca4 = new ColorAnimation(darkColor, new Duration(TimeSpan.FromMilliseconds(1200)));

            icon.Stroke = new SolidColorBrush(glowColor);

            ca1.Completed += (s, o) =>
            {
                icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
                border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
            };
            ca2.Completed += (s, o) =>
            {
                icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca3);
                border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca3);
            };
            ca3.Completed += (s, o) =>
            {
                icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca4);
                border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca4);
            };
            ca4.Completed += (s, o) =>
            {
                GlowEnded();
            };
            icon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, ca1);
            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca1);


        }

    }
}
