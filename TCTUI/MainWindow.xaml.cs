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
using Tera.classes;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media.Effects;
using System.IO;
using System.Drawing.Imaging;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public static class TCTProps
    {
        public static bool reset;
        public static bool firstLaunchAfterReset;
        public static DateTime lastClosed;
        public static double Top;
        public static double Left;
        public static double Width;
        public static double Height;
    }

    public partial class TeraMainWindow : Window
    {
        #region Constructor
        public TeraMainWindow()
        {
            InitializeComponent();
            this.Top = TCTProps.Top;
            this.Left = TCTProps.Left;
            this.Height = TCTProps.Height;
            this.Width = TCTProps.Width;

            UI.win = this;


        }
        #endregion

        #region Fields
        static public string activeChar;
        static public System.Windows.Media.Color baseColor      = System.Windows.Media.Color.FromArgb(255, 96, 125, 139);
        static public System.Windows.Media.Color accentColor    = System.Windows.Media.Color.FromArgb(255, 255, 120, 42);

        //public static TeraLogic TeraLogic = new TeraLogic();
        public static CharViewContentProvider cvcp = new CharViewContentProvider();
        private static List<newStrip> newStrips = new List<newStrip>();
        private static List<dungeonStrip> dungStrips = new List<dungeonStrip>();
        private static List<Laurel> laurels = new List<Laurel>();
        public static bool leftSlideIsOpen = false;
        TranslateTransform t_selTab = new TranslateTransform();
        TranslateTransform t_panel = new TranslateTransform();
        ScaleTransform s_selTab = new ScaleTransform();
        public DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
        public DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(100));
        public DoubleAnimation glowIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));
        public DoubleAnimation glowOut = new DoubleAnimation( .3, TimeSpan.FromMilliseconds(150));
        public DoubleAnimation expand = new DoubleAnimation(40, TimeSpan.FromMilliseconds(100));

        #endregion

        #region Properties
        //public static List<Character> CharList { get { return charList; } set { charList = value; } }
        public static List<Laurel> Laurels { get { return laurels; } set { laurels = value; } }

        public static List<newStrip> NewStrips { get { return newStrips; } set { newStrips = value; } }
        public static List<dungeonStrip> DungStrips { get { return dungStrips; } set { dungStrips= value; } }

        #endregion

        #region Methods

        public static void labelUpdate(string labelName, string text, int variable)
        {
            Label lbl = FindChild<Label>(Application.Current.MainWindow, labelName);
            lbl.Content = text + Convert.ToString(variable);
        }
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
        public static void setTabIndex(int i, oldStrip c)
        {
            c.nameTB.TabIndex = 5 * i + 0;
            c.dailiesTB.TabIndex = 5 * i + 2;
            c.creditsTB.TabIndex = 5 * i + 1;
            c.mvTB.TabIndex = 5 * i + 3;
            c.gftTB.TabIndex = 5 * i + 4;
        }
        public static void createBinding(int i, string property, TextBox t)
        {
            var binding = new Binding();
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            t.SetBinding(TextBox.TextProperty, binding);
        }
        public static void createBinding(int i, string property, Label t)
        {
            var binding = new Binding();
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            t.SetBinding(Label.ContentProperty, binding);
        }
        public static void createBinding(int i, string property, TextBlock t)
        {
            var b1 = new Binding();
            b1.Source = TeraLogic.CharList[i];
            b1.Path = new PropertyPath(property);
            b1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            t.SetBinding(TextBlock.TextProperty, b1);
        }
        public static void createBinding(int i, string property, TextBlock t, GuildConverter converter)
        {
            var b1 = new Binding();
            b1.Source = TeraLogic.CharList[i];
            b1.Path = new PropertyPath(property);
            b1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b1.Converter = converter;
            t.SetBinding(TextBlock.TextProperty, b1);
        }
        public static void createBinding(int i, string property, TextBlock t, LocationConverter converter)
        {
            var b1 = new Binding();
            b1.Source = TeraLogic.CharList[i];
            b1.Path = new PropertyPath(property);
            b1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b1.Converter = converter;
            t.SetBinding(TextBlock.TextProperty, b1);
        }
        public static void createBinding(int i, string property, TextBlock t, LastOnlineConverter converter)
        {
            var b1 = new Binding();
            b1.Source = TeraLogic.CharList[i];
            b1.Path = new PropertyPath(property);
            b1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b1.Converter = converter;
            t.SetBinding(TextBlock.TextProperty, b1);
        }

        public static void createBinding(int i, string property, BarGauge t, int maxValue, int threshold, bool color, bool invert)
        {
            var b = new Binding();
            b.Source = TeraLogic.CharList[i];
            b.Path = new PropertyPath(property);
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.Converter = new barLengthConverter();
            b.ConverterParameter = new double[] { t.@base.Width, maxValue };
            t.val.SetBinding(WidthProperty, b);

            var b2 = new Binding();
            b2.Source = TeraLogic.CharList[i];
            b2.Path = new PropertyPath(property);
            b2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b2.Converter = new ProgressToColorConverter();

            object[] parc = { maxValue, threshold, invert };

            b2.ConverterParameter = parc;
            if (color)
            {
                t.val.SetBinding(Shape.FillProperty, b2);

            }
            var b3 = new Binding();
            b3.Source = TeraLogic.CharList[i];
            b3.Path = new PropertyPath(property);
            b3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            t.txt.SetBinding(TextBlock.TextProperty, b3);
            #region MyRegion
            ////var binding1 = new Binding();
            ////binding1.Source = TeraLogic.CharList[i];
            ////binding1.Path = new PropertyPath(property);
            ////binding1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ////t.grid.SetBinding(ToolTipProperty, binding1);

            //var binding2 = new Binding();
            //binding2.Source = TeraLogic.CharList[i];
            //binding2.Path = new PropertyPath(property);
            //binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //binding2.Converter = new barLengthConverter(); //ProgressToAngleConverter();
            //object[] par = { maxValue, threshold, invert };

            //binding2.ConverterParameter = par;//(double)maxValue;
            ////t.arc.SetBinding(Arc.EndAngleProperty, binding2);
            //t.val.SetBinding(WidthProperty, binding2);

            //var binding3 = new Binding();
            //binding3.Source = TeraLogic.CharList[i];
            //binding3.Path = new PropertyPath(property);
            //binding3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //binding3.Converter = new ProgressToColorConverter();

            //object[] parc = { maxValue, threshold, invert };

            //binding3.ConverterParameter = parc;
            //if (color)
            //{
            //    t.arc.SetBinding(Arc.StrokeProperty, binding3);
            //    t.@base.SetBinding(Ellipse.StrokeProperty, binding3);

            //}
            #endregion
        }
        public static void createQuestBinding(int i, string propertyW, string propertyD, QuestGauge t, int maxValueW, int thresholdW, int maxValueD, int thresholdD, bool color, bool invert)
        {
            var b = new Binding();
            b.Source = TeraLogic.CharList[i];
            b.Path = new PropertyPath(propertyD);
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.Converter = new DailybarLengthConverter();
            b.ConverterParameter = new double[] { t.@base.Width, TeraLogic.MAX_WEEKLY-TeraLogic.CharList[i].Weekly };
            t.valD.SetBinding(WidthProperty, b);
            
            //var b2 = new Binding();
            //b2.Source = TeraLogic.CharList[i];
            //b2.Path = new PropertyPath(propertyD);
            //b2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //b2.Converter = new ProgressToColorConverter();

            //object[] parc = { maxValueD, thresholdD, invert };

            //b2.ConverterParameter = parc;
            //if (color)
            //{
            //    t.valD.SetBinding(Shape.FillProperty, b2);
            //}

            var b3 = new Binding();
            b3.Source = TeraLogic.CharList[i];
            b3.Path = new PropertyPath(propertyD);
            b3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            t.txtD.SetBinding(TextBlock.TextProperty, b3);

            var b4 = new Binding();
            b4.Source = TeraLogic.CharList[i];
            b4.Path = new PropertyPath(propertyW);
            b4.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b4.Converter = new barLengthConverter();
            b4.ConverterParameter = new double[] { t.@base.Width, maxValueW };
            t.valW.SetBinding(WidthProperty, b4);

            var b5 = new Binding();
            b5.Source = TeraLogic.CharList[i];
            b5.Path = new PropertyPath(propertyW);
            b5.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b5.Converter = new ProgressToColorConverter();

            object[] parcW = { maxValueW, thresholdW, invert };

            b5.ConverterParameter = parcW;
            if (color)
            {
                t.valW.SetBinding(Shape.FillProperty, b5);
                t.valD.SetBinding(Shape.FillProperty, b5);
                t.borD.SetBinding(Border.BorderBrushProperty, b5);

            }
            var b6 = new Binding();
            b6.Source = TeraLogic.CharList[i];
            b6.Path = new PropertyPath(propertyW);
            b6.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            t.txtW.SetBinding(TextBlock.TextProperty, b6);

        }


        public static void createClassImageBinding(int i, string property, System.Windows.Controls.Image t, string def)
        {
            var binding = new Binding();
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Converter = new charClassConverter();
            binding.ConverterParameter = def;
            t.SetBinding(System.Windows.Controls.Image.SourceProperty, binding);
        }
        public static void createLaurelImageBinding(int i, string property, System.Windows.Controls.Image t, string def)
        {
            var binding = new Binding();
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Converter = new LaurelImgConverter();
            binding.ConverterParameter = def;
            t.SetBinding(System.Windows.Controls.Image.SourceProperty, binding);
        }
  
        
        public static void createCheckBinding(int i, string property, System.Windows.Controls.Image t)
        {
            var binding = new Binding();
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Converter = new DailiesToVisibilityConverter();
            t.SetBinding(VisibilityProperty, binding);
        }
        public static void createDailiesGaugeBinding(int i, string property, object x)
        {
            var t = (x as System.Windows.Controls.Image);
            var binding = new Binding();
            binding.Mode = BindingMode.OneWay;
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Converter = new DailiesGaugeImageSourceConverter();
            t.SetBinding(System.Windows.Controls.Image.SourceProperty, binding);
            
        }
        public static void shapeFillColorBinding(int i, string property, Shape t, IValueConverter conv)
        {
            var binding = new Binding();
            binding.Source = TeraLogic.CharList[i];
            binding.Path = new PropertyPath(property);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Converter = conv;// new LaurelColorConverter();
            t.SetBinding(Shape.FillProperty, binding);
        }
   //     public static void createDgGaugeBinding(int i, int j, string property, System.Windows.Controls.Image t, int p)
   //     {
   //         var binding = new Binding();
   //         binding.Mode = BindingMode.OneWay;
   //         binding.Source = TeraLogic.CharList[i].Dungeons[j];
   //         binding.Path = new PropertyPath(property);
   //         binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
   //         binding.Converter = new DgGaugeImageSourceConverter();
   //         binding.Mode = BindingMode.OneWay;

   //         binding.ConverterParameter = p;
   //         t.SetBinding(System.Windows.Controls.Image.SourceProperty, binding);

   //     }
   //     public static void createDgBinding(int i,int j, string property, UserControl t, int p)
   //     {
   //         var binding = new Binding();
   //         binding.Source = TeraLogic.CharList[i].Dungeons[j];
   //         binding.Path = new PropertyPath(property);
   //         binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
   //         binding.Mode = BindingMode.OneWay;
   ///*         binding.Converter = new DgRunsConverter();
   //         binding.ConverterParameter = p;*/
   //         ((t.Content as Grid).FindName("t") as TextBlock).SetBinding(TextBlock.TextProperty, binding);
   //     }
        private void newCharControls(int i)
        {

            /*creates text boxes data bindings*/
            createBinding(i, "Name", TeraMainWindow.NewStrips[i].nameTB);
            createBinding(i, "Credits", TeraMainWindow.NewStrips[i].creditsTB, TeraLogic.MAX_CREDITS, TeraLogic.MAX_CREDITS - 300, true, true);
            createBinding(i, "MarksOfValor", TeraMainWindow.NewStrips[i].mvTB, TeraLogic.MAX_MARKS, TeraLogic.MAX_MARKS-10, true, true);
            createBinding(i, "GoldfingerTokens", TeraMainWindow.NewStrips[i].gftTB, TeraLogic.MAX_GF_TOKENS, TeraLogic.MAX_GF_TOKENS-10, true, true);
            createBinding(i, "Level", NewStrips[i].lvlTB);
            createQuestBinding(i, "Weekly", "Dailies", TeraMainWindow.NewStrips[i].questTB, TeraLogic.MAX_WEEKLY, TeraLogic.MAX_WEEKLY - TeraLogic.MAX_DAILY, TeraLogic.MAX_DAILY, TeraLogic.MAX_DAILY, true, false);
            createClassImageBinding(i, "CharClass", TeraMainWindow.NewStrips[i].classImage, "sd");
            shapeFillColorBinding(i, "Laurel", TeraMainWindow.NewStrips[i].laurelRect, new LaurelColorConverter());

            /*creates bindings for tags*/
            var b = new Binding();
            b.Source = TeraLogic.CharList[i];
            b.Path = new PropertyPath("Name");
            NewStrips[i].nameTB.SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].creditsTB.SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].mvTB.SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].questTB.SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].gftTB.SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].laurelRect.SetBinding(FrameworkElement.TagProperty, b);
            NewStrips[i].lvlTB.SetBinding(TagProperty, b);
            NewStrips[i].SetBinding(TagProperty, b);

            //TeraMainWindow.shapeFillColorBinding(i, "IsDirty", TeraMainWindow.NewStrips[i].dirtyLed, new ledConv());

            /*adds strip to panel*/
            (TeraMainWindow.NewStrips[i].Content as Grid).Height = 1;
            ovPage.tableGridContent.Items.Add(TeraMainWindow.NewStrips[i]); /*newCharBox*/

            (TeraMainWindow.NewStrips[i].Content as Grid).BeginAnimation(FrameworkElement.HeightProperty, expand);
            //TeraMainWinwdow.createDungStrips(i);
        }
        public static void delDungStrip(string name)
        {
            var a = FindChild<dungeonsPage>(Application.Current.MainWindow, "dgPage").dungeonTableGridContent;
            foreach (dungeonStrip c in a.Children)
            {
                if (c.Tag.ToString() == name)
                {
                    a.Children.Remove(c);
                    break;
                }
            }

        }
        public static void createDungStrips(int i)
        {
           
                /*adds strip*/
                DungStrips.Add(new dungeonStrip());
            var b = new Binding();
            b.Source = TeraLogic.CharList[i];
            b.Path = new PropertyPath("Name");
            DungStrips[i].SetBinding(TagProperty, b);

            /*creates text boxes data bindings*/
            createClassImageBinding(i, "CharClass", DungStrips[i].classImage, "sd");
                var binding = new Binding();
                binding.Source = TeraLogic.CharList[i];
                binding.Path = new PropertyPath("Name");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                DungStrips[i].charName.SetBinding(TextBlock.TextProperty, binding);
                (DungStrips[i].CtSP as StackPanel).SetBinding(TagProperty, binding);

            for (int j = 0; j < TeraLogic.CharList[i].Dungeons.Count; j++)
            {
                //_addDungCtrls(i,j); 
            }


                /*adds strip to panel*/
           //    FindChild<dungeonsPage>(Application.Current.MainWindow, "dgPage").dungeonTableGridContent.Children.Add(DungStrips[i]);
         


            

        }
        public void createNewStrip(int i)
        {
            /*adds strip*/
            NewStrips.Add(new newStrip());
            var gftCol = new System.Windows.Media.Color { A = 255, R = 255, G = 210, B = 120 };

            //NewStrips[i].gftTB.arc.Stroke = new SolidColorBrush(gftCol);
            //NewStrips[i].gftTB.@base.Stroke = new SolidColorBrush(gftCol);

            NewStrips[i].gftTB.val.Fill= new SolidColorBrush(gftCol);
            //NewStrips[i].gftTB.@base.Fill = new SolidColorBrush(gftCol);

            //NewStrips[i].gftTB.@base.Opacity = .15;

            newCharControls(i);


        }
        public void createNewStrips()
        {

            for (int i = 0; i < TeraLogic.CharList.Count; i++)
            {
                createNewStrip(i);
            }

        }



        //public void addNewStrips()
        //{


        //    // var a = FindChild<overviewPage2>(w, "ovPage");
        //    var a = UI.win.ovPage;
        //        for (int i = 0; i < TeraLogic.CharList.Count; i++)
        //        {
        //            (NewStrips[i].Content as Grid).BeginAnimation(HeightProperty, new DoubleAnimation(40, TimeSpan.FromMilliseconds(100)));
        //            a.tableGridContent.Children.Add(NewStrips[i]);
        //        }
             
        //}
        //public static void addDungCtrls(int i, int j)
        //{
        //    if (Properties.Settings.Default.TeraClub)
        //    {

        //            if (TeraLogic.DungeonsArray[j].Equals("KC") || TeraLogic.DungeonsArray[j].Equals("CW") || TeraLogic.DungeonsArray[j].Equals("KDNM"))
        //            {
        //                var x = new dgSel4(TeraLogic.DungeonsArray[j], CharList[i].Name);
        //                createDgBinding(i, j, "Runs", x, 4);
        //                createDgGaugeBinding(i, j, "Runs", x.dgGaugeMask, 4);
        //                x.Tag = TeraLogic.DungeonsArray[j];
        //                Grid.SetColumn(x, j + 1);
        //                (DungStrips[i].Content as Grid).Children.Add(x);
        //            }
        //            else if (TeraLogic.DungeonsArray[j].Equals("GG"))
        //            {
        //                var x = new dgSel3(TeraLogic.DungeonsArray[j], CharList[i].Name);
        //                createDgBinding(i, j, "Runs", x, 3);
        //                createDgGaugeBinding(i, j, "Runs", x.dgGaugeMask, 3);
        //                x.Tag = TeraLogic.DungeonsArray[j];
        //                Grid.SetColumn(x, j + 1);
        //                (DungStrips[i].Content as Grid).Children.Add(x);
        //            }
        //            else
        //            {
        //                var x = new dgSel2(TeraLogic.DungeonsArray[j], CharList[i].Name);
        //                createDgBinding(i, j, "Runs", x, 2);
        //                createDgGaugeBinding(i, j, "Runs", x.dgGaugeMask, 2);

        //                x.Tag = TeraLogic.DungeonsArray[j];
        //                Grid.SetColumn(x, j + 1);
        //                (DungStrips[i].Content as Grid).Children.Add(x);
        //            }

        //    }

        //    else
        //    {

        //            if (TeraLogic.DungeonsArray[j].Equals("KC") || TeraLogic.DungeonsArray[j].Equals("CW") || TeraLogic.DungeonsArray[j].Equals("KDNM"))
        //            {
        //                var x = new dgSel2(TeraLogic.DungeonsArray[j], CharList[i].Name);
        //                createDgBinding(i, j, "Runs", x, 2);
        //                createDgGaugeBinding(i, j, "Runs", x.dgGaugeMask, 2);

        //                x.Tag = TeraLogic.DungeonsArray[j];
        //                Grid.SetColumn(x, j + 1);
        //                (DungStrips[i].Content as Grid).Children.Add(x);
        //            }
        //            else if (TeraLogic.DungeonsArray[j].Equals("GG"))
        //            {
        //                var x = new dgSel3(TeraLogic.DungeonsArray[j], CharList[i].Name);
        //                createDgBinding(i, j, "Runs", x, 3);
        //                createDgGaugeBinding(i, j, "Runs", x.dgGaugeMask, 3);

        //                x.Tag = TeraLogic.DungeonsArray[j];
        //                Grid.SetColumn(x, j + 1);
        //                (DungStrips[i].Content as Grid).Children.Add(x);
        //            }
        //            else
        //            {
        //                var x = new dgSel1(TeraLogic.DungeonsArray[j], CharList[i].Name);
        //                createDgBinding(i, j, "Runs", x, 1);
        //           //     createDgGaugeBinding(i, j, "Runs", x.dgGaugeMask, 1);

        //                x.Tag = TeraLogic.DungeonsArray[j];
        //                Grid.SetColumn(x, j + 1);
        //                (DungStrips[i].Content as Grid).Children.Add(x);
        //            }


        //    }
        //}
        //public static void _addDungCtrls(int ch, int dung)
        //{
        //    var a = FindChild<dungeonsPage>(Application.Current.MainWindow, "dgPage");
        //    var spacer = 5;
        //    if (!Properties.Settings.Default.TeraClub)
        //    {
        //        if (DungList[dung].MaxBaseRuns == 1)
        //        {
        //            var x = new dgSelNew(DungList[dung].ShortName, CharList[ch].Name);
        //            createDgBinding(ch, dung, "Runs", x, 4);
        //            x.Tag = DungList[dung].ShortName;
        //            x.Width = (a.ActualWidth - a.dungHeaderGrid.ColumnDefinitions[1].ActualWidth-spacer) / DungList.Count;
        //            (DungStrips[ch].CtSP as StackPanel).Children.Add(x);
        //        }
        //        else if (DungList[dung].MaxBaseRuns == 2)
        //        {
        //            var x = new dgSel2(DungList[dung].ShortName, CharList[ch].Name);
        //            createDgBinding(ch, dung, "Runs", x, 4);
        //            x.Tag = DungList[dung].ShortName;
        //            x.Width = (a.ActualWidth - a.dungHeaderGrid.ColumnDefinitions[1].ActualWidth - spacer) / DungList.Count;
        //            (DungStrips[ch].CtSP as StackPanel).Children.Add(x);
        //        }
        //    }
        //    else
        //    {
        //        if (DungList[dung].MaxBaseRuns == 1)
        //        {
        //            var x = new dgSel2(DungList[dung].ShortName, CharList[ch].Name);
        //            createDgBinding(ch, dung, "Runs", x, 4);
        //            x.Tag = DungList[dung].ShortName;
        //            x.Width = (a.ActualWidth - a.dungHeaderGrid.ColumnDefinitions[1].ActualWidth - spacer) / DungList.Count;
        //            (DungStrips[ch].CtSP as StackPanel).Children.Add(x);
        //        }
        //        else if (DungList[dung].MaxBaseRuns == 2)
        //        {
        //            var x = new dgSel4(DungList[dung].ShortName, CharList[ch].Name);
        //            createDgBinding(ch, dung, "Runs", x, 4);
        //            x.Tag = DungList[dung].ShortName;
        //            x.Width = (a.ActualWidth - a.dungHeaderGrid.ColumnDefinitions[1].ActualWidth - spacer) / DungList.Count;
        //            (DungStrips[ch].CtSP as StackPanel).Children.Add(x);
        //        }
        //    }
        //}

        //public void createCreditsGraphBars()
        //{
        //    double barWidth=10;
        //    double margin;
        //    margin = (ovPage.CreditsGraph.ActualWidth - barWidth * CharList.Count)/(2*CharList.Count);
        //    for (int i = 0; i < CharList.Count; i++)
        //    {

        //        var bar = new System.Windows.Shapes.Rectangle();
        //        bar.Width = barWidth;
        //        bar.MinHeight = 2;
        //        bar.Margin=new System.Windows.Thickness(margin,0,margin,0);
        //        bar.VerticalAlignment = VerticalAlignment.Bottom;
        //        bar.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,96, 125, 139));
        //        bar.Opacity = .3;
        //        bar.Tag = CharList[i].Name;
        //       // bar.Cursor = Cursors.Hand;

        //        var barHeightBinding = new Binding();
        //        barHeightBinding.Source = CharList[i];
        //        barHeightBinding.Path = new PropertyPath("Credits");
        //        barHeightBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //        barHeightBinding.Converter = new graphCreditsConverter();
        //        barHeightBinding.ConverterParameter = Convert.ToDouble(ovPage.CreditsGraph.ActualHeight);
        //        bar.SetBinding(Shape.HeightProperty, barHeightBinding);

        //        var barColorBinding = new Binding();
        //        barColorBinding.Source = CharList[i];
        //        barColorBinding.Path = new PropertyPath("Credits");
        //        barColorBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //        barColorBinding.Converter = new barColConverter();
        //        bar.SetBinding(Shape.FillProperty, barColorBinding);
        //        bar.MouseEnter += (s, ev) =>
        //        {
        //            bar.BeginAnimation(OpacityProperty, glowIn);

        //            ovPage.CreditsHeaderTitle.Text = Convert.ToString(bar.Tag) + ":   " + CharList.Find(x => x.Name.Equals(bar.Tag)).Credits;


        //        };
        //        ovPage.CreditsGraph.MouseLeave += (s, ev) =>
        //        {
        //            ovPage.CreditsHeaderTitle.Text = "Credits Graph";
        //        };
        //        bar.MouseLeave += (s, ev) =>
        //        {
        //            bar.BeginAnimation(OpacityProperty, glowOut);
        //        };

        //        ovPage.CreditsGraph.Children.Add(bar);
        //    }

        //}
        //private void moveTab(double x, double w)
        //{
        //    DoubleAnimationUsingKeyFrames a = new DoubleAnimationUsingKeyFrames();
        //    DoubleAnimationUsingKeyFrames b = new DoubleAnimationUsingKeyFrames();

        //    //var v = x;
           
        //    a.KeyFrames.Add(new SplineDoubleKeyFrame(x, TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
        //    b.KeyFrames.Add(new SplineDoubleKeyFrame(w, TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
        //    t_selTab.BeginAnimation(TranslateTransform.XProperty, a);
        // //   selectedTab.BeginAnimation(System.Windows.Shapes.Rectangle.WidthProperty, b);
        //}
        //private void movePanel(int c)
        //{
        //    DoubleAnimationUsingKeyFrames a = new DoubleAnimationUsingKeyFrames();
        //    var v = -630 * (c -1);
        //    a.KeyFrames.Add(new SplineDoubleKeyFrame(v, TimeSpan.FromMilliseconds(300), new KeySpline(.5, 0, .3, 1)));
        //    t_panel.BeginAnimation(TranslateTransform.XProperty, a);

        //}
        public void addDungHeader(int i)
        {
            var a = FindChild<dungeonsPage>(Application.Current.MainWindow, "dgPage");
            int tc = 0;
            if (Properties.Settings.Default.TeraClub)
            {
                tc = 2;
            }
            else { tc = 1; }

            TextBlock t = new TextBlock();
            t.Text = TeraLogic.DungList[i].ShortName.ToString();
            t.Height = 20;
            t.Margin = new Thickness(0);
            t.VerticalAlignment = VerticalAlignment.Stretch;
            t.Width = (a.ActualWidth - a.dungHeaderGrid.ColumnDefinitions[1].ActualWidth - 5) / TeraLogic.DungList.Count;
            t.TextWrapping = TextWrapping.Wrap;
            var c = new System.Windows.Media.Color();
            c.A = 91;
            c.R = 0;
            c.G = 0;
            c.B = 0;
            t.Foreground = new SolidColorBrush(c);
            t.VerticalAlignment = VerticalAlignment.Center;
            t.TextAlignment = TextAlignment.Center;
          //  t.ToolTip = DungList[i].FullName + " (" + DungList[i].MaxBaseRuns * tc + " max runs)";

            Popup p = new Popup();
            Grid g = new Grid();
            StackPanel s = new StackPanel();
            
            p.AllowsTransparency = true;
            var b = new System.Windows.Media.Color();
            b.A = 255;
            b.R = 255;
            b.G = 255;
            b.B = 255;
            s.Background = new SolidColorBrush(b);
            p.Child = g;
            TextBlock tName = new TextBlock();
            TextBlock tMaxRuns = new TextBlock();
            TextBlock tIlvl = new TextBlock();
            TextBlock tGroup = new TextBlock();
            g.Effect =
            new DropShadowEffect
            {
                Color = new System.Windows.Media.Color { A = 255, R = 0, G = 0, B = 0 },
                Direction = 315,
                ShadowDepth = 5,
                Opacity = .25,
                BlurRadius = 15
                
            };

            tName.Text = TeraLogic.DungList[i].FullName;
            tMaxRuns.Text = "Maximum daily runs: " + (TeraLogic.DungList[i].MaxBaseRuns * tc).ToString();
            tIlvl.Text = "Recommended item level: " + TeraLogic.DungList[i].RequiredIlvl.ToString();
            tGroup.Text = "Group size: " + TeraLogic.DungList[i].GroupSize.ToString();

            tName.Margin = new Thickness(5,10,10,2);
            tMaxRuns.Margin = new Thickness(5,0,5,0);
            tIlvl.Margin = new Thickness(5, 0, 5, 0);
            tGroup.Margin = new Thickness(5, 0, 5, 10);
            tName.HorizontalAlignment = HorizontalAlignment.Left;
            tMaxRuns.HorizontalAlignment = HorizontalAlignment.Left;
            tIlvl.HorizontalAlignment = HorizontalAlignment.Left;
            tGroup.HorizontalAlignment = HorizontalAlignment.Left;

            s.Children.Add(tName);
            s.Children.Add(tMaxRuns);
            s.Children.Add(tIlvl);
            s.Children.Add(tGroup);
            g.Children.Add(s);

            g.Margin = new Thickness (20);


            t.MouseEnter += (send, evt) => { if (!p.IsOpen)
                {
                    p.IsOpen = true;
                    p.PlacementTarget = t;
                    p.Placement = PlacementMode.Relative;
                    p.HorizontalOffset = -10;
                    p.VerticalOffset = -10;

                } };
            t.MouseLeave += (send, evt) => { if (p.IsOpen) { p.IsOpen = false; } };

            (a.dungHeaderPanel as StackPanel).Children.Add(t);


        }
        public void checkDungList(List<Character> chList, List<Dungeon> dgList)
        {
            bool found = false;

            for (int i = 0; i < chList.Count; i++)
            {
                for (int j = 0; j < dgList.Count; j++)
                {
                    found = false;

                    for (int h = 0; h < chList[i].Dungeons.Count; h++)
                    {
                        if(chList[i].Dungeons[h].Name == dgList[j].ShortName)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        chList[i].Dungeons.Insert(j, new CharDungeon(dgList[j].ShortName, 0));
                    }
                }   
            }

            found = false;

            for (int i = 0; i < chList.Count; i++)
            {
                for (int h = 0; h < chList[i].Dungeons.Count; h++)
                {
                    found = false;

                    for (int j = 0; j < dgList.Count; j++)
                    {
                        if (chList[i].Dungeons[h].Name == dgList[j].ShortName)
                        {
                            found = true;
                            break;
                        }

                    }

                    if (!found)
                    {
                        chList[i].Dungeons.RemoveAt(h);
                    }

                }
            }


        }

        public void updateLog(string txt)
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
                        Log.Items.Add(li); //Insert(0,li);
                        Log.ScrollIntoView(li);
                    }
                }
                else
                {
                    Log.Items.Add(li); //Insert(0,li);
                    Log.ScrollIntoView(li);

                }

            }

            ));
            

        }
        public void setGuildImg(Bitmap logo)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                MemoryStream stream = new MemoryStream();

                logo.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                chView.guildLogo.Source = result;


            }));
        }

        #endregion

        #region Event Handlers
        private bool isLogExpanded = false;
        private void expandLog(object sender, MouseButtonEventArgs e)
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            /*loads data from XML*/

            if (TeraLogic.CharList != null)
            {
                checkDungList(TeraLogic.CharList, TeraLogic.DungList);
            }



            /*fills laurels array*/
            for (int i = 0; i < TeraLogic.CharList.Count; i++)
            {
                laurels.Add(new Laurel());
                laurels[i].color = laurels[i].getHexColorFromName(TeraLogic.CharList[i].Laurel);
            }

            /*creates strips and Mstrips*/
            createNewStrips();

            /*creates transforms for tab movement*/
            var tcOn = new ThicknessAnimationUsingKeyFrames();
            var tcOff = new ThicknessAnimationUsingKeyFrames();
            var tcOnFill = new ColorAnimation(System.Windows.Media.Color.FromArgb(255, 255, 255, 255), System.Windows.Media.Color.FromArgb(255, 255, 120, 42), TimeSpan.FromMilliseconds(220));
            var tcOffFill = new ColorAnimation(System.Windows.Media.Color.FromArgb(255, 255, 120, 42), System.Windows.Media.Color.FromArgb(255, 255, 255, 255), TimeSpan.FromMilliseconds(220));
            var tcOnBackFill = new ColorAnimation(System.Windows.Media.Color.FromArgb(25, 0, 0, 0), System.Windows.Media.Color.FromArgb(100, 255, 120, 42), TimeSpan.FromMilliseconds(220));
            var tcOffBackFill = new ColorAnimation(System.Windows.Media.Color.FromArgb(100, 255, 120, 42), System.Windows.Media.Color.FromArgb(25, 0, 0, 0), TimeSpan.FromMilliseconds(220));

            tcOn.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(20, 0, 0, 0), TimeSpan.FromMilliseconds(220), new KeySpline(.5, 0, .3, 1)));
            tcOff.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(-20, 0, 0, 0), TimeSpan.FromMilliseconds(220), new KeySpline(.5, 0, .3, 1)));



            if (!Properties.Settings.Default.TeraClub)
            {
                leftSlide1.tc_switch.BeginAnimation(MarginProperty, tcOff);
                leftSlide1.tc_switch.Fill.BeginAnimation(SolidColorBrush.ColorProperty, tcOffFill);
                leftSlide1.tc_switch_back.Fill.BeginAnimation(SolidColorBrush.ColorProperty, tcOffBackFill);
            }
            else
            {
                leftSlide1.tc_switch.BeginAnimation(MarginProperty, tcOn);
                leftSlide1.tc_switch.Fill.BeginAnimation(SolidColorBrush.ColorProperty, tcOnFill);
                leftSlide1.tc_switch_back.Fill.BeginAnimation(SolidColorBrush.ColorProperty, tcOnBackFill);
            }

            TeraLogic.IsSaved = true;
            int tc = 1;
            if (Properties.Settings.Default.TeraClub)
            {
                tc = 2;
            }
            foreach (var dg in TeraLogic.DungList)
            {
                dgCounter d = new dgCounter();
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
            TCTNotifier.NotificationProvider.NS.sendNotification("TCT is running.");

            this.Activate();

        }


        private void saveButtonClicked(object sender, RoutedEventArgs e)
        {
            TeraLogic.saveCharsToXml();
            TeraLogic.saveGuildDB();

            TeraLogic.IsSaved = true;
        }
        private void tab_Clicked(object sender, MouseButtonEventArgs e)
        {

            TextBlock s = (sender) as TextBlock;
            var c = Grid.GetColumn(s);
            double x = 0;
            for (int i = c - 1; i > 0; i--)
            {
       //         x = x + TabHeader.ColumnDefinitions[i].ActualWidth;
            }
            var w = s.ActualWidth;

            //Application.Current.Dispatcher.BeginInvoke((Action)((() => moveTab(x, w))), null);
          //  Application.Current.Dispatcher.BeginInvoke((Action)((() => movePanel(c))), null);

        }
        private void rejectDrop(object sender, DragEventArgs e)
        {
            if (e.Source.GetType().ToString() != new newStrip().GetType().ToString())
            {
                ovPage.tableGridContent.Items.Clear(); /*newCharBox*/

                for (int i = 0; i < TeraLogic.CharList.Count; i++)
                {
                    ovPage.tableGridContent.Items.Add(NewStrips[i]);

                }
                foreach (newStrip item in ovPage.tableGridContent.Items)
                {
                    (item.Content as Grid).BeginAnimation(HeightProperty, expand);
                    
                }

            }

        }
        int i = 0;
        private void newCharButtonClicked(object sender, MouseButtonEventArgs e)
        {
            string teststring = "Test";

           TCTNotifier.NotificationProvider.NS.sendNotification(teststring, TCTNotifier.NotificationType.Credits, Colors.LightGreen);
           i++;
        }
        Popup dgWindow = new Popup();
        private void showLeftSlide(object sender, MouseButtonEventArgs e)
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
        private void closeLeftSlide(object sender, MouseButtonEventArgs e)
        {
        }
        public void resetDailyData(object sender, RoutedEventArgs e)
        {
            /*resets dungeons runs*/
            int tc = 1;
            if (Properties.Settings.Default.TeraClub)
            {
                tc = 2;
            }
            foreach (var c in TeraLogic.CharList)
            {
                int i = 0;
                foreach (var d in c.Dungeons)
                {
                    if (d.Name.Equals("CA") || d.Name.Equals("AH") || d.Name.Equals("GL") || d.Name.Equals("EA"))
                    {
                        d.Runs = TeraLogic.DungList[i].MaxBaseRuns;
                    }
                    else
                    {
                        d.Runs = TeraLogic.DungList[i].MaxBaseRuns * tc;
                    }
                    i++;
                }
            }

            /*reset dailies*/
            foreach (var c in TeraLogic.CharList)
            {
                c.Dailies = 8;
            }

        }
        public void resetWeeklyData(object sender, RoutedEventArgs e)
        {
            foreach (var c in TeraLogic.CharList)
            {
                c.Weekly = 0;
            }
        }

        private void showDungeonWindow(object sender, RoutedEventArgs e)
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
                            if (Properties.Settings.Default.TeraClub)
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
                                Converter = new DgFillColorConverter(),
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

        #region Title Bar Buttons
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void RedCloseButton(object sender, MouseEventArgs e)
        {
            //CloseButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 210, 30, 30));
        }
        private void NormalCloseButton(object sender, MouseEventArgs e)
        {
            //CloseButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 210, 30, 30));

        }
        private void PressedCloseButton(object sender, MouseButtonEventArgs e)
        {
            //CloseButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 255, 255, 255));

        }
        private void NormalCloseButton(object sender, MouseButtonEventArgs e)
        {
            //CloseButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 210, 30, 30));

        }

        private void Close(object sender, MouseButtonEventArgs e)
        {
            if (TeraLogic.IsSaved)
            {
                Storyboard sb = (Storyboard)TryFindResource("sb");
                sb.Completed += (o, s) => Application.Current.Shutdown();
                sb.Begin(this);
            }
            else
            {
                TeraMainWindow d = new TeraMainWindow();
                d.Content = new exitDiag();


                d.Owner = this;
                d.WindowStyle = WindowStyle.None;
                d.AllowsTransparency = true;
                d.SizeToContent = SizeToContent.WidthAndHeight;
                d.Background = null;
                d.Topmost = true;
                d.ShowInTaskbar = false;
                d.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var c = new System.Windows.Shapes.Rectangle();
                c.HorizontalAlignment = HorizontalAlignment.Stretch;
                c.VerticalAlignment = VerticalAlignment.Stretch;
                c.Opacity = 0;
                c.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,0,0,0));
                Grid.SetRowSpan(c, MainGrid.RowDefinitions.Count);

                this.MainGrid.Children.Add(c);
                var an = new DoubleAnimation(.3, TimeSpan.FromMilliseconds(200));
                var an2 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200));
                d.Opacity = 0;
                d.Show();
                d.BeginAnimation(OpacityProperty, an2);
                c.BeginAnimation(OpacityProperty, an);

                this.IsEnabled = false;
            }

        }
        private void HoverMinButton(object sender, MouseEventArgs e)
        {
            //MinButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 255, 255, 255));
        }
        private void NormalMinButton(object sender, MouseEventArgs e)
        {
            //MinButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 210, 30, 30));

        }
        private void PressedMinButton(object sender, MouseButtonEventArgs e)
        {
            //MinButtonBG.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(60, 255, 255, 255));

        }
        private void Minimize(object sender, MouseButtonEventArgs e)
        {

            this.WindowState = WindowState.Minimized;
        }
        #endregion


        private void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //exitDiag d = new exitDiag();

            //var c = new System.Windows.Shapes.Rectangle();
            //c.HorizontalAlignment = HorizontalAlignment.Stretch;
            //c.VerticalAlignment = VerticalAlignment.Stretch;
            //c.Opacity = 0;
            //c.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
            //Grid.SetRowSpan(c, MainGrid.RowDefinitions.Count);

            //this.MainGrid.Children.Add(c);
            //var an = new DoubleAnimation(.3, TimeSpan.FromMilliseconds(500));
            //var an2 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));

            //c.BeginAnimation(OpacityProperty, an);
            //d.Owner = this;
            //d.ShowDialog();
            //if (d.Result)
            //{

            TeraLogic.saveCharsToXml();
            TeraLogic.saveGuildDB();
            TCTProps.Top = this.Top;
            TCTProps.Left = this.Left;
            TCTProps.Height = this.Height;
            TCTProps.Width = this.Width;


            //this.Close();
            //Environment.Exit(0);

            //}
            //else
            //{
            //    e.Cancel = true;
            //    an2.Completed += (se, ev) =>
            //    {
            //        MainGrid.Children.Remove(c);
            //    };
            //        c.BeginAnimation(OpacityProperty, an2);
            //}


        }


            //if (!TeraLogic.IsSaved)
            //{
            //    var c = new System.Windows.Shapes.Rectangle();
            //    c.HorizontalAlignment = HorizontalAlignment.Stretch;
            //    c.VerticalAlignment = VerticalAlignment.Stretch;
            //    c.Opacity = 0;
            //    c.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
            //    Grid.SetRowSpan(c, MainGrid.RowDefinitions.Count);

            //    this.MainGrid.Children.Add(c);
            //    var an = new DoubleAnimation(.3, TimeSpan.FromMilliseconds(200));
              
            //    c.BeginAnimation(OpacityProperty, an);
            //    d.ShowDialog();
            //    if (d.Result)
            //    {
            //        MainWindow.TeraLogic.saveCharsToXml(MainWindow.CharList);
            //    }
            //}
            //Application.Current.Shutdown();

        

        private void Win_Closing(object sender, EventArgs e)
        {
           
        }
        #endregion

        private void TeraMainWin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

    }

}



