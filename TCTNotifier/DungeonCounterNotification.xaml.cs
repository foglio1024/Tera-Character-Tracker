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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCTNotifier
{
    /// <summary>
    /// Logica di interazione per DungeonCounterNotification.xaml
    /// </summary>
    public partial class DungeonCounterNotification : UserControl
    {
        public DungeonCounterNotification()
        {
            InitializeComponent();
        }
        private int notificationTime = 1800 * 3; // notification time in milliseconds
        static System.Timers.Timer OpeningTimer;
        static System.Timers.Timer timer;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OpeningTimer = new Timer(350);
            OpeningTimer.Elapsed += new ElapsedEventHandler(NotificationWait);
            OpeningTimer.Enabled = true;
        }

        void NotificationWait(object sender, ElapsedEventArgs ev)
        {
            OpeningTimer.Stop();
            timer = new System.Timers.Timer(notificationTime);
            timer.Elapsed += new ElapsedEventHandler(NotificationEnded);
            timer.Enabled = true;

        }

        private void NotificationEnded(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            NotificationProvider.NotificationDeployer.CloseAnim();
        }
    }
}
