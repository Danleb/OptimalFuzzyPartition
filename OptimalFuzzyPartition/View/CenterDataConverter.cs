using OptimalFuzzyPartitionAlgorithm.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace OptimalFuzzyPartition.View
{
    public class CenterDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var centerDatas = (List<CenterData>)value;
            var centerDataViews = centerDatas.Select((v, i) => new CenterDataView(v, i)).ToList();
            return centerDataViews;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}