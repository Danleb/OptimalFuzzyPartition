using OptimalFuzzyPartitionAlgorithm.Settings;
using System;

namespace OptimalFuzzyPartitionAlgorithm
{
    [Serializable]
    public class PartitionSettings
    {
        public bool CalculateTargetFunctionalValue;

        public bool CalculateDualFunctionalValue;

        public bool IsCenterPlacingTask;

        public SpaceSettings SpaceSettings;

        public CentersSettings CentersSettings;

        public FuzzyPartitionFixedCentersSettings FuzzyPartitionFixedCentersSettings;

        public FuzzyPartitionPlacingCentersSettings FuzzyPartitionPlacingCentersSettings;

        public RAlgorithmSettings RAlgorithmSettings;
    }
}