using System;
using System.Globalization;
using System.Windows.Data;

namespace Tera
{
    internal class DgRunsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var maxRuns = (int)parameter;
            var rawRuns = (int)value;
            int remainingRuns = maxRuns - rawRuns;
            return rawRuns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}