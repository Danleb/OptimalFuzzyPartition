using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithm.Settings
{
    [Serializable]
    public class CentersSettings
    {
        [JsonIgnore]
        public int CentersCount => CenterDatas.Count;

        [JsonIgnore]
        public int FixedCentersCount => CenterDatas.Where(v => v.IsFixed).Count();

        [JsonIgnore]
        public int PlacingCentersCount => CentersCount - FixedCentersCount;

        public List<CenterData> CenterDatas = new List<CenterData>();
    }
}
