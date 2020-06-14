using OptimalFuzzyPartition.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View
{
    public class MetricsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var metricsType = (MetricsType)value;

            switch (metricsType)
            {
                case MetricsType.Euclidean:
                    return "Евклідова метрика";
                case MetricsType.Manhattan:
                    return "Манхеттенська метрика";
                case MetricsType.Chebyshev:
                    return "Метрика Чебишева";
                case MetricsType.CustomFunction:
                    return "Довільно вказана функція відстані";
                default:
                    return "Не вказано";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}