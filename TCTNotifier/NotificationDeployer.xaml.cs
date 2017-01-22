using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCTNotifier
{
    
    /// <summary>
    /// Logica di interazione per Notification.xaml
    /// </summary>
    public partial class NotificationDeployer : Window
    {
        public static double StartingThickness { get; set; }
        public static double EndThickness { get; set; }
        public NotificationDeployer()
        {
            InitializeComponent();
            this.ShowActivated = false;     

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
        NotificationInfo ni;
        public void Pop(NotificationInfo Ni)
        {
            ni = Ni;

            if (ni.Right)
            {
                NotificationHolder.Margin = new Thickness { Left = 220, Top = 0, Right = 0, Bottom = 0 };
                Left = SystemParameters.FullPrimaryScreenWidth - 200;
                StartingThickness = 200;
                EndThickness = 0;
            }
            else
            {
                NotificationHolder.Margin = new Thickness { Left = -200, Top = 0, Right = 0, Bottom = 0 };
                Left = 0;
                StartingThickness = -200;
                EndThickness = 0;
            }

            this.Dispatcher.Invoke(() =>
            {
                ThicknessAnimationUsingKeyFrames open = new ThicknessAnimationUsingKeyFrames();
                open.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(EndThickness), TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
                NotificationHolder.BeginAnimation(Grid.MarginProperty, open);
                if (TCTData.TCTProps.NotificationSound)
                {
                    if (ni.Sound)
                    {
                        System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Environment.CurrentDirectory + "\\content\\served.wav");
                        sp.Load();
                        sp.Play();
                    }
                }
            });
        }



        public void CloseAnim()
        {
            this.Dispatcher.Invoke(() =>
            {
                ThicknessAnimationUsingKeyFrames close = new ThicknessAnimationUsingKeyFrames();
                close.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(StartingThickness,0,0,0), TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
                close.Completed += (s, o) =>
                {
                    NotificationHolder.BeginAnimation(Grid.MarginProperty, null);
                    Hide();
                    ni.SetDelivered();
                    NotificationProvider.deliveredNotifications.Add(ni);
                    NotificationProvider.NQ.Remove(ni);
                    NotificationProvider.NQ.SetBusyToFalseOnEnd();
                };

                NotificationHolder.BeginAnimation(Grid.MarginProperty, close);
            });

        }

    }

}
