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
    /// Logica di interazione per BarGauge.xaml
    /// </summary>
    public partial class QuestGauge : UserControl
    {
        public QuestGauge()
        {
            InitializeComponent();
            var s = new Style { TargetType = typeof(TextBlock)};
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right ));
            s.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            s.Setters.Add(new Setter(WidthProperty, Double.NaN));
            s.Setters.Add(new Setter(FontWeightProperty, FontWeights.DemiBold));
            s.Setters.Add(new Setter(FontSizeProperty, 11.0));
            var s1 = new Style { TargetType = typeof(TextBlock), BasedOn = s };
            var s2 = new Style { TargetType = typeof(TextBlock), BasedOn = s };
            var s3 = new Style { TargetType = typeof(TextBlock), BasedOn = s };

            switch (TCTData.Settings.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));
                    break;
                case TCTData.Enums.Theme.Dark:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));

                    break;
                default:
                    break;
            }
            this.Resources["TB1"] = s1;
            this.Resources["TB2"] = s2;
            this.Resources["TB3"] = s3;

        }
    }
}
