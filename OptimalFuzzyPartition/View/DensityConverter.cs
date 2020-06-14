using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View
{
    public class DensityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var density = (DensityType)value;

            switch (density)
            {
                case DensityType.Everywhere1:
                    return "Тотожна одиниця";
                case DensityType.CustomFunction:
                    break;
                case DensityType.ByPointsGrid:
                    break;
                default:
                    return "Не задано";
            }

            return "Не задано";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}