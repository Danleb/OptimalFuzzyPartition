using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;

namespace OFP_AlgorithmTests
{
    public class FuzzyPartitionFixedCentersTests
    {
        [Test]
        public void TwoPointsTest()
        {
            var settings = new PartitionSettings
            {
                AdditiveCoefficients = new List<double>
                {
                    1,
                    2
                },
                MultiplicativeCoefficients = new List<double>
                {
                    2,
                    3
                },
                CenterPositions = new List<Vector<double>>
                {
                    VectorUtils.CreateVector(3.33, 5),
                    VectorUtils.CreateVector(6.66, 5),
                },
                Density = vector => 1,
                CentersCount = 2,
                Distance = (point1, point2) => (point1 - point2).L2Norm(),
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(10, 10),
                IsCenterPlacingTask = false,
                MaxIterationsCount = 1000,
                GridSize = new List<int> { 100, 100 },

            };

            var stepFunction = new Func<int, double>(k => 1);

            var partition = new FuzzyPartitionFixedCenters(settings, stepFunction);
            partition.CreatePartition();
        }
    }
}