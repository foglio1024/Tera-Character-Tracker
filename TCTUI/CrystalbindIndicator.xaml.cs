using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Tera
{
    /// <summary>
    /// Logica di interazione per CrystalbindIndicator.xaml
    /// </summary>
    public partial class CrystalbindIndicator : UserControl
    {
        public CrystalbindIndicator()
        {
            InitializeComponent();
            arc.Stroke = SystemParameters.WindowGlassBrush;
            var b = new Binding
            {
                Source = arc,
                Path = new PropertyPath("EndAngle"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new CcbTime_AngleToColor(),
            };
            led.SetBinding(Shape.FillProperty, b);
            arc.SetBinding(Shape.StrokeProperty, b);
        }

        private class CcbTime_AngleToColor : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var v = (double)value;
                double th = 359.999 / 12;
                if (v < th && v > 0)
                {
                    return new SolidColorBrush(TeraLogic.TCTProps.accentColor);
                }
                else if(v == 0)
                {
                    return new SolidColorBrush(Color.FromArgb(50,0,0,0));
                }
                else return new SolidColorBrush(SystemParameters.WindowGlassColor);

            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
