using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCTData;

namespace TCTUI.Controls
{
    /// <summary>
    /// Logica di interazione per overviewPage2.xaml
    /// </summary>
    public partial class AccountContainer : UserControl
    {
        public AccountContainer()
        {
            InitializeComponent();
            var s = new Style { TargetType = typeof(TextBlock) };
            s.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));
            s.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.FontSizeProperty, 12.0));
            s.Setters.Add(new Setter(TextBlock.HeightProperty, 17.0));

            var d = new Style { TargetType = typeof(Border) };
            switch (TCTData.Settings.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    s.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Foreground2)));
                    d.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.LightTheme_Dividers)));

                    break;
                case TCTData.Enums.Theme.Dark:
                    s.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Foreground2)));
                    d.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(TCTData.Colors.DarkTheme_Dividers)));

                    break;
                default:
                    break;
            }

            headerGrid.Resources["headerTB"] = s;
            headerGrid.Resources["divider"] = d;
        }

        DoubleAnimation fadeIn = new DoubleAnimation(.5, TimeSpan.FromMilliseconds(150));
        DoubleAnimation fadeOut = new DoubleAnimation(.1, TimeSpan.FromMilliseconds(150));
        DoubleAnimation fadeOut0 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150));
        DoubleAnimation fadeIn1 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));
        DoubleAnimation expand = new DoubleAnimation(40, TimeSpan.FromMilliseconds(100));

        private void resetConfirmation(object sender, MouseButtonEventArgs e)
        {
            //if (dailiesConfPopup.IsOpen)
            //{
            //    fadeOut0.Completed += (s, ev) => dailiesConfPopup.IsOpen = false;
            //    dailiesConfPopup.Child.BeginAnimation(OpacityProperty, fadeOut0);
            //}
            //else
            //{
            //    dailiesConfPopup.IsOpen = true;

            //    dailiesConfPopup.Child.BeginAnimation(OpacityProperty, fadeIn1);


            //}
        }

        private void resetDailies(object sender, MouseButtonEventArgs e)
        {
            foreach (var character in Data.CharList)
            {
                //TO DO: confirmation popup
                character.Weekly = 0;
                //closeConfPopup(sender, e);

            }
        }

        private void opacityUp(object sender, MouseEventArgs e)
        {
            (sender as Image).BeginAnimation(OpacityProperty, fadeIn);
        }
        private void opacityDown(object sender, MouseEventArgs e)
        {
            (sender as Image).BeginAnimation(OpacityProperty, fadeOut);
        }
        private void rejectDrop(object sender, DragEventArgs e)
        {
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {

        }
    }
}