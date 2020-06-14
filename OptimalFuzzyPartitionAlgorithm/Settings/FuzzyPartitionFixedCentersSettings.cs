using System;

namespace OptimalFuzzyPartitionAlgorithm
{
    [Serializable]
    public class FuzzyPartitionFixedCentersSettings
    {
        public int MaxIterationsCount;

        public double GradientStep;

        public double GradientEpsilon;
    }
}