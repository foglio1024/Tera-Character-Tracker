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
using TCTUI;

namespace Tera.Controls
{
    /// <summary>
    /// Logica di interazione per ExtendedCharacterStrip.xaml
    /// </summary>
    public partial class ExtendedCharacterStrip : UserControl
    {
        public ExtendedCharacterStrip()
        {
            InitializeComponent();
            var d = new Style { TargetType = typeof(Border) };
            var s1 = new Style { TargetType = typeof(TextBlock) };
            var s2 = new Style { TargetType = typeof(TextBlock) };
            var s3 = new Style { TargetType = typeof(TextBlock) };
            var i = new Style { TargetType = typeof(Rectangle) };
            switch (TCTData.TCTProps.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));
                    d.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Dividers)));
                    i.Setters.Add(new Setter(Shape.FillProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground3)));
                    break;
                case TCTData.Enums.Theme.Dark:
                    s1.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground1)));
                    s2.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    s3.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));
                    d.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Dividers)));
                    i.Setters.Add(new Setter(Shape.FillProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground3)));
                    break;
                default:
                    break;
            }
            Resources["stripTB1"] = s1;
            Resources["stripTB2"] = s2;
            Resources["stripTB3"] = s3;
            Resources["divider"] = d;
            Resources["classImg"] = i;
        }

        DoubleAnimationUsingKeyFrames sizeInH = new DoubleAnimationUsingKeyFrames();
        DoubleAnimationUsingKeyFrames sizeInW = new DoubleAnimationUsingKeyFrames();

        DoubleAnimationUsingKeyFrames sizeOut = new DoubleAnimationUsingKeyFrames();

        private void selectChar(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TeraLogic.SelectCharacter(this.Tag.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            sizeInH.KeyFrames.Add(new SplineDoubleKeyFrame(260, TimeSpan.FromMilliseconds(350), new KeySpline(.5, 0, .3, 1)));
            sizeInW.KeyFrames.Add(new SplineDoubleKeyFrame(350, TimeSpan.FromMilliseconds(350), new KeySpline(.5, 0, .3, 1)));
            sizeOut.KeyFrames.Add(new SplineDoubleKeyFrame(1, TimeSpan.FromMilliseconds(350), new KeySpline(.5, 0, .3, 1)));

        }
        private void rowNormal(object sender, MouseEventArgs e)
        {
            // if (classSelPopup.IsOpen == false)
            // {
            var s = sender as ExtendedCharacterStrip;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(30, 155, 155, 155);
            an.To = Color.FromArgb(0, 0, 0, 0);
            an.Duration = TimeSpan.FromMilliseconds(20);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
            // }
        }
        public void rowHighlight(object sender, MouseEventArgs e)
        {
            //var s = sender as ExtendedCharacterStrip;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(0, 0, 0, 0);
            an.To = Color.FromArgb(30, 155, 155, 155);
            an.Duration = TimeSpan.FromMilliseconds(0);
            this.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
        }
        public void rowSelect(bool state)
        {
            Color col;

            var c = TCTData.Colors.FadedBaseColor;

            if (state)
            {
                col = new Color { A = 20, R = c.R, G = c.G, B = c.B };
            }
            else
            {
                
                col = new Color { A = 0, R = 0, G = 0, B = 0};
            }

            select.Fill = new SolidColorBrush(col);
        }

    }
}
