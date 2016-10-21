using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Tera.Converters;

namespace Tera
{
    public static class DataBinder
    {
        public static void BindCharPropertyToShapeFillColor(int i, string property, Shape t, IValueConverter conv)
        {
            var b = new Binding {
                Source = TeraLogic.CharList[i],
                Path = new PropertyPath(property),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = conv
            };

            t.SetBinding(Shape.FillProperty, b);
        }
        public static void BindParameterToImageSourceWithConverter(int i, string prop, System.Windows.Controls.Image img, string definition, IValueConverter conv)
        {
            var b = new Binding
            {
                Source = TeraLogic.CharList[i],
                Path = new PropertyPath(prop),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = conv,
                ConverterParameter = definition
            };

            img.SetBinding(System.Windows.Controls.Image.SourceProperty, b);
        }
        public static Binding GenericCharBinding(int i, string prop, IValueConverter conv, object par)
        {
            var b = new Binding
            {
                Source = TeraLogic.CharList[i],
                Path = new PropertyPath(prop),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = conv,
                ConverterParameter = par
            };

            return b;
        }
        public static Binding GenericCharBinding(int i, string prop)
        {
            var b = new Binding
            {
                Source = TeraLogic.CharList[i],
                Path = new PropertyPath(prop),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            return b;
        }
        public static void BindParameterToQuestBarGauge(int i, string propertyW, string propertyD, QuestGauge t, int maxValueW, int thresholdW, int maxValueD, int thresholdD, bool color, bool invert)
        {

            t.barD.SetBinding(Shape.WidthProperty, DataBinder.GenericCharBinding(i, propertyD, new Daily_ValueToBarWidth(), new double[] { t.@base.Width, TeraLogic.MAX_WEEKLY - TeraLogic.CharList[i].Weekly }));
            t.barW.SetBinding(Shape.WidthProperty, DataBinder.GenericCharBinding(i, propertyW, new ValueToBarLenght(), new double[] { t.@base.Width, maxValueW }));
            t.txtD.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, propertyD));
            t.txtW.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, propertyW));

            if (color)
            {
                t.barW.SetBinding(Shape.FillProperty, DataBinder.GenericCharBinding(i, propertyW, new ProgressToColor(), new object[] { maxValueW, thresholdW, invert }));
                t.barD.SetBinding(Shape.FillProperty, DataBinder.GenericCharBinding(i, propertyW, new ProgressToColor(), new object[] { maxValueW, thresholdW, invert }));
                t.borD.SetBinding(Border.BorderBrushProperty, DataBinder.GenericCharBinding(i, propertyW, new ProgressToColor(), new object[] { maxValueW, thresholdW, invert }));
            }
        }
        public static void BindParameterToBarGauge(int i, string property, BarGauge t, int maxValue, int threshold, bool color, bool invert)
        {
            t.val.SetBinding(Shape.WidthProperty, GenericCharBinding(i, property, new ValueToBarLenght(), new double[] { t.@base.Width, maxValue }) );

            if (color)
            {
                t.val.SetBinding(Shape.FillProperty, GenericCharBinding(i, property,new ProgressToColor(), new object[] { maxValue, threshold, invert }));
            }

            t.txt.SetBinding(TextBlock.TextProperty, GenericCharBinding(i, property));
        }
        public static void CreateDgBindings(int charIndex, CharView w)
        {
            CreateDgBindingsForTier(charIndex, w.t2panel.Children);
            CreateDgBindingsForTier(charIndex, w.t3panel.Children);
            CreateDgBindingsForTier(charIndex, w.tier4panel.Children);
            CreateDgBindingsForTier(charIndex, w.tier5panel.Children);
            CreateDgBindingsForTier(charIndex, w.soloPanel.Children);
        }
        public static void CreateDgBindingsForTier(int charIndex, UIElementCollection coll)
        {
            foreach (DungeonRunsCounter dc in coll)
            {
                int tc = 1;

                if (TeraLogic.AccountList.Find(a => a.Id == TeraLogic.CharList[charIndex].AccountId).TeraClub)
                {
                    tc = 2;
                }



                //int dgIndex = TeraLogic.CharList[charIndex].Dungeons.IndexOf(TeraLogic.CharList[charIndex].Dungeons.Find(d => d.Name.Equals(dc.Name)));

                //var counterText = new Binding
                //{
                //    Source = TeraLogic.CharList[charIndex].Dungeons[dgIndex],
                //    Path = new PropertyPath("Runs"),
                //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                //    Mode = BindingMode.OneWay,
                //    Converter = new IntToString(),
                //};
                var counterText = new Binding
                {
                    Source = TeraLogic.CharList[charIndex].Dungeons.Find(d => d.Name == dc.Name),
                    Path = new PropertyPath("Runs"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = new IntToString(),
                };

                //int p = 0;
                //if (dc.n.Text == "AH" || dc.n.Text == "EA" || dc.n.Text == "GL" || dc.n.Text == "CA")
                //{
                //    p = TeraLogic.DungList[dgIndex].MaxBaseRuns;
                //}
                //else
                //{
                //    p = TeraLogic.DungList[dgIndex].MaxBaseRuns * tc;
                //}
                int p = 0;
                if (dc.n.Text == "AH" || dc.n.Text == "EA" || dc.n.Text == "GL" || dc.n.Text == "CA")
                {
                    p = TeraLogic.DungList.Find(d => d.ShortName == dc.Name).MaxBaseRuns;
                }
                else
                {
                    p = TeraLogic.DungList.Find(d => d.ShortName == dc.Name).MaxBaseRuns * tc;
                }
                var ellipseFill = new Binding
                {
                    Source = TeraLogic.CharList[charIndex].Dungeons.Find(d => d.Name == dc.Name),
                    Path = new PropertyPath("Runs"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = new Dungeon_RunsToColor(),
                    ConverterParameter = p
                };


                dc.ell.SetBinding(Shape.FillProperty, ellipseFill);
                dc.t.SetBinding(TextBlock.TextProperty, counterText);
            }
        }

        internal static void BindParameterToArcGauge(int i, string v, CrystalbindIndicator ccbInd, IValueConverter valueToAngle)
        {
            ccbInd.arc.SetBinding(Arc.EndAngleProperty, GenericCharBinding(i, v, valueToAngle, null));
        }
    }
}
