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
    /// Logica di interazione per MaterialSwitch.xaml
    /// </summary>
    public partial class MaterialSwitch : UserControl
    {
        public MaterialSwitch()
        {
            InitializeComponent();

            On = new ThicknessAnimationUsingKeyFrames();
            Off = new ThicknessAnimationUsingKeyFrames();
            OnFill = new ColorAnimation(Colors.White, UI.Colors.SolidAccentColor, TimeSpan.FromMilliseconds(150));
            OffFill = new ColorAnimation(UI.Colors.SolidAccentColor, Colors.White, TimeSpan.FromMilliseconds(150));
            OnBackFill = new ColorAnimation(UI.Colors.FadedGray, UI.Colors.FadedAccentColor, TimeSpan.FromMilliseconds(150));
            OffBackFill = new ColorAnimation(UI.Colors.FadedAccentColor, UI.Colors.FadedGray, TimeSpan.FromMilliseconds(150));
            On.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(20, 0, 0, 0), TimeSpan.FromMilliseconds(220), new KeySpline(.5, 0, .3, 1)));
            Off.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(-20, 0, 0, 0), TimeSpan.FromMilliseconds(220), new KeySpline(.5, 0, .3, 1)));

        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(bool), typeof(MaterialSwitch), new PropertyMetadata(false));
        public bool Status
        {
            get { return (bool)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public void TurnOn()
        {
            switchHead.BeginAnimation(MarginProperty, On);
            switchHead.Fill.BeginAnimation(SolidColorBrush.ColorProperty, OnFill);
            switchBack.Fill.BeginAnimation(SolidColorBrush.ColorProperty, OnBackFill);
            Status = true;
        }
        public void TurnOff()
        {
            switchHead.BeginAnimation(MarginProperty, Off);
            switchHead.Fill.BeginAnimation(SolidColorBrush.ColorProperty, OffFill);
            switchBack.Fill.BeginAnimation(SolidColorBrush.ColorProperty, OffBackFill);
            Status = false;
        }

        ThicknessAnimationUsingKeyFrames On;
        ThicknessAnimationUsingKeyFrames Off;
        ColorAnimation OnFill;
        ColorAnimation OffFill;
        ColorAnimation OnBackFill;
        ColorAnimation OffBackFill;

        private void SwitchPressed(object sender, MouseButtonEventArgs ev)
        {
            if (Status)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }

        }
    }
}
