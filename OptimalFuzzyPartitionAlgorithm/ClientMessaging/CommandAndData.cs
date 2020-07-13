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

        public byte[] ImageSavePath;

        public bool DrawWithMistrustCoefficient;

        public double MistrustCoefficient;
    }
}