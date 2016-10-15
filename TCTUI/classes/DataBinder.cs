using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Tera.classes;

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

            t.barD.SetBinding(Shape.WidthProperty, DataBinder.GenericCharBinding(i, propertyD, new DailybarLengthConverter(), new double[] { t.@base.Width, TeraLogic.MAX_WEEKLY - TeraLogic.CharList[i].Weekly }));
            t.barW.SetBinding(Shape.WidthProperty, DataBinder.GenericCharBinding(i, propertyW, new barLengthConverter(), new double[] { t.@base.Width, maxValueW }));
            t.txtD.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, propertyD));
            t.txtW.SetBinding(TextBlock.TextProperty, DataBinder.GenericCharBinding(i, propertyW));

            if (color)
            {
                t.barW.SetBinding(Shape.FillProperty, DataBinder.GenericCharBinding(i, propertyW, new ProgressToColorConverter(), new object[] { maxValueW, thresholdW, invert }));
                t.barD.SetBinding(Shape.FillProperty, DataBinder.GenericCharBinding(i, propertyW, new ProgressToColorConverter(), new object[] { maxValueW, thresholdW, invert }));
                t.borD.SetBinding(Border.BorderBrushProperty, DataBinder.GenericCharBinding(i, propertyW, new ProgressToColorConverter(), new object[] { maxValueW, thresholdW, invert }));
            }
        }
        public static void BindParameterToBarGauge(int i, string property, BarGauge t, int maxValue, int threshold, bool color, bool invert)
        {
            t.val.SetBinding(Shape.WidthProperty, GenericCharBinding(i, property, new barLengthConverter(), new double[] { t.@base.Width, maxValue }) );

            if (color)
            {
                t.val.SetBinding(Shape.FillProperty, GenericCharBinding(i, property,new ProgressToColorConverter(), new object[] { maxValue, threshold, invert }));
            }

            t.txt.SetBinding(TextBlock.TextProperty, GenericCharBinding(i, property));
        }

    }
}
