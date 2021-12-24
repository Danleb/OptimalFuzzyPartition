using OptimalFuzzyPartition.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View.Converter
{
    public class MetricsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is string) return string.Empty;

            var metricsType = (MetricsType)value;

            switch (metricsType)
            {
                case MetricsType.Euclidean:
                    return App.Localize("EuclideanMetric");
                case MetricsType.Manhattan:
                    return App.Localize("ManhattanMetric");
                case MetricsType.Chebyshev:
                    return App.Localize("ChebishevMetric");
                case MetricsType.CustomFunction:
                    return App.Localize("CustomDistanceFunction");
                default:
                    return "NONE";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
