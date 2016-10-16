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
            a1.KeyFrames.Add(new SplineDoubleKeyFrame(.1, TimeSpan.FromMilliseconds(100), new KeySpline(.5, 0, .3, 1)));
            a2.KeyFrames.Add(new SplineDoubleKeyFrame(0, TimeSpan.FromMilliseconds(100), new KeySpline(.5, 0, .3, 1)));
            m1.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(5,7,0,0), TimeSpan.FromMilliseconds(100), new KeySpline(.5, 0, .3, 1)));
            m2.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0,7,0,0), TimeSpan.FromMilliseconds(100), new KeySpline(.5, 0, .3, 1)));

           
        }

        DoubleAnimationUsingKeyFrames a1 = new DoubleAnimationUsingKeyFrames();
        DoubleAnimationUsingKeyFrames a2 = new DoubleAnimationUsingKeyFrames();
        ThicknessAnimationUsingKeyFrames m1 = new ThicknessAnimationUsingKeyFrames();
        ThicknessAnimationUsingKeyFrames m2 = new ThicknessAnimationUsingKeyFrames();


        private void gotKbFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void leftClick(object sender, MouseEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }
        private void leftClick(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }



        private void editCharName(object s, MouseButtonEventArgs e)
        {
            TextBox t = new TextBox();
            //TeraMainWindow.createBinding(TeraMainWindow.cvcp.getCharIndex(), "Name", t);
            var r = s as TextBlock;
            (r.Parent as Grid).Children.Add(t);
            Grid.SetColumn(t, 0);

            t.KeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Return || ev.Key == Key.Escape)
                {
                    (r.Parent as Grid).Children.Remove(t);
                }
            };

            t.Margin = r.Margin;
            t.Width = r.ActualWidth + 6;
            t.Height = r.ActualHeight + 0;
            t.HorizontalAlignment = r.HorizontalAlignment;
            t.VerticalAlignment = VerticalAlignment.Top;
            t.BorderThickness = new Thickness(0);
            t.VerticalContentAlignment = VerticalAlignment.Center;
            t.FontSize = r.FontSize;
            t.Foreground = r.Foreground;
            t.FontWeight = r.FontWeight;
            t.Focus();
            t.SelectionStart = t.Text.Length;
            t.SelectionLength = 0;

        }

        private void textBoxCheck(object sender)
        {
            /*checks if textboxes value exceedes max value and corrects it if it does*/
            int x = 0;

            if ((sender as TextBox).Name.Equals("dailiesTB"))
            {
                if ((sender as TextBox).Text != "" && int.TryParse(((sender as TextBox).Text), out x))
                {
                    if (Convert.ToInt32((sender as TextBox).Text) > TeraLogic.MAX_WEEKLY)
                    {
                        (sender as TextBox).Text = TeraLogic.MAX_WEEKLY.ToString();
                    }
                }
            }
            if ((sender as TextBox).Name.Equals("creditsTB"))
            {
                if ((sender as TextBox).Text != "" && int.TryParse(((sender as TextBox).Text), out x))
                {
                    if (Convert.ToInt32((sender as TextBox).Text) > TeraLogic.MAX_CREDITS)
                    {
                        (sender as TextBox).Text = TeraLogic.MAX_CREDITS.ToString();
                    }
                }
            }
            if ((sender as TextBox).Name.Equals("mvTB"))
            {
                if ((sender as TextBox).Text != "" && int.TryParse(((sender as TextBox).Text), out x))
                {
                    if (Convert.ToInt32((sender as TextBox).Text) > TeraLogic.MAX_MARKS)
                    {
                        (sender as TextBox).Text = TeraLogic.MAX_MARKS.ToString();
                    }
                }
            }
            if ((sender as TextBox).Name.Equals("gftTB"))
            {
                if ((sender as TextBox).Text != "" && int.TryParse(((sender as TextBox).Text), out x))
                {
                    if (Convert.ToInt32((sender as TextBox).Text) > TeraLogic.MAX_GF_TOKENS)
                    {
                        (sender as TextBox).Text = TeraLogic.MAX_GF_TOKENS.ToString();
                    }
                }
            }
        }
        private void isSavedToFalse(object sender, TextChangedEventArgs e)
        {
            /*sets unsaved state*/
            TeraLogic.IsSaved = false;

            /*runs check*/
            textBoxCheck(sender);


        }

    }
}
