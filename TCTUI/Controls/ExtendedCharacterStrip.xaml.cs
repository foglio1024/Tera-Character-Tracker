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
