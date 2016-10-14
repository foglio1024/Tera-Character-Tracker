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
    public partial class dgSelNew : UserControl
    {
        public dgSelNew()
        {
            InitializeComponent();
        }


        private void addDungCount(object sender, MouseButtonEventArgs e)
        {
            var charName = ((StackPanel)this.Parent).Tag as string;
            var charIndex = TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(charName)));

            var dgName = this.Tag as string;
            var dgIndex = TeraLogic.CharList[charIndex].Dungeons.IndexOf(TeraLogic.CharList[charIndex].Dungeons.Find(x => x.Name.Equals(dgName)));
            if (TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs < TeraLogic.DungList[dgIndex].MaxBaseRuns)
            {
                TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs++;
            }
            else
            {
                TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs = 0;
            }
        }

    }
}
