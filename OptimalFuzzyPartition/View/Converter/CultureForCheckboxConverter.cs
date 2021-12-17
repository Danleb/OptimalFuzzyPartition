using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View.Converter
{
    public class CultureForCheckboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var currentCulture = (CultureInfo)value;
            var targetLanguage = (string)parameter;
            var isMatch = currentCulture.Name.Substring(0, 2) == targetLanguage;
            return isMatch;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
