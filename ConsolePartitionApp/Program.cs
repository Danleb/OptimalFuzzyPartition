using System.Collections.Generic;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;

namespace ConsolePartitionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var partitionSettings = new PartitionSettings
            {
                AdditiveCoefficients = new List<double> { 0, 0 },
                MultiplicativeCoefficients = new List<double> { 0, 0 },
                CentersCount = 2,
                CentersDeltaEpsilon = 0.01,
                Density = vector => 1,
                Distance = (vector, vector2) => (vector - vector2).L2Norm(),
                H0 = 0.4,
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(10, 10),
                GridSize = new List<int> { 100, 100 },
                //ExponentialWeight = 2,
                IsCenterPlacingTask = true,
                MaxIterationsCount = 1000,
            };

            var partition = new Partition(partitionSettings);

            partition.CreatePartition();
        }
    }
}