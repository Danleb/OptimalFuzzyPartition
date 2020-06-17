using MathNet.Numerics.LinearAlgebra;
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
        public void Grid3x3PlacingPartitionTest()
        {
            var f = "Logs.txt";
            if (File.Exists(f))
                File.Delete(f);
            Trace.Listeners.Add(new TextWriterTraceListener(f));

            var settings = new PartitionSettings
            {
                IsCenterPlacingTask = false,
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
                            Position = VectorUtils.CreateVector(2, 5),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(8, 5),
                            IsFixed = true
                        },
                    }
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.01,
                    GradientStep = 0.1,
                    MaxIterationsCount = 400
                },
                FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
                {
                    CentersDeltaEpsilon = 0.1,
                    GaussLegendreIntegralOrder = 32
                },
                RAlgorithmSettings = new RAlgorithmSettings
                {
                    SpaceStretchFactor = 2,
                    H0 = 1,
                    MaxIterationsCount = 30
                }
            };

            var zeroTaus = new List<Vector<double>>
            {
                //VectorUtils.CreateVector(2, 5),
                //VectorUtils.CreateVector(8, 5)
                VectorUtils.CreateVector(1, 1),
                VectorUtils.CreateVector(10, 10)
            };

            var calculator = new FuzzyPartitionFixedCentersAlgorithm(settings);
            var partition = calculator.BuildPartition();
            var muValueGetters = partition.Select(v => new MuValueInterpolator(settings.SpaceSettings, new MuGridValueGetter(v))).ToList();
            var placingAlgorithm = new FuzzyPartitionPlacingCentersAlgorithm(settings, zeroTaus, muValueGetters);

            while (true)
            {
                placingAlgorithm.DoIteration(muValueGetters);

                var centers = placingAlgorithm.GetCenters();

                var targetFunctionalCalculator = new TargetFunctionalCalculator(settings);
                var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);
                Trace.WriteLine($"Target functional value = {targetFunctionalValue}\n");

                for (int i = 0; i < settings.CentersSettings.CentersCount; i++)
                {
                    settings.CentersSettings.CenterDatas[i].Position = centers[i].Clone();
                    Trace.WriteLine($"Center #{i + 1}: {centers[i][0]:0.00} {centers[i][1]:0.00}");
                }

                calculator = new FuzzyPartitionFixedCentersAlgorithm(settings);
                partition = calculator.BuildPartition();

                if (placingAlgorithm.IsStopConditionSatisfied())
                    break;
            }

            var list = placingAlgorithm.GetCenters();
            for (var index = 0; index < list.Count; index++)
            {
                var center = list[index];

                Trace.WriteLine($"Center #{index + 1}: {center[0]:0.00} {center[1]:0.00}");
            }

            Trace.Flush();

            var vec = VectorUtils.CreateVector(44, 4, 4, 4, 4);
            var x = 4;
        }

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
            var muValueGetters = partition.Select(v => new MuValueInterpolator(settings.SpaceSettings, new MuGridValueGetter(v))).ToList();

            var targetFunctionalCalculator = new TargetFunctionalCalculator(settings);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);
            Trace.WriteLine($"Target functional value = {targetFunctionalValue}\n");

            Trace.WriteLine("Center #1 mu matrix:");
            MatrixUtils.TraceMatrix(partition[0]);

            Trace.WriteLine("Center #2 mu matrix:");
            MatrixUtils.TraceMatrix(partition[1]);

            var sum = partition.Aggregate((a, b) => a + b);
            Trace.WriteLine("Sum mu matrix:");
            MatrixUtils.TraceMatrix(sum);
            Trace.Flush();

            var vec = VectorUtils.CreateVector(44, 4, 4, 4, 4);
            var x = 4;
        }
    }
}