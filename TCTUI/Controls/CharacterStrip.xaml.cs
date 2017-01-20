using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tera.Converters;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per newStrip.xaml
    /// </summary>
    public partial class CharacterStrip : UserControl
    {

        public CharacterStrip()
        {
            InitializeComponent();
        }

        public static bool _drag;
        public static bool _isDown;
        public   TeraMainWindow _visual;
        public UIElement _draggedItem;
        public CharacterStrip item2;
        
        DoubleAnimation fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(120));
        DoubleAnimation fadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(120));
        DoubleAnimation fadeOutL = new DoubleAnimation(0, TimeSpan.FromMilliseconds(120));
        DoubleAnimation fadeInL = new DoubleAnimation(1, TimeSpan.FromMilliseconds(120));

        DoubleAnimation fadeIn_03 = new DoubleAnimation(.4, TimeSpan.FromMilliseconds(70));
        DoubleAnimation shrink = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));
        DoubleAnimation expand = new DoubleAnimation(40, TimeSpan.FromMilliseconds(100));


        DoubleAnimationUsingKeyFrames sizeInH = new DoubleAnimationUsingKeyFrames();
        DoubleAnimationUsingKeyFrames sizeInW = new DoubleAnimationUsingKeyFrames();

        DoubleAnimationUsingKeyFrames sizeOut = new DoubleAnimationUsingKeyFrames();

        DoubleAnimationUsingKeyFrames barExp = new DoubleAnimationUsingKeyFrames();
        DoubleAnimationUsingKeyFrames barShr = new DoubleAnimationUsingKeyFrames();

        public void rowHighlight(object sender, MouseEventArgs e)
        {
            var s = sender as CharacterStrip;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(0, 0, 0, 0);
            an.To = Color.FromArgb(30, 155, 155, 155);
            an.Duration = TimeSpan.FromMilliseconds(0);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
            showArrow(s, e);
            showDel(s, e);
        }

        private void rowNormal(object sender, MouseEventArgs e)
        {
           // if (classSelPopup.IsOpen == false)
           // {
            var s = sender as CharacterStrip;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(30, 155, 155, 155);
            an.To = Color.FromArgb(0, 0, 0, 0);
            an.Duration = TimeSpan.FromMilliseconds(90);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an); 
            hideArrow(s, e);
            hideDel(s, e);
           // }
        }

        public void rowSelect(bool state)
        {
            Color col;

            if (state)
            {
                col = new Color { A = 10, R = 0, G = 0, B = 0 };
            }
            else
            {
                col = new Color { A = 0, R = 230, G = 245, B = 255 };
            }

            select.Fill = new SolidColorBrush(col);
        }
        private void classSelShowMenu(object sender, MouseEventArgs e)
        {

            //if (classSelPopup.IsOpen)
            //{
            //    fadeOut.Completed += (s, ev) =>
            //    {
            //        selMenu.Opacity = 0;
            //        classSelPopup.IsOpen = false;
            //    };

            //    selMenu.BeginAnimation(OpacityProperty, fadeOut);
            //    selMenu.BeginAnimation(HeightProperty, sizeOut);
            //    selMenu.BeginAnimation(WidthProperty, sizeOut);
            //}
            //else
            //{
            //    classSelPopup.PlacementTarget = className;
            //    classSelPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Left;
                
            //    TeraMainWindow.activeChar = Convert.ToString((sender as Image).Tag);
            //    classSelPopup.IsOpen = true;
            //    selMenu.Height = 1;
            //    selMenu.Width = 1;

            //    selMenu.BeginAnimation(OpacityProperty, fadeIn);
            //    selMenu.BeginAnimation(HeightProperty, sizeInH);
            //    selMenu.BeginAnimation(WidthProperty, sizeInW);

            //}

        }
        private void closeOnMouseLeave(object sender, MouseEventArgs e)
        {
            fadeOut.Completed += (se, ev) =>
            {
                (sender as Popup).Child.Opacity = 0;
                (sender as Popup).IsOpen = false;
            };
            (sender as Popup).Child.BeginAnimation(OpacityProperty, fadeOut);
            (sender as Popup).Child.BeginAnimation(HeightProperty, sizeOut);
            (sender as Popup).Child.BeginAnimation(WidthProperty, sizeOut);
            //editClass.BeginAnimation(OpacityProperty, fadeOut);
            var s = this;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(30, 155, 155, 155);
            an.To = Color.FromArgb(0, 0, 0, 0);
            an.Duration = TimeSpan.FromMilliseconds(75);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
            hideArrow(s, e);
            hideDel(s, e);

        }
        private void closeOnLaurelSelected(object sender, MouseButtonEventArgs e)
        {
            fadeOutL.Completed += (s, ev) =>
            {
                (sender as Popup).Child.Opacity = 0;
                (sender as Popup).IsOpen = false;
            };
            (sender as Popup).Child.BeginAnimation(OpacityProperty, fadeOutL);
            (sender as Popup).Child.BeginAnimation(HeightProperty, sizeOut);
            (sender as Popup).Child.BeginAnimation(WidthProperty, sizeOut);
        }
        private void closeOnClassSelected(object sender, MouseButtonEventArgs e)
        {
            fadeOut.Completed += (se, ev) =>
            {
                (sender as Popup).Child.Opacity = 0;
                (sender as Popup).IsOpen = false;
            };
            (sender as Popup).Child.BeginAnimation(OpacityProperty, fadeOut);
            (sender as Popup).Child.BeginAnimation(HeightProperty, sizeOut);
            (sender as Popup).Child.BeginAnimation(WidthProperty, sizeOut);
            //editClass.BeginAnimation(OpacityProperty, fadeOut);
            var s = this;
            var an = new ColorAnimation();
            an.From = Color.FromArgb(30, 155, 155, 155);
            an.To = Color.FromArgb(0, 0, 0, 0);
            an.Duration = TimeSpan.FromMilliseconds(75);
            s.Background.BeginAnimation(SolidColorBrush.ColorProperty, an);
            hideArrow(s, e);
            hideDel(s, e);
        }
        private void showArrow(object sender, MouseEventArgs e)
        {

           // editClass.BeginAnimation(OpacityProperty, fadeIn_03);
        }
        private void showDel(object sender, MouseEventArgs e)
        {

            del.BeginAnimation(OpacityProperty, fadeIn_03);
        }
        private void hideArrow(object sender, MouseEventArgs e)
        {
            //editClass.BeginAnimation(OpacityProperty, fadeOut);
        }
        private void hideDel(object sender, MouseEventArgs e)
        {
            del.BeginAnimation(OpacityProperty, fadeOut);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            sizeInH.KeyFrames.Add(new SplineDoubleKeyFrame(260, TimeSpan.FromMilliseconds(350), new KeySpline(.5, 0, .3, 1)));
            sizeInW.KeyFrames.Add(new SplineDoubleKeyFrame(350, TimeSpan.FromMilliseconds(350), new KeySpline(.5, 0, .3, 1)));
            sizeOut.KeyFrames.Add(new SplineDoubleKeyFrame(1, TimeSpan.FromMilliseconds(350), new KeySpline(.5, 0, .3, 1)));

        }
        private void dragMD(object sender, MouseButtonEventArgs e)
        {
            _isDown = true;

        }
        private void dragMM(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
               
                _drag = true;
                item2 = new CharacterStrip();
                _draggedItem = e.Source as CharacterStrip;
                item2 = (_draggedItem as CharacterStrip);

                CreateDragDropWindow(_draggedItem, e.GetPosition(e.Source as Rectangle));
                ((_draggedItem as CharacterStrip).Content as Grid).BeginAnimation(HeightProperty, shrink);
                DragDrop.DoDragDrop(_draggedItem, new DataObject("UIElement", _draggedItem, true), DragDropEffects.Move);

                _visual.Close();
                _drag = false;
                _isDown = false;
            }
            
        }       
        private void dragMU(object sender, MouseButtonEventArgs e)
        {
            _drag = false;
            _isDown = false;
            _visual = null;

        }
        private void mainGF(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            _visual.Left = w32Mouse.X -760 ;
            _visual.Top = w32Mouse.Y +20;

        }
        private void nsDrop(object sender, DragEventArgs e)
        {

          //  var p = MainWindow.FindChild<overviewPage2>(Application.Current.MainWindow, "ovPage");
         //   var q = p.FindName("CreditsGraph");
         //   var r = q as StackPanel;
            var xt = e.Source.GetType().ToString();
           
                if (e.Data.GetDataPresent("UIElement"))
                {
                    UIElement droptarget = e.Source as UIElement;
                    var panel = (droptarget as CharacterStrip).Parent as StackPanel;
                    int droptargetIndex = -1, i = 0;
                    foreach (UIElement element in panel.Children)
                    {
                        if (element.Equals(droptarget))
                        {
                            droptargetIndex = i;
                            break;
                        }
                        i++;
                    }
                    var sourceItem = e.Data.GetData("UIElement") as CharacterStrip;

                    if (droptargetIndex != -1)
                    {
                        Character temp = new Character();
                    CharacterStrip temp2 = new CharacterStrip();
                        var originIndex = TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(sourceItem.Tag)));
                        temp = TeraLogic.CharList[originIndex];
                        temp2 = TeraMainWindow.CharacterStrips[originIndex];
                        panel.Children.Remove(sourceItem);
                        panel.Children.Insert(droptargetIndex, sourceItem);
                        (sourceItem.Content as Grid).BeginAnimation(HeightProperty, expand);
                        TeraLogic.CharList.RemoveAt(originIndex);
                        TeraLogic.CharList.Insert(droptargetIndex, temp);
                        TeraMainWindow.CharacterStrips.RemoveAt(originIndex);
                        TeraMainWindow.CharacterStrips.Insert(droptargetIndex, temp2);

                      //  var t = r.Children[originIndex];
                     //   r.Children.RemoveAt(originIndex);
                    //    r.Children.Insert(droptargetIndex, t);
                    }
                }
            
            
            _isDown = false;
                _drag = false;
            TeraLogic.IsSaved = false;

        }
        public void CreateDragDropWindow(Visual dragElement, Point xy)
        {
            _visual = new TeraMainWindow();
            _visual.WindowStyle = WindowStyle.None;
            _visual.AllowsTransparency = true;
            _visual.AllowDrop = false;
            _visual.Background = null;
            _visual.IsHitTestVisible = false;
            _visual.SizeToContent = SizeToContent.WidthAndHeight;
            _visual.Topmost = true;
            _visual.ShowInTaskbar = false;

            var s = new Image();

            Grid g = new Grid();
           // g.Style = FindResource("MaterialGrid") as Style;
            g.Margin = new Thickness(10, 10, 10, 10);
            g.Width = ((FrameworkElement)dragElement).ActualWidth;
            g.Height = ((FrameworkElement)dragElement).ActualHeight;
            Rectangle r = new Rectangle();
            r.HorizontalAlignment = HorizontalAlignment.Stretch;
            r.VerticalAlignment = VerticalAlignment.Stretch;
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            s.VerticalAlignment = VerticalAlignment.Stretch;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(dragElement);
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)g.Width, (int)g.Height, 96, 96,
                                                 PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(dragElement);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
                bitmap.Render(dv);
       
            s.Source = bitmap;
            g.Children.Add(s);

            _visual.Content = g;
            _visual.Left = xy.X-780;
            _visual.Top = xy.Y -80;
            _visual.Show();
        }
        public void delStrip(object sender, MouseButtonEventArgs e)
        {
            /**/
           // MainWindow.delDungStrip(this.Tag.ToString());
           // MainWindow.DungStrips.Remove(MainWindow.DungStrips.Find(x => Tag.Equals(this.Tag)));

            /*removes entry from chars array*/
            TeraLogic.CharList.Remove(TeraLogic.CharList.Find(x=>x.Name.Equals(this.Tag)));

            /*removes entry from strips array*/
            TeraMainWindow.CharacterStrips.Remove(TeraMainWindow.CharacterStrips.Find(x => Tag.Equals(this.Tag)));

            /*removes strip from panel after animation*/
            shrink.Completed += (a,b) => (this.Parent as StackPanel).Children.Remove(this);

            /*animate strip*/
            (this.Content as Grid).BeginAnimation(HeightProperty, shrink);

            /*sets data to unsaved*/
            TeraLogic.IsSaved = false;

        }
        public void setDirty(object sender, MouseButtonEventArgs e)
        {
            if (!TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(this.Tag)))].IsDirty)
            {
                TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(this.Tag)))].IsDirty = true;
                TeraLogic.IsSaved = false;
               // dirtyLed.Fill = new SolidColorBrush(new Color { A = 255, R = 200, B = 0, G = 50 });
            }
            else
            {
                TeraLogic.CharList[TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(this.Tag)))].IsDirty = false;
                TeraLogic.IsSaved = false;
                //  dirtyLed.Fill = new SolidColorBrush(new Color { A = 0, R = 200, B = 0, G = 50 });

            }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        private void textBoxCheck(object sender)
        {
            /*checks if textboxes value exceedes max value and corrects it if it does*/
            int x = 0;

            if ((sender as TextBox).Name.Equals("weeklyTB"))
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





        private void selectChar(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TeraLogic.SelectCharacter(this.Tag.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            //try
            //{
            //    TeraMainWindow.cvcp.SelectedChar = TeraLogic.CharList.Find(x => x.Name.Equals(this.Tag));
            //    var charIndex = (TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Equals(TeraMainWindow.cvcp.SelectedChar))));
            //    var w = UI.win.chView;

            
            //    w.charName.Text = TeraMainWindow.cvcp.SelectedChar.Name;
            //    w.charClassTB.Text = TeraMainWindow.cvcp.SelectedChar.CharClass;
            //    TeraMainWindow.createClassImageBinding(charIndex, "CharClass", w.classImg, "hd");
                
            //    TeraMainWindow.createLaurelImageBinding(TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Equals(TeraMainWindow.cvcp.SelectedChar))), "Laurel", w.laurelImg, "hd");
            //    TeraMainWindow.createBinding(charIndex, "Name", w.charName);
            //    TeraMainWindow.createBinding(charIndex, "Dailies", w.weeklyTB);
            //    TeraMainWindow.createBinding(charIndex, "Credits", w.creditsTB);
            //    TeraMainWindow.createBinding(charIndex, "MarksOfValor", w.mvTB);
            //    TeraMainWindow.createBinding(charIndex, "GoldfingerTokens", w.gfTB);

            //    TeraLogic.createBarBindings(w.questsBar, charIndex, "Dailies", TeraLogic.MAX_WEEKLY);
            //    createBarBindings(w.creditsBar, charIndex, "Credits", TeraLogic.MAX_CREDITS);
            //    createBarBindings(w.marksBar, charIndex, "MarksOfValor", TeraLogic.MAX_MARKS);
            //    createBarBindings(w.gfBar, charIndex, "GoldfingerTokens", TeraLogic.MAX_GF_TOKENS);

            //    createDgBindings(w, charIndex);



            //}

            //catch (Exception)
            //{
            //    Console.WriteLine(e.ToString());
            //}



        }


    }
}
    

