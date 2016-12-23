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
    public partial class Notification : Window
    {
        public Notification()
        {
            InitializeComponent();
            
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        NotificationInfo ni;
        public void Pop(NotificationInfo _n)
        {
            ni = _n;
            this.Dispatcher.Invoke(() =>
            {
                ThicknessAnimationUsingKeyFrames open = new ThicknessAnimationUsingKeyFrames();
                open.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0), TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
                NotificationHolder.BeginAnimation(Grid.MarginProperty, open);
                if (ni.Sound)
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Environment.CurrentDirectory + "\\content\\served.wav");
                    sp.Load();
                    sp.Play();
                }
            });
        }

        public void CloseAnim()
        {
            this.Dispatcher.Invoke(() =>
            {
            ThicknessAnimationUsingKeyFrames close = new ThicknessAnimationUsingKeyFrames();
            close.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(-200, 0, 0, 0), TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
            close.Completed += (s, o) =>
            {
                Hide();
                NotificationProvider.NQ.Remove(ni);
                NotificationProvider.NQ.SetBusyToFalseOnEnd();
            };

                NotificationHolder.BeginAnimation(Grid.MarginProperty, close);
            });

        }

    }

}
