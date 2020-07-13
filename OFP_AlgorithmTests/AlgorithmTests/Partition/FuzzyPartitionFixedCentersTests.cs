using NUnit.Framework;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithmTests.AlgorithmTests
{
    public class FuzzyPartitionFixedCentersTests
    {
        [Test]
        public void MuGridsFixedPartition3x3Test()
        {
            const string f = "Logs.txt";
            if (File.Exists(f))
                File.Delete(f);
            Trace.Listeners.Add(new TextWriterTraceListener(f));

            var settings = new PartitionSettings
            {
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(0, 0),
                    MaxCorner = VectorUtils.CreateVector(10, 10),
                    GridSize = new List<int> { 8, 8 },
                    DensityType = DensityType.Everywhere1,
                    MetricsType = MetricsType.Euclidean,
                    CustomDensityFunction = null,
                    CustomDistanceFunction = null
                },
                CentersSettings = new CentersSettings
                {
                    CentersCount = 2,
                    CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(3.33, 5),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(6.66, 5),
                            IsFixed = true
                        },
                    }
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.01,
                    GradientStep = 0.1,
                    MaxIterationsCount = 400
                }
            };

            var calculator = new FuzzyPartitionFixedCentersAlgorithm(settings);
            var partition = calculator.BuildPartition();
            var muValueGetters = partition.Select(v => new GridValueInterpolator(settings.SpaceSettings, new MatrixGridValueGetter(v))).ToList();

            var targetFunctionalCalculator = new TargetFunctionalCalculator(settings);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);
            Trace.WriteLine($"Target functional value = {targetFunctionalValue}\n");

            //Trace.WriteLine("Center #1 mu matrix:");
            //MatrixUtils.WriteMatrix(partition[0]);

            //Trace.WriteLine("Center #2 mu matrix:");
            //MatrixUtils.WriteMatrix(partition[1], );

            //var sum = partition.Aggregate((a, b) => a + b);
            //Trace.WriteLine("Sum mu matrix:");
            //MatrixUtils.WriteMatrix(sum);
            //Trace.Flush();

            var vec = VectorUtils.CreateVector(0, 0);
        }
    }
}