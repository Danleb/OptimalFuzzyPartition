using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View.Converter
{
    public class ThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var currentTheme = (string)value;
            var targetTheme = (string)parameter;
            var isMatch = currentTheme.Contains(targetTheme);
            return isMatch;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
