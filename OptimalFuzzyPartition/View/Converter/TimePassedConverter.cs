using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View
{
    public class TimePassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var time = (TimeSpan)value;
            var s = time.ToString(@"mm\:ss\:fff");
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}