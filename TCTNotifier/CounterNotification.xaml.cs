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
    public partial class CounterNotification : UserControl
    {

        public CounterNotification()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OpeningTimer = new Timer(3350);
            OpeningTimer.Elapsed += new ElapsedEventHandler(EndNotification);
            OpeningTimer.Enabled = true;
        }
        
        private void EndNotification(object sender, ElapsedEventArgs ev)
        {
            OpeningTimer.Stop();
            NotificationProvider.NotificationDeployer.CloseAnim();
        }

        static System.Timers.Timer OpeningTimer;
        
    }
}
