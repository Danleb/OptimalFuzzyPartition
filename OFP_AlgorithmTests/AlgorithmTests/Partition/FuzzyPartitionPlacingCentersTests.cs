using NUnit.Framework;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OptimalFuzzyPartitionAlgorithmTests.AlgorithmTests.Partition
{
    public record FinalPartitionData(List<CenterData> Centers, double TargetFunctionalValue);
    public record FuzzyPartitionTestData(PartitionSettings Settings, FinalPartitionData Expected);

    public static class FuzzyPartitionSettingsBuilder
    {
        public static FuzzyPartitionTestData _1_Center()
        {
            var finalPartitionTestData = new FinalPartitionData(new List<CenterData>
            {
                MakeCenter(0.5, 0.5)
            }, 0.38243711);
            return new FuzzyPartitionTestData(GenerateSettings(1), finalPartitionTestData);
        }

        public static FuzzyPartitionTestData _3_Center()
        {
            var finalPartitionTestData = new FinalPartitionData(new List<CenterData>
            {
                MakeCenter(0.5, 0.3),
                MakeCenter(0.3, 0.7),
                MakeCenter(0.7, 0.7),
                MakeCenter(0.5, 0.3),
            }, 0.11);
            return new FuzzyPartitionTestData(GenerateSettings(1), finalPartitionTestData);
        }

        public static FuzzyPartitionTestData _5_Center()
        {
            var finalPartitionTestData = new FinalPartitionData(new List<CenterData>
            {
                //MakeCenter(0.5, 0.3),
                //MakeCenter(0.3, 0.7),
                //MakeCenter(0.7, 0.7),
                //MakeCenter(0.5, 0.3),
                MakeCenter(0.5, 0.5),
            }, 0.11);
            return new FuzzyPartitionTestData(GenerateSettings(1), finalPartitionTestData);
        }

        private static CenterData MakeCenter(double x, double y)
        {
            return new CenterData
            {
                A = 0,
                W = 1.0,
                IsFixed = false,
                Position = VectorUtils.CreateVector(x, y)
            };
        }

        private static PartitionSettings GenerateSettings(int centersCount)
        {
            var settings = new PartitionSettings
            {
                IsCenterPlacingTask = true,
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(0, 0),
                    MaxCorner = VectorUtils.CreateVector(1, 1),
                    GridSize = new List<int> { 50, 50 },
                    DensityType = DensityType.Everywhere1,
                    MetricsType = MetricsType.Euclidean,
                    CustomDensityFunction = null,
                    CustomDistanceFunction = null
                },
                CentersSettings = new CentersSettings
                {
                    CenterDatas = new List<CenterData>()
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.001,
                    GradientStep = 1,//0.1
                    MaxIterationsCount = 1000
                },
                FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
                {
                    CentersDeltaEpsilon = 0.001,
                },
                RAlgorithmSettings = new RAlgorithmSettings
                {
                    SpaceStretchFactor = 2,
                    H0 = 0.7,
                    MaxIterationsCount = 100
                },
                GaussLegendreIntegralOrder = 32
            };

            for (var i = 0; i < centersCount; i++)
            {
                var data = new CenterData
                {
                    A = 0,
                    W = 1,
                    Position = VectorUtils.CreateVector(0, 0),
                    IsFixed = false
                };
                settings.CentersSettings.CenterDatas.Add(data);
            }

            return settings;
        }
    }

    public class FuzzyPartitionPlacingCentersTests
    {
        public const double PositionDelta = 0.01;

        public readonly FuzzyPartitionTestData _1_CenterData = FuzzyPartitionSettingsBuilder._1_Center();
        public readonly FuzzyPartitionTestData _3_CenterData = FuzzyPartitionSettingsBuilder._3_Center();
        public readonly FuzzyPartitionTestData _5_CenterData = FuzzyPartitionSettingsBuilder._5_Center();

        [TestCaseSource(nameof(_1_CenterData))]
        public void _1_Center(FuzzyPartitionTestData testData)
        {
            ExecuteTest(testData);
        }

        [TestCaseSource(nameof(_3_CenterData))]
        public void _3_Center(FuzzyPartitionTestData testData)
        {
            ExecuteTest(testData);
        }

        [TestCaseSource(nameof(_5_CenterData))]
        public void _5_Center(FuzzyPartitionTestData testData)
        {
            ExecuteTest(testData);
        }

        private void ExecuteTest(FuzzyPartitionTestData testData)
        {
            var expected = testData.Expected;
            var actual = ExecutePlacingAlgorithm(testData.Settings);
            var used = new Dictionary<CenterData, bool>();

            //Assert.AreEqual(expected.TargetFunctionalValue, actual.TargetFunctionalValue);
            for (var i = 0; i < expected.Centers.Count; i++)
            {
                var expectedCenter = expected.Centers[i];

                // In case when there are several centers with the same characteritics, there are several interchangable partitions possible.
                var nearestData = actual.Centers.MinElement(v => (v.Position - expectedCenter.Position).L2Norm());

                Assert.False(used[nearestData]);

                used[nearestData] = true;

                Assert.AreEqual(expectedCenter.Position[0], nearestData.Position[0], PositionDelta);
                Assert.AreEqual(expectedCenter.Position[1], nearestData.Position[1], PositionDelta);
            }
        }

        private void TraceCentersData(List<CenterData> centers)
        {
            for (var i = 0; i < centers.Count; i++)
            {
                Trace.WriteLine($"Center #{i + 1}: {centers[i].Position[0]:0.00} {centers[i].Position[1]:0.00}");
            }
            Trace.Flush();
        }

        private FinalPartitionData ExecutePlacingAlgorithm(PartitionSettings settings)
        {
            var f = "Logs.txt";
            if (File.Exists(f))
                File.Delete(f);
            Trace.Listeners.Add(new TextWriterTraceListener(f));

            var subgradientEvaluator = GradientCalculatorBuilder.CreateGradientEvaluatorCPU(settings);
            var placingAlgorithm = new FuzzyPartitionPlacingCentersAlgorithm(settings, subgradientEvaluator);
            var targetFunctionalValue = 0d;

            while (!placingAlgorithm.IsFinished)
            {
                Trace.WriteLine($"Starting iteration # {placingAlgorithm.PerformedIterationCount + 1}\n");
                Trace.Flush();

                placingAlgorithm.DoIteration();

                // Calculate fixed partition for current centers positions
                var centers = placingAlgorithm.GetCurrentCenters();
                var centersSettings = new CentersSettings { CenterDatas = centers };

                var fuzzyFixedPartitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(settings.SpaceSettings, centersSettings, settings.FuzzyPartitionFixedCentersSettings);
                var partition = fuzzyFixedPartitionEvaluator.BuildPartition();
                var muValueGetters = partition.CreateGridValueInterpolators(settings.SpaceSettings);

                // Calculate target and dual functional values
                var targetFunctionalCalculator = new TargetFunctionalCalculator(settings.SpaceSettings, centersSettings, settings.GaussLegendreIntegralOrder);
                targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

                Trace.WriteLine($"Target functional value = {targetFunctionalValue}\n");
                TraceCentersData(centers);
            }

            var resultCenters = placingAlgorithm.GetCurrentCenters();
            TraceCentersData(resultCenters);

            Trace.WriteLine($"Iterations count: {placingAlgorithm.PerformedIterationCount}");
            Trace.WriteLine($"Finished========================\n");
            Trace.Flush();
            Trace.Close();

            return new FinalPartitionData(resultCenters, targetFunctionalValue);
        }



        //[Test]
        //public void UnitSquare1Center()
        //{
        //    var settings = new PartitionSettings
        //    {
        //        IsCenterPlacingTask = true,
        //        SpaceSettings = new SpaceSettings
        //        {
        //            MinCorner = VectorUtils.CreateVector(0, 0),
        //            MaxCorner = VectorUtils.CreateVector(1, 1),
        //            GridSize = new List<int> { 50, 50 },
        //            DensityType = DensityType.Everywhere1,
        //            MetricsType = MetricsType.Euclidean,
        //            CustomDensityFunction = null,
        //            CustomDistanceFunction = null
        //        },
        //        CentersSettings = new CentersSettings
        //        {
        //            CenterDatas = new List<CenterData>
        //            {
        //                new CenterData
        //                {
        //                    A = 0,
        //                    W = 1,
        //                    Position = VectorUtils.CreateVector(0, 0),
        //                    IsFixed = false
        //                }
        //            }
        //        },
        //        FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
        //        {
        //            GradientEpsilon = 0.001,
        //            GradientStep = 1,
        //            MaxIterationsCount = 1000
        //        },
        //        FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
        //        {
        //            CentersDeltaEpsilon = 0.001,
        //        },
        //        RAlgorithmSettings = new RAlgorithmSettings
        //        {
        //            SpaceStretchFactor = 2,
        //            H0 = 0.7,
        //            MaxIterationsCount = 100
        //        },
        //        GaussLegendreIntegralOrder = 32
        //    };

        //    var result = ExecutePlacingAlgorithm(settings);
        //    Assert.AreEqual(0.38243711, result.TargetFunctionalValue, 0.001);
        //    Assert.AreEqual(0.5, result.Centers[0].Position[0], 0.01);
        //    Assert.AreEqual(0.5, result.Centers[0].Position[1], 0.01);
        //}

        //[Test]
        //public void UnitSquare2Centers()
        //{
        //    var settings = new PartitionSettings
        //    {
        //        IsCenterPlacingTask = true,
        //        SpaceSettings = new SpaceSettings
        //        {
        //            MinCorner = VectorUtils.CreateVector(0, 0),
        //            MaxCorner = VectorUtils.CreateVector(1, 1),
        //            GridSize = new List<int> { 50, 50 },
        //            DensityType = DensityType.Everywhere1,
        //            MetricsType = MetricsType.Euclidean,
        //            CustomDensityFunction = null,
        //            CustomDistanceFunction = null
        //        },
        //        CentersSettings = new CentersSettings
        //        {
        //            CenterDatas = new List<CenterData>
        //            {
        //                new CenterData
        //                {
        //                    A = 0,
        //                    W = 1,
        //                    Position = VectorUtils.CreateVector(0, 0),
        //                    IsFixed = false
        //                },
        //                new CenterData
        //                {
        //                    A = 0,
        //                    W = 1,
        //                    Position = VectorUtils.CreateVector(0, 0),
        //                    IsFixed = false
        //                },
        //            }
        //        },
        //        FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
        //        {
        //            GradientEpsilon = 0.001,
        //            GradientStep = 1,//0.1
        //            MaxIterationsCount = 1000
        //        },
        //        FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
        //        {
        //            CentersDeltaEpsilon = 0.001,
        //        },
        //        RAlgorithmSettings = new RAlgorithmSettings
        //        {
        //            SpaceStretchFactor = 2,
        //            H0 = 0.7,
        //            MaxIterationsCount = 100
        //        },
        //        GaussLegendreIntegralOrder = 32
        //    };

        //    var result = ExecutePlacingAlgorithm(settings);
        //    Assert.AreEqual(0.18644, result.TargetFunctionalValue, 0.001);

        //    //Assert.AreEqual(0.33, result.Centers[0].Position[0], 0.01);
        //    //Assert.AreEqual(0.5, result.Centers[0].Position[1], 0.01);

        //    //Assert.AreEqual(0.66, result.Centers[1].Position[0], 0.01);
        //    //Assert.AreEqual(0.5, result.Centers[1].Position[1], 0.01);
        //}

        //[Test]
        //public void UnitSquare3Centers()
        //{

        //}

        //[Test]
        //public void UnitSquare9Centers()
        //{
        //    var settings = new PartitionSettings
        //    {
        //        IsCenterPlacingTask = true,
        //        SpaceSettings = new SpaceSettings
        //        {
        //            MinCorner = VectorUtils.CreateVector(0, 0),
        //            MaxCorner = VectorUtils.CreateVector(1, 1),
        //            GridSize = new List<int> { 50, 50 },
        //            DensityType = DensityType.Everywhere1,
        //            MetricsType = MetricsType.Euclidean,
        //            CustomDensityFunction = null,
        //            CustomDistanceFunction = null
        //        },
        //        CentersSettings = new CentersSettings
        //        {
        //            CenterDatas = new List<CenterData>()
        //        },
        //        FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
        //        {
        //            GradientEpsilon = 0.001,
        //            GradientStep = 1,//0.1
        //            MaxIterationsCount = 1000
        //        },
        //        FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
        //        {
        //            CentersDeltaEpsilon = 0.001,
        //        },
        //        RAlgorithmSettings = new RAlgorithmSettings
        //        {
        //            SpaceStretchFactor = 2,
        //            H0 = 0.7,
        //            MaxIterationsCount = 100
        //        },
        //        GaussLegendreIntegralOrder = 32
        //    };
        //    for (var i = 0; i < 9; i++)
        //    {
        //        var data = new CenterData
        //        {
        //            A = 0,
        //            W = 1,
        //            Position = VectorUtils.CreateVector(0, 0),
        //            IsFixed = false
        //        };
        //        settings.CentersSettings.CenterDatas.Add(data);
        //    }

        //    var result = ExecutePlacingAlgorithm(settings);
        //    Assert.True(true);
        //    //Assert.AreEqual(0.18644, result.TargetFunctionalValue, 0.001);

        //    //Assert.AreEqual(0.33, result.Centers[0].Position[0], 0.01);
        //    //Assert.AreEqual(0.5, result.Centers[0].Position[1], 0.01);

        //    //Assert.AreEqual(0.66, result.Centers[1].Position[0], 0.01);
        //    //Assert.AreEqual(0.5, result.Centers[1].Position[1], 0.01);
        //}
    }
}