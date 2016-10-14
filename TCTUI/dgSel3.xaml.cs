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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per dgSel.xaml
    /// </summary>
    public partial class dgSel3 : UserControl
    {
        public dgSel3(string _name, string _tag)
        {
            InitializeComponent();
            this.Name = _name;
            this.Tag = _tag;
        }

        private void addDungCount(object sender, MouseButtonEventArgs e)
        {
            var charName = ((Grid)this.Parent).Tag as string;
            var charIndex = TeraMainWindow.CharList.IndexOf(TeraMainWindow.CharList.Find(x => x.Name.Equals(charName)));

            var dgName = this.Tag as string;
            var dgIndex = TeraMainWindow.CharList[charIndex].Dungeons.IndexOf(TeraMainWindow.CharList[charIndex].Dungeons.Find(x => x.Name.Equals(dgName)));
            if (TeraMainWindow.CharList[charIndex].Dungeons[dgIndex].Runs < 3)
            {
                TeraMainWindow.CharList[charIndex].Dungeons[dgIndex].Runs++;
            }
            else
            {
                TeraMainWindow.CharList[charIndex].Dungeons[dgIndex].Runs=0;
            }
        }

      
    }
}
