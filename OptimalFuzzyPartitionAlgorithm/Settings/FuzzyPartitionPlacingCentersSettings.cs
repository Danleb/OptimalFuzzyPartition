using System;

namespace OptimalFuzzyPartitionAlgorithm.Settings
{
    /// <summary>
    /// Settings for the solving the fuzzy partition task with optimal centers placing.
    /// </summary>
    [Serializable]
    public class FuzzyPartitionPlacingCentersSettings
    {
        public double CentersDeltaEpsilon;
    }
}