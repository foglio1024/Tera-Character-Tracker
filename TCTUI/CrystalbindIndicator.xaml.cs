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
            arc.Stroke = new SolidColorBrush(UI.Colors.SolidBaseColor);
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
                    return new SolidColorBrush(UI.Colors.SolidAccentColor);
                }
                else if(v == 0)
                {
                    return new SolidColorBrush(UI.Colors.FadedGray);
                }
                else return new SolidColorBrush(UI.Colors.SolidBaseColor);

            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
