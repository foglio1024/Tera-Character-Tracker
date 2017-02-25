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

namespace TCTUI.Controls
{
    /// <summary>
    /// Logica di interazione per DungeonClearsCounter.xaml
    /// </summary>
    public partial class DungeonClearsCounter : UserControl
    {
        public DungeonClearsCounter()
        {
            InitializeComponent();
            var s1 = new Style { TargetType = typeof(TextBlock) };
            var s2 = new Style { TargetType = typeof(TextBlock) };
            var s3 = new Style { TargetType = typeof(TextBlock) };
            var so1 = new Style { TargetType = typeof(TextBlock) };
            var so2 = new Style { TargetType = typeof(TextBlock) };
            var so3 = new Style { TargetType = typeof(TextBlock) };

            switch (TCTData.Settings.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));
                    so1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    so2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    so3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));

                    break;
                case TCTData.Enums.Theme.Dark:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));
                    so1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    so2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    so3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));

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

        }
    }
}
