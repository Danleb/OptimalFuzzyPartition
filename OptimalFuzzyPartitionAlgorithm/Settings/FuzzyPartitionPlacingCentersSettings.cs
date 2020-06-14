using System;

namespace OptimalFuzzyPartitionAlgorithm.Settings
{
    /// <summary>
    /// Settings for the 
    /// </summary>
    [Serializable]
    public class FuzzyPartitionPlacingCentersSettings
    {
        public int MaxIterationsCount;

        public double CentersDeltaEpsilon;
    }
}