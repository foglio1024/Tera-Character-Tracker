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
using TCTData.Enums;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per dgCounter.xaml
    /// </summary>
    public partial class DungeonRunsCounter : UserControl
    {
        public DungeonRunsCounter()
        {
            InitializeComponent();
        }

        private void subtractDungRun(object sender, MouseButtonEventArgs e)
        {
            if (TeraLogic.cvcp.SelectedChar != null)
            {
                int tc = 1;
                if (Properties.Settings.Default.TeraClub)
                {
                    tc = 2;
                }
                var charName = TeraLogic.cvcp.SelectedChar.Name;
                var charIndex = TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(charName)));

                var dgName = this.Tag as string;
                var dgIndex = TeraLogic.CharList[charIndex].Dungeons.IndexOf(TeraLogic.CharList[charIndex].Dungeons.Find(x => x.Name.Equals(dgName)));
                if(dgName == "EA" || dgName == "CA" || dgName == "AH" || dgName == "GL")
                {
                    if (TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs > 0)
                    {
                        TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs--;
                    }
                    else
                    {
                        TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs = TeraLogic.DungList[dgIndex].MaxBaseRuns;
                    }
                }
                else
                {
                    if (TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs > 0)
                    {
                        TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs--;
                    }
                    else
                    {
                        TeraLogic.CharList[charIndex].Dungeons[dgIndex].Runs = TeraLogic.DungList[dgIndex].MaxBaseRuns * tc;
                    }
                }
          
                TeraLogic.IsSaved = false; 
            }

        }
        public void SetGquestStatus(GuildQuestStatus status)
        {
            switch (status)
            {
                case GuildQuestStatus.Available:
                    gQuestLed.Fill = new SolidColorBrush(UI.Colors.FadedBaseColor);
                    gQuestLed.Opacity = 1;
                    break;

                case GuildQuestStatus.Taken:
                    gQuestLed.Fill = new SolidColorBrush(UI.Colors.SolidYellow);
                    gQuestLed.Opacity = 1;
                    break;

                case GuildQuestStatus.Completed:
                    gQuestLed.Fill = new SolidColorBrush(UI.Colors.SolidGreen);
                    gQuestLed.Opacity = 1;
                    break;

                case GuildQuestStatus.NotFound:
                    gQuestLed.Opacity = 0;
                    break;

                default:
                    break;
            }
        }
    }
}
