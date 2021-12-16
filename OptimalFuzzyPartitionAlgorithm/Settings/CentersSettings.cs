using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithm.Settings
{
    [Serializable]
    public class CentersSettings
    {
        public int CentersCount => CenterDatas.Count;
        public int FixedCentersCount => CenterDatas.Where(v => v.IsFixed).Count();
        public int PlacingCentersCount => CentersCount - FixedCentersCount;

        public List<CenterData> CenterDatas;
    }
}