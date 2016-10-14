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

namespace Tera
{
    /// <summary>
    /// Logica di interazione per exitDiag.xaml
    /// </summary>
    public partial class exitDiag : Window
    {
        public exitDiag()
        {
            InitializeComponent();
            Result = false;
        }

        public bool Result { get; private set; }

     //   DoubleAnimation d = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));

        private void saveAndClose(object sender, MouseButtonEventArgs e)
        {
            //MainWindow.tct.saveCharsToXml(MainWindow.CharList);
            //d.Completed += (o, s) => Application.Current.Shutdown();
            //this.BeginAnimation(OpacityProperty,d);
            //Application.Current.MainWindow.BeginAnimation(OpacityProperty,d);
            Result = true;
            this.Close();
            
        }

        private void closeAndNoSave(object sender, MouseButtonEventArgs e)
        {
            //d.Completed += (o, s) => Application.Current.Shutdown();
            //this.BeginAnimation(OpacityProperty, d);
            //Application.Current.MainWindow.BeginAnimation(OpacityProperty, d);
            Result = false;
            this.Close();

        }
    }
}
