using OptimalFuzzyPartitionAlgorithm.Settings;
using System;

namespace OptimalFuzzyPartitionAlgorithm
{
    [Serializable]
    public class PartitionSettings
    {
        public bool IsCenterPlacingTask;

        public SpaceSettings SpaceSettings;

        public CentersSettings CentersSettings;

        public FuzzyPartitionFixedCentersSettings FuzzyPartitionFixedCentersSettings;

        public FuzzyPartitionPlacingCentersSettings FuzzyPartitionPlacingCentersSettings;

        public RAlgorithmSettings RAlgorithmSettings;

        public PartitionSettings GetCopy()
        {
            var settings = (PartitionSettings)MemberwiseClone();
            


            return settings;
        }
    }
}