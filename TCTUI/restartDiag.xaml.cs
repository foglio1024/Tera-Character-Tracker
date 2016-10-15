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
using System.Windows.Shapes;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per restartDiag.xaml
    /// </summary>
    public partial class restartDiag : UserControl
    {
        public restartDiag()
        {
            InitializeComponent();
        }
        DoubleAnimation d = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));

        private void saveAndClose(object sender, MouseButtonEventArgs e)
        {
            TeraLogic.SaveCharacters();
            TeraLogic.SaveAccounts();
            d.Completed += (o, s) => { Application.Current.Shutdown();   };
            this.BeginAnimation(OpacityProperty, d);
            Application.Current.MainWindow.BeginAnimation(OpacityProperty, d);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }
    }
}
