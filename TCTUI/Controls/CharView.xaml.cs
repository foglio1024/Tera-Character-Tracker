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
    /// Logica di interazione per CharView.xaml
    /// </summary>
    public partial class CharView : UserControl
    {
        public CharView()
        {
           
            InitializeComponent();

            var d = new Style { TargetType = typeof(Border) };
            var s1 = new Style { TargetType = typeof(TextBlock) };
            var s2 = new Style { TargetType = typeof(TextBlock) };
            var s3 = new Style { TargetType = typeof(TextBlock) };
            var sx = new Style { TargetType = typeof(TextBox) };

            switch (TCTData.TCTProps.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    //Add setters
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));
                    sx.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    sx.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Dividers)));
                    d.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Dividers)));
                    //Set colors and shadows
                    newDgPanel.Background = new SolidColorBrush(TCTData.Colors.LightTheme_Card);
                    animGrid.Background = new SolidColorBrush(TCTData.Colors.LightTheme_Card);
                    newDgPanel.Effect = TCTData.Shadows.LightThemeShadow;
                    animGrid.Effect = TCTData.Shadows.LightThemeShadow;
                    break;

                case TCTData.Enums.Theme.Dark:
                    //Add setters
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));
                    sx.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    sx.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Dividers)));
                    d.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Dividers)));
                    //Set colors and shadows
                    newDgPanel.Background = new SolidColorBrush(TCTData.Colors.DarkTheme_Card);
                    animGrid.Background = new SolidColorBrush(TCTData.Colors.DarkTheme_Card);
                    newDgPanel.Effect = TCTData.Shadows.DarkThemeShadow;
                    animGrid.Effect = TCTData.Shadows.DarkThemeShadow;
                    break;

                default:
                    break;
            }

            Resources["TB1"] = s1;
            Resources["TB2"] = s2;
            Resources["TB3"] = s3;
            Resources["TBx"] = sx;
            Resources["divider"] = d;
        }
    }
}
