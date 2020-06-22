using System;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    [Serializable]
    public class CommandAndData
    {
        public CommandType CommandType;

        public PartitionSettings PartitionSettings;

        public bool AlwaysShowCentersInfo;

        public int IterationNumber;

        public double TargetFunctionalValue;

        public double DualFunctionalValue;

        public bool WorkFinished;

        public byte[] ImageSavePath;

        public bool DrawWithMistrustCoefficient;

        public double MistrustCoefficient;
    }
}