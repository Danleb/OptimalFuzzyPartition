using System;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    [Serializable]
    public class CommandAndData
    {
        public CommandType CommandType;

        public PartitionSettings PartitionSettings;
    }
}