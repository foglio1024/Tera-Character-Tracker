using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using TCTUI.Converters;
using TCTUI.Controls;
using TCTUI.Views;
using TCTData;

namespace TCTUI
{
    public static class DataBinder
    {
        public static void BindCharPropertyToShapeFillColor(int i, string property, Shape t, IValueConverter conv)
        {
            var b = new Binding {
                Source = TCTData.Data.CharList[i],
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
                Source = Data.CharList[i],
                Path = new PropertyPath(prop),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = conv,
                ConverterParameter = definition
            };

            img.SetBinding(System.Windows.Controls.Image.SourceProperty, b);
        }
        public static void BindParameterToOpacityMaskImageSourceWithConverter(int i, string prop, System.Windows.Shapes.Rectangle img, string definition, IValueConverter conv)
        {
            var b = new Binding
            {
                Source = Data.CharList[i],
                Path = new PropertyPath(prop),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = conv,
                ConverterParameter = definition
            };

            img.SetBinding(Shape.OpacityMaskProperty, b);
        }
        public static Binding GenericCharBinding(int i, string prop, IValueConverter conv, object par)
        {
            var b = new Binding
            {
                Source = Data.CharList[i],
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
                Source = Data.CharList[i],
                Path = new PropertyPath(prop),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            return b;
        }
        public static void BindParameterToQuestBarGauge(int i, string propertyW, string propertyD, QuestGauge t, int maxValueW, int thresholdW, int maxValueD, int thresholdD, bool color, bool invert)
        {

            t.barD.SetBinding(Shape.WidthProperty, DataBinder.GenericCharBinding(i, propertyD, new Daily_ValueToBarWidth(), new double[] { t.@base.Width, TCTConstants.MAX_WEEKLY - Data.CharList[i].Weekly }));
            t.barW.SetBinding(Shape.WidthProperty, DataBinder.GenericCharBinding(i, propertyW, new ValueToBarLenght(), new double[] { t.@base.Width, maxValueW }));
            t.txtD.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, propertyD));
            t.txtW.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, propertyW));

            t.barW.SetBinding(Shape.FillProperty, DataBinder.GenericCharBinding(i, "WeeklyBonus", new WeeklyBonusToColor(), Data.CharList[i].Weekly));
            t.barD.SetBinding(Shape.FillProperty, DataBinder.GenericCharBinding(i, "WeeklyBonus", new WeeklyBonusToColor(), Data.CharList[i].Weekly));
            t.borD.SetBinding(Border.BorderBrushProperty, DataBinder.GenericCharBinding(i, "WeeklyBonus", new WeeklyBonusToColor(), Data.CharList[i].Weekly));
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
        public static void CreateDgClearsBindingsForTier(int charIndex, UIElementCollection coll)
        {
            foreach (DungeonClearsCounter dc in coll)
            {
                var counterText = new Binding
                {
                    Source = Data.CharList[charIndex].Dungeons.Find(d => d.Name == dc.Name),
                    Path = new PropertyPath("Clears"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = new IntToString(),
                };

                var fill = new Binding
                {
                    Source = Data.CharList[charIndex].Dungeons.Find(d => d.Name == dc.Name),
                    Path = new PropertyPath("Clears"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = new Dungeon_ClearsToColor(),
                };


                dc.bgRect.SetBinding(Shape.FillProperty, fill);
                dc.clearsText.SetBinding(TextBlock.TextProperty, counterText);
            }

        }
        public static void CreateDgBindingsForTier(int charIndex, UIElementCollection coll)
        {
            foreach (DungeonRunsCounter dc in coll)
            {
                int tc = 1;

                if (Data.AccountList.Find(a => a.Id == Data.CharList[charIndex].AccountId).TeraClub)
                {
                    tc = 2;
                }

                var counterText = new Binding
                {
                    Source = Data.CharList[charIndex].Dungeons.Find(d => d.Name == dc.Name),
                    Path = new PropertyPath("Runs"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.OneWay,
                    Converter = new IntToString(),
                };

                int p = 0;
                if (dc.n.Text == "AH" || dc.n.Text == "EA" || dc.n.Text == "GL" || dc.n.Text == "CA")
                {
                    p = Data.DungList.Find(d => d.ShortName == dc.Name).MaxBaseRuns;
                }
                else
                {
                    p = Data.DungList.Find(d => d.ShortName == dc.Name).MaxBaseRuns * tc;
                }
                var ellipseFill = new Binding
                {
                    Source = Data.CharList[charIndex].Dungeons.Find(d => d.Name == dc.Name),
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

        internal static void CreateDgClearsBindings(int charIndex, CharView w)
        {
            CreateDgClearsBindingsForTier(charIndex, w.t2panelC.Children);
            CreateDgClearsBindingsForTier(charIndex, w.t3panelC.Children);
            CreateDgClearsBindingsForTier(charIndex, w.t4panelC.Children);
            CreateDgClearsBindingsForTier(charIndex, w.t5panelC.Children);

        }

        internal static void BindParameterToArcGauge(int i, string v, CrystalbindIndicator ccbInd, IValueConverter valueToAngle)
        {
            ccbInd.arc.SetBinding(Arc.EndAngleProperty, GenericCharBinding(i, v, valueToAngle, null));
        }
    }
}
