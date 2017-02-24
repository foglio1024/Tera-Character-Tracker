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
using TCTUI;

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
            var s1 = new Style { TargetType = typeof(TextBlock) };
            var s2 = new Style { TargetType = typeof(TextBlock) };
            var s3 = new Style { TargetType = typeof(TextBlock) };
            var so1 = new Style { TargetType = typeof(TextBlock) };
            var so2 = new Style { TargetType = typeof(TextBlock) };
            var so3 = new Style { TargetType = typeof(TextBlock) };

            var e = new Style { TargetType = typeof(Ellipse) };

            switch (TCTData.TCTProps.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));
                    so1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    so2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    so3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));

                    e.Setters.Add(new Setter(Shape.StrokeProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Card)));
                    break;
                case TCTData.Enums.Theme.Dark:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));
                    so1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    so2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    so3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));

                    e.Setters.Add(new Setter(Shape.StrokeProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Card)));

                    break;
                default:
                    break;
            }
            this.Resources["TB1"] = s1;
            this.Resources["TB2"] = s2;
            this.Resources["TB3"] = s3;
            this.Resources["TBo1"] = so1;
            this.Resources["TBo2"] = so2;
            this.Resources["TBo3"] = so3;

            this.Resources["led"] = e;
        }

        private void subtractDungRun(object sender, MouseButtonEventArgs e)
        {
            if (UI.SelectedChar != null)
            {
                int tc = 1;
                if (Properties.Settings.Default.TeraClub)
                {
                    tc = 2;
                }
                var charName = UI.SelectedChar.Name;
                var charIndex = TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(charName)));

                var dgName = n.Text;
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
                    gQuestLed.Fill = new SolidColorBrush(TCTData.Colors.SolidBaseColor);
                    gQuestLed.Opacity = 1;
                    break;

                case GuildQuestStatus.Taken:
                    gQuestLed.Fill = new SolidColorBrush(TCTData.Colors.SolidYellow);
                    gQuestLed.Opacity = 1;
                    break;

                case GuildQuestStatus.Completed:
                    gQuestLed.Fill = new SolidColorBrush(TCTData.Colors.SolidGreen);
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
