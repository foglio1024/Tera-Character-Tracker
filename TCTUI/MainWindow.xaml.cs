using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Drawing;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media.Effects;
using System.IO;
using System.Drawing.Imaging;
using System.ComponentModel;
using Tera.Converters;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>

    public partial class TeraMainWindow : Window
    {
        #region Constructor
        public TeraMainWindow()
        {
            InitializeComponent();
            Top = TeraLogic.TCTProps.Top;
            Left = TeraLogic.TCTProps.Left;
            Height = TeraLogic.TCTProps.Height;
            Width = TeraLogic.TCTProps.Width;

            UI.MainWin = this;
        }
        #endregion

        #region Fields
        const int LOG_CAP = 100;
        bool leftSlideIsOpen = false;
        bool isLogExpanded = false;
        Popup dgWindow = new Popup();
        public DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
        public DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(100));
        public DoubleAnimation glowIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));
        public DoubleAnimation glowOut = new DoubleAnimation( .3, TimeSpan.FromMilliseconds(150));
        public DoubleAnimation expand = new DoubleAnimation(40, TimeSpan.FromMilliseconds(100));

        #endregion

        #region Properties
        public static List<CharacterStrip> CharacterStrips { get; set; } = new List<CharacterStrip>();
        #endregion

        #region Methods
        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        private void CreateStripControlsBindings(int i)
        {

            /*creates text boxes data bindings*/
            CharacterStrips[i].nameTB.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, "Name"));
            CharacterStrips[i].lvlTB.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, "Level"));
            DataBinder.BindParameterToBarGauge(i, "Credits", CharacterStrips[i].creditsTB, TeraLogic.MAX_CREDITS, TeraLogic.MAX_CREDITS - 300, true, true);
            DataBinder.BindParameterToBarGauge(i, "MarksOfValor", CharacterStrips[i].mvTB, TeraLogic.MAX_MARKS, TeraLogic.MAX_MARKS-10, true, true);
            DataBinder.BindParameterToBarGauge(i, "GoldfingerTokens", CharacterStrips[i].gftTB, TeraLogic.MAX_GF_TOKENS, TeraLogic.MAX_GF_TOKENS-10, true, true);
            DataBinder.BindParameterToQuestBarGauge(i, "Weekly", "Dailies", CharacterStrips[i].questTB, TeraLogic.MAX_WEEKLY, TeraLogic.MAX_WEEKLY - TeraLogic.MAX_DAILY, TeraLogic.MAX_DAILY, TeraLogic.MAX_DAILY, true, false);
            DataBinder.BindParameterToImageSourceWithConverter(i, "CharClass", CharacterStrips[i].classImage, "sd", new ClassToImage());
            DataBinder.BindCharPropertyToShapeFillColor(i, "Laurel", CharacterStrips[i].laurelRect, new Laurel_GradeToColor());
            DataBinder.BindParameterToArcGauge(i, "Crystalbind", CharacterStrips[i].ccbInd, new TimeToAngle());
            CharacterStrips[i].ccbInd.SetBinding(ToolTipProperty, DataBinder.GenericCharBinding(i, "Crystalbind", new TicksToTimespan(), null));
            CharacterStrips[i].SetBinding(TagProperty, DataBinder.GenericCharBinding(i, "Name"));
            
        }
        public void AddStripToContainer(int i)
        {
            /*adds strip to panel*/
            (CharacterStrips[i].Content as Grid).Height = 1;
            accounts.chContainer.Items.Add(CharacterStrips[i]);
            (CharacterStrips[i].Content as Grid).BeginAnimation(FrameworkElement.HeightProperty, expand);
        }
        public void CreateStrip(int i)
        {
            /*adds strip to array*/
            CharacterStrips.Add(new CharacterStrip());

            /*create bindings for controls*/
            CreateStripControlsBindings(i);

            /*add strip to container*/
            AddStripToContainer(i);

        }
        public void UpdateLog(string txt)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var li = new ListBoxItem();

                DateTime time = DateTime.Now;
                string format = "HH:mm";
                li.Focusable = false;
                li.IsTabStop = false;
                li.HorizontalAlignment = HorizontalAlignment.Stretch;
                li.Content = "[" + time.ToString(format) + "]  -  " + txt;
                if (Log.Items.Count > 0)
                {
                    ListBoxItem item = Log.Items[Log.Items.Count - 1] as ListBoxItem;
                    var tmp = item.Content as string;
                    tmp = tmp.Substring(12);

                    if (txt != tmp)
                    {
                        Log.Items.Add(li); 
                        Log.ScrollIntoView(li);
                    }
                }
                else
                {
                    Log.Items.Add(li);
                    Log.ScrollIntoView(li);

                }
                if(Log.Items.Count > LOG_CAP)
                {
                    Log.Items.RemoveAt(0);
                }

            }

            ));
            

        }
        public void SetGuildImage(Bitmap logo)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                MemoryStream stream = new MemoryStream();

                logo.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                chView.guildLogo.Source = result;


            }));
        }
        #endregion

        #region Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            /*creates strips for loaded chars*/
            for (int i = 0; i < TeraLogic.CharList.Count; i++)
            {
                CreateStrip(i);
            }
               

            TeraLogic.IsSaved = true;

            foreach (var dg in TeraLogic.DungList)
            {
                DungeonRunsCounter d = new DungeonRunsCounter();
                d.Name = dg.ShortName;
                d.Tag = dg.ShortName;
                d.n.Text = dg.ShortName;

                switch (dg.Tier)
                {
                    case DungeonTier.Starter:
                        chView.starterTier.Children.Add(d);
                        break;

                    case DungeonTier.Mid:
                        chView.midTier.Children.Add(d);
                        break;

                    case DungeonTier.MidHigh:
                        chView.midHighTier.Children.Add(d);
                        break;

                    case DungeonTier.High:
                        chView.highTier.Children.Add(d);
                        break;

                    case DungeonTier.Top:
                        chView.topTier.Children.Add(d);
                        break;

                    default:
                        break;
                }
            }

            ToolBar.Background = SystemParameters.WindowGlassBrush;
            StatusBar.Background = SystemParameters.WindowGlassBrush;
            chView.guildGrid.Background = SystemParameters.WindowGlassBrush;

            this.Activate();

        }
        private void LogSizeToggle(object sender, MouseButtonEventArgs e)
        {
            var s = sender as Grid;
            var expand = new DoubleAnimationUsingKeyFrames();
            expand.KeyFrames.Add(new SplineDoubleKeyFrame(145, TimeSpan.FromMilliseconds(150), new KeySpline(.5, 0, .3, 1)));
            var shrink = new DoubleAnimationUsingKeyFrames();
            shrink.KeyFrames.Add(new SplineDoubleKeyFrame(20, TimeSpan.FromMilliseconds(150), new KeySpline(.5, 0, .3, 1)));

            expand.Completed += (se, ex) => {
                Log.Items.MoveCurrentToLast();
                Log.ScrollIntoView(Log.Items.CurrentItem);
            };

            shrink.Completed += (se, ex) =>
            {
                Log.Items.MoveCurrentToLast();
                Log.ScrollIntoView(Log.Items.CurrentItem);
            };

            /*convert to animation*/
            if (!isLogExpanded)
            {
                s.BeginAnimation(HeightProperty, expand);
                isLogExpanded = true;
            }
            else 
            {
                s.BeginAnimation(HeightProperty, shrink);
                isLogExpanded = false;
            }
        } 
        private void SaveButtonPressed(object sender, RoutedEventArgs e)
        {
            TeraLogic.SaveCharacters();
            TeraLogic.SaveAccounts();
            TeraLogic.SaveGuildsDB();

            TeraLogic.IsSaved = true;
        }
        private void LeftSlideToggle(object sender, MouseButtonEventArgs e)
        {
             ThicknessAnimationUsingKeyFrames an = new ThicknessAnimationUsingKeyFrames();

            if (!leftSlideIsOpen)
            {
                an.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0), TimeSpan.FromMilliseconds(220), new KeySpline(.5, 0, .3, 1)));
                leftSlideIsOpen = true;
            }
            else
            {
                an.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(-420,0,0,0), TimeSpan.FromMilliseconds(220), new KeySpline(.5, 0, .3, 1)));
                leftSlideIsOpen = false;
            }

            leftSlide1.BeginAnimation(MarginProperty, an);

        }
        private void ShowDungeonOverview(object sender, RoutedEventArgs e)
        {
            
            var w = dgWindow;
            w.MouseLeave += (s0, e0) =>
             {
                 w.IsOpen = false;
             };
            if (w.IsOpen)
            {
                w.IsOpen = false;
            }
            else
            {
                /*add char names in left column*/

                w.Child = new DungeonsWindow();
                w.AllowsTransparency = true;
                w.PlacementTarget = sender as System.Windows.Controls.Image;
                w.Placement = PlacementMode.Right;
                foreach (var c in TeraLogic.CharList)
                {
                    if (c.Level == 65)
                    {
                        (w.Child as DungeonsWindow).chars.Children.Add(new TextBlock { Text = c.Name, Height = 20, Foreground = new SolidColorBrush { Color = new System.Windows.Media.Color { A = 130, R = 0, G = 0, B = 0 } } });
                    }
                }


                /*add dungeons short names in header (also "Characters")*/
                foreach (var dg in TeraLogic.DungList)
                {
                    (w.Child as DungeonsWindow).header.Children.Add(new TextBlock { Text = dg.ShortName, TextAlignment=TextAlignment.Center, Width = 30, Foreground = new SolidColorBrush { Color = new System.Windows.Media.Color { A = 130, R = 0, G = 0, B = 0 } }, VerticalAlignment = VerticalAlignment.Center });
                }

                /*add dungeon circles indicators in main panel*/
                foreach (var c in TeraLogic.CharList)
                {
                    if (c.Level == 65)
                    {

                        var sp = new StackPanel();
                        sp.Orientation = Orientation.Horizontal;
                        foreach (var dg in TeraLogic.DungList)
                        {
                            var g = new Grid();
                            
                            g.Width = 30;
                            g.Height = 20;
                            g.Background = new SolidColorBrush(new System.Windows.Media.Color{ A = 255, R = 255, G = 255, B = 255 });
                            var el = new Ellipse();
                            var t = new TextBlock();
                            el.Width = 15;
                            el.Height = 15;
                            t.FontSize = 11;
                            t.Foreground = new SolidColorBrush(new System.Windows.Media.Color { A = 190, R = 255, G = 255, B = 255 });
                            el.HorizontalAlignment = HorizontalAlignment.Center;
                            el.VerticalAlignment = VerticalAlignment.Center;
                            t.HorizontalAlignment = HorizontalAlignment.Center;
                            t.VerticalAlignment = VerticalAlignment.Center;
                            t.TextAlignment = TextAlignment.Center;
                            t.FontWeight = FontWeights.DemiBold;
                            el.Fill = new SolidColorBrush(new System.Windows.Media.Color { A = 255, R = 0, G = 0, B = 0 });
                            int max = dg.MaxBaseRuns;
                            if (TeraLogic.AccountList.Find(a => a.Id == c.AccountId).TeraClub)
                            {
                                if(dg.ShortName != "CA" && dg.ShortName != "GL" && dg.ShortName != "AH")
                                {
                                    max = dg.MaxBaseRuns * 2;
                                }
                            }
                            int dgIndex = c.Dungeons.IndexOf(c.Dungeons.Find(d => d.Name.Equals(dg.ShortName)));
                            var b = new Binding
                            {
                                Source = c.Dungeons[dgIndex],
                                Path = new PropertyPath("Runs"),
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.OneWay,
                                Converter = new Dungeon_RunsToColor(),
                                ConverterParameter = max,

                            };
                            var tb = new Binding
                            {
                                Source = c.Dungeons[dgIndex],
                                Path = new PropertyPath("Runs"),
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.OneWay,
                            };
                            t.SetBinding(TextBlock.TextProperty, tb);
                            el.SetBinding(Shape.FillProperty, b);

                            g.Children.Add(el);
                            g.Children.Add(t);
                            g.Tag = c.Name +"%"+ dg.ShortName;
                            g.MouseEnter += (snd, evn) =>
                            {
                                var parameters = (snd as Grid).Tag.ToString().Split('%');
                                string chName = parameters[0];
                                string dgName = parameters[1];
                                int chIndex = TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(chName)));
                                int dgIndex0 = TeraLogic.DungList.IndexOf(TeraLogic.DungList.Find(x => x.ShortName.Equals(dgName)));

                                (((w.Child as DungeonsWindow).chars.Children[chIndex] as TextBlock)).Foreground = new SolidColorBrush(new System.Windows.Media.Color { A = 255, R = 0, G = 0, B = 0 });
                                (((w.Child as DungeonsWindow).header.Children[dgIndex0] as TextBlock)).Foreground = new SolidColorBrush(new System.Windows.Media.Color { A = 255, R = 0, G = 0, B = 0 });

                            };

                            g.MouseLeave += (snd, evn) =>
                            {
                                var parameters = (snd as Grid).Tag.ToString().Split('%');
                                string chName = parameters[0];
                                string dgName = parameters[1];
                                int chIndex = TeraLogic.CharList.IndexOf(TeraLogic.CharList.Find(x => x.Name.Equals(chName)));
                                int dgIndex0 = TeraLogic.DungList.IndexOf(TeraLogic.DungList.Find(x => x.ShortName.Equals(dgName)));

                                (((w.Child as DungeonsWindow).chars.Children[chIndex] as TextBlock)).Foreground = new SolidColorBrush(new System.Windows.Media.Color { A = 130, R = 0, G = 0, B = 0 });
                                (((w.Child as DungeonsWindow).header.Children[dgIndex0] as TextBlock)).Foreground = new SolidColorBrush(new System.Windows.Media.Color { A = 130, R = 0, G = 0, B = 0 });

                            };

                            sp.Children.Add(g);


                        }

                    (w.Child as DungeonsWindow).content.Children.Add(sp);

                    }
                }



                w.IsOpen = true;
            }
        }
        private void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TeraLogic.SaveCharacters();
            TeraLogic.SaveAccounts();
            TeraLogic.SaveGuildsDB();
            TeraLogic.TCTProps.Top = this.Top;
            TeraLogic.TCTProps.Left = this.Left;
            TeraLogic.TCTProps.Height = this.Height;
            TeraLogic.TCTProps.Width = this.Width;
        }
        private void TeraMainWin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
        #endregion

        int i = 0;
        private void TestButton(object sender, MouseButtonEventArgs e)
        {
            string teststring = "Test" + i;

           TCTNotifier.NotificationProvider.NS.sendNotification(teststring, TCTNotifier.NotificationType.Credits, Colors.LightGreen);
           i++;
        }

    }

}



