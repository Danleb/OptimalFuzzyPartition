using NUnit.Framework;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithmTests.AlgorithmTests
{
    public class FuzzyPartitionFixedCentersTests
    {
        [Test]
        public void UnitSquare1Center()
        {
            var spaceSettings = new SpaceSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(1, 1),
                GridSize = new List<int> { 50, 50 },
                DensityType = DensityType.Everywhere1,
                MetricsType = MetricsType.Euclidean,
                CustomDensityFunction = null,
                CustomDistanceFunction = null
            };
            var centersSettings = new CentersSettings
            {
                CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.5, 0.5),
                            IsFixed = true
                        }
                    },
            };
            var partitionSettings = new FuzzyPartitionFixedCentersSettings
            {
                GradientEpsilon = 0.001,
                GradientStep = 1,
                MaxIterationsCount = 1000
            };
            var gaussLegendreIntegralOrder = 32;

            Assert.AreEqual(1, centersSettings.CentersCount);
            Assert.AreEqual(1, centersSettings.FixedCentersCount);
            Assert.AreEqual(0, centersSettings.PlacingCentersCount);

            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(spaceSettings, centersSettings, partitionSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(spaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(spaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            Assert.AreEqual(0.38243711, targetFunctionalValue, 0.001);
            Assert.AreEqual(0.38243711, dualFunctionalValue, 0.001);
            Assert.AreEqual(partitionEvaluator.PerformedIterationsCount, 0);
        }

        [Test]
        public void UnitSquare2Centers()
        {
            var spaceSettings = new SpaceSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(1, 1),
                GridSize = new List<int> { 50, 50 },
                DensityType = DensityType.Everywhere1,
                MetricsType = MetricsType.Euclidean,
                CustomDensityFunction = null,
                CustomDistanceFunction = null
            };
            var centersSettings = new CentersSettings
            {
                CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.25, 0.5),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.75, 0.5),
                            IsFixed = true
                        }
                    },
            };
            var partitionSettings = new FuzzyPartitionFixedCentersSettings
            {
                GradientEpsilon = 0.001,
                GradientStep = 1,
                MaxIterationsCount = 1000
            };
            var gaussLegendreIntegralOrder = 32;

            Assert.AreEqual(2, centersSettings.CentersCount);
            Assert.AreEqual(2, centersSettings.FixedCentersCount);
            Assert.AreEqual(0, centersSettings.PlacingCentersCount);

            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(spaceSettings, centersSettings, partitionSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(spaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(spaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            Assert.AreEqual(0.188551295725909, targetFunctionalValue, 0.0001);
            Assert.AreEqual(0.188551295725909, dualFunctionalValue, 0.001);
            Assert.LessOrEqual(partitionEvaluator.PerformedIterationsCount, 1000);
        }

        [Test]
        public void UnitSquare2CentersWithA()
        {
            var spaceSettings = new SpaceSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(1, 1),
                GridSize = new List<int> { 50, 50 },
                DensityType = DensityType.Everywhere1,
                MetricsType = MetricsType.Euclidean,
                CustomDensityFunction = null,
                CustomDistanceFunction = null
            };
            var centersSettings = new CentersSettings
            {
                CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.25, 0.5),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0.2,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.75, 0.5),
                            IsFixed = true
                        }
                    },
            };
            var partitionSettings = new FuzzyPartitionFixedCentersSettings
            {
                GradientEpsilon = 0.001,
                GradientStep = 1,
                MaxIterationsCount = 1000
            };
            var gaussLegendreIntegralOrder = 32;

            Assert.AreEqual(2, centersSettings.CentersCount);
            Assert.AreEqual(2, centersSettings.FixedCentersCount);
            Assert.AreEqual(0, centersSettings.PlacingCentersCount);

            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(spaceSettings, centersSettings, partitionSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(spaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(spaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            Assert.AreEqual(0.234878918790086, targetFunctionalValue, 0.0001);
            Assert.AreEqual(0.234878918790086, dualFunctionalValue, 0.001);
            Assert.LessOrEqual(partitionEvaluator.PerformedIterationsCount, 1000);
        }

        [Test]
        public void UnitSquare3Centers()
        {
            var spaceSettings = new SpaceSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(1, 1),
                GridSize = new List<int> { 50, 50 },
                DensityType = DensityType.Everywhere1,
                MetricsType = MetricsType.Euclidean,
                CustomDensityFunction = null,
                CustomDistanceFunction = null
            };
            var centersSettings = new CentersSettings
            {
                CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.25, 0.5),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.75, 0.5),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.5, 0.75),
                            IsFixed = true
                        }
                    },
            };
            var partitionSettings = new FuzzyPartitionFixedCentersSettings
            {
                GradientEpsilon = 0.001,
                GradientStep = 1,
                MaxIterationsCount = 1000
            };
            var gaussLegendreIntegralOrder = 32;

            Assert.AreEqual(3, centersSettings.CentersCount);
            Assert.AreEqual(3, centersSettings.FixedCentersCount);
            Assert.AreEqual(0, centersSettings.PlacingCentersCount);

            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(spaceSettings, centersSettings, partitionSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(spaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(spaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            Assert.AreEqual(0.124428343589458, targetFunctionalValue, 0.001);
            Assert.AreEqual(0.124428343589458, dualFunctionalValue, 0.001);
            Assert.LessOrEqual(partitionEvaluator.PerformedIterationsCount, 1000);
        }

        [Test]
        public void UnitSquare7Centers()
        {
            var spaceSettings = new SpaceSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(1, 1),
                GridSize = new List<int> { 50, 50 },
                DensityType = DensityType.Everywhere1,
                MetricsType = MetricsType.Euclidean,
                CustomDensityFunction = null,
                CustomDistanceFunction = null
            };
            var centersSettings = new CentersSettings
            {
                CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.2, 0.7),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.5, 0.8),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.8, 0.7),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.8, 0.3),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.5, 0.2),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.2, 0.3),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.5, 0.5),
                            IsFixed = true
                        },
                    },
            };
            var partitionSettings = new FuzzyPartitionFixedCentersSettings
            {
                GradientEpsilon = 0.0001,
                GradientStep = 1,
                MaxIterationsCount = 1000
            };
            var gaussLegendreIntegralOrder = 32;

            Assert.AreEqual(7, centersSettings.CentersCount);
            Assert.AreEqual(7, centersSettings.FixedCentersCount);
            Assert.AreEqual(0, centersSettings.PlacingCentersCount);

            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(spaceSettings, centersSettings, partitionSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(spaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(spaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            Assert.AreEqual(0.0500511474840841, targetFunctionalValue, 0.001);
            Assert.AreEqual(0.0500511474840841, dualFunctionalValue, 0.001);
            Assert.LessOrEqual(partitionEvaluator.PerformedIterationsCount, 1000);
        }

        [Test]
        public void UnitSquare17Centers_AtCenter()
        {
            var spaceSettings = new SpaceSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(1, 1),
                GridSize = new List<int> { 16, 16 },
                DensityType = DensityType.Everywhere1,
                MetricsType = MetricsType.Euclidean,
                CustomDensityFunction = null,
                CustomDistanceFunction = null
            };
            var centersSettings = new CentersSettings
            {
                CenterDatas = new List<CenterData>()
            };
            var partitionSettings = new FuzzyPartitionFixedCentersSettings
            {
                GradientEpsilon = 0.0001,
                GradientStep = 1,
                MaxIterationsCount = 2000,
                PsiStartValue = -0.05//-0.1
            };
            var gaussLegendreIntegralOrder = 16;

            for (var i = 0; i < 17; i++)
            {
                centersSettings.CenterDatas.Add(new CenterData
                {
                    A = 0,
                    IsFixed = true,
                    W = 1.0,
                    Position = VectorUtils.CreateVector(0.5, 0.5)
                });
            }

            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(spaceSettings, centersSettings, partitionSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(spaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(spaceSettings, centersSettings, gaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(spaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            for (var i = 0; i < 17; i++)
            {
                var value = muValueGetters[i].GetGridValueAtPoint(0.5, 0.5);
                Assert.GreaterOrEqual(value, 0.01);
            }

            Assert.LessOrEqual(partitionEvaluator.PerformedIterationsCount, 500);
            Assert.AreEqual(0.0500511474840841, targetFunctionalValue, 0.001);
            Assert.AreEqual(0.0500511474840841, dualFunctionalValue, 0.001);
            Assert.LessOrEqual(partitionEvaluator.PerformedIterationsCount, 1000);
        }
    }
}