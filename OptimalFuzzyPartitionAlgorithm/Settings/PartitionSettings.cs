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

        /// <summary>
        /// Parameter for integration on supply area. Is used for calculating 
        /// target functional and gradient values.
        /// </summary>
        public int GaussLegendreIntegralOrder { get; set; }
    }
}