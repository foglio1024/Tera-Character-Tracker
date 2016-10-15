using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Logica di interazione per classSelMenu.xaml
    /// </summary>
    public partial class classSelMenu : UserControl
    {
        public classSelMenu()
        {
            InitializeComponent();

        }
        private void rowHighlight(object sender, MouseEventArgs e)
        {
            var s = sender as Grid;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(0, 0, 0, 0);
            an.To = Color.FromArgb(30, 155, 155, 155);
            an.Duration = TimeSpan.FromMilliseconds(200);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
        }

        private void rowNormal(object sender, MouseEventArgs e)
        {
            var s = sender as Grid;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(30, 155, 155, 155);
            an.To = Color.FromArgb(0, 0, 0, 0);
            an.Duration = TimeSpan.FromMilliseconds(100);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
        }

        private void classSelected(object sender, MouseButtonEventArgs e)
        {

            //TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(c => c.Name.Equals(TeraMainWindow.activeChar)))].CharClass=(sender as Grid).Tag.ToString();
            TeraLogic.IsSaved = false;

        }
        private void laurelSelected(object sender, MouseButtonEventArgs e)
        {

            //TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(c => c.Name.Equals(TeraMainWindow.activeChar)))].Laurel = (sender as Grid).Tag.ToString();
            TeraLogic.IsSaved = false;

        }
    }

}
