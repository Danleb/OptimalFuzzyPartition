using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View.Converter
{
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            if (str.Count(v => v == '.') == 1)
            {
                var dotIndex = str.IndexOf('.');
                if (dotIndex == -1)
                {
                    return value;
                }

                var tail = str.Substring(dotIndex + 1);
                if (tail.All(v => v == '0'))
                {
                    return ".";
                }

                return value;
            }

            // return an invalid value in case of the value ends with a point
            // value.ToString().EndsWith(".") ? "." : value;
            return value;
        }
    }
}
