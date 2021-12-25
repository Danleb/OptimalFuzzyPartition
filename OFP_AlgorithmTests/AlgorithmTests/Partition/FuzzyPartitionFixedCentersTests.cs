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
    public static class TestDataUtils
    {
        public static PartitionSettings GetDefaultSettings()
        {
            return new PartitionSettings
            {
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(0, 0),
                    MaxCorner = VectorUtils.CreateVector(1, 1),
                    GridSize = new List<int> { 32, 32 },
                    DensityType = DensityType.Everywhere1,
                    MetricsType = MetricsType.Euclidean,
                    CustomDensityFunction = null,
                    CustomDistanceFunction = null
                },
                CentersSettings = new CentersSettings(),
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.001,
                    GradientStep = 1,
                    MaxIterationsCount = 500
                },
                GaussLegendreIntegralOrder = 32,
                RAlgorithmSettings = null,
                IsCenterPlacingTask = false,
                FuzzyPartitionPlacingCentersSettings = null,
            };
        }

        public static CenterData MakeCenter(double x, double y, double a, double w)
        {
            return new CenterData
            {
                A = a,
                W = w,
                IsFixed = true,
                Position = VectorUtils.CreateVector(x, y)
            };
        }

        public static void AddCenter(this PartitionSettings partitionSettings, double x, double y, double a = 0.0, double w = 1.0)
        {
            partitionSettings.CentersSettings.CenterDatas.Add(MakeCenter(x, y, a, w));
        }

        public static void ExecuteTest(PartitionSettings settings, double expectedFunctionalValue, double epsilon = 0.001)
        {
            var partitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(settings.SpaceSettings, settings.CentersSettings, settings.FuzzyPartitionFixedCentersSettings);
            var partition = partitionEvaluator.BuildPartition(out var psiGrid);
            var muValueGetters = partition.CreateGridValueInterpolators(settings.SpaceSettings);

            var targetFunctionalCalculator = new TargetFunctionalCalculator(settings.SpaceSettings, settings.CentersSettings, settings.GaussLegendreIntegralOrder);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);

            var dualFunctionalCalculator = new DualFunctionalCalculator(settings.SpaceSettings, settings.CentersSettings, settings.GaussLegendreIntegralOrder, psiGrid.ToGridValueInterpolator(settings.SpaceSettings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();

            Assert.AreEqual(expectedFunctionalValue, targetFunctionalValue, epsilon);
            Assert.AreEqual(expectedFunctionalValue, dualFunctionalValue, epsilon);
            //Assert.AreEqual(partitionEvaluator.PerformedIterationsCount, 0);
        }
    }

    public class FuzzyPartitionFixedCentersTests
    {
        [Test]
        public void UnitSquare1Center()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.5, 0.5);
            // todo move to separate test
            Assert.AreEqual(1, settings.CentersSettings.CentersCount);
            Assert.AreEqual(1, settings.CentersSettings.FixedCentersCount);
            Assert.AreEqual(0, settings.CentersSettings.PlacingCentersCount);
            TestDataUtils.ExecuteTest(settings, 0.38243711);
        }

        [Test]
        public void UnitSquare2Centers()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.25, 0.5);
            settings.AddCenter(0.75, 0.5);
            TestDataUtils.ExecuteTest(settings, 0.188551295725909);
        }

        [Test]
        public void Centers2_WithA()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.25, 0.5, a: 0);
            settings.AddCenter(0.75, 0.5, a: 0.2);
            TestDataUtils.ExecuteTest(settings, 0.234878918790086);
        }

        [Test]
        public void Centers3()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.25, 0.5);
            settings.AddCenter(0.75, 0.5);
            settings.AddCenter(0.5, 0.75);
            TestDataUtils.ExecuteTest(settings, 0.124428343589458);
        }

        [Test]
        public void Centers7()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.2, 0.7);
            settings.AddCenter(0.5, 0.8);
            settings.AddCenter(0.8, 0.7);
            settings.AddCenter(0.8, 0.3);
            settings.AddCenter(0.5, 0.2);
            settings.AddCenter(0.2, 0.3);
            settings.AddCenter(0.5, 0.5);
            TestDataUtils.ExecuteTest(settings, 0.0500511474840841, 0.0001);
        }

        [Test]
        public void Centers7_WithA()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.2, 0.7, a: 0.2);
            settings.AddCenter(0.5, 0.8, a: 0.2);
            settings.AddCenter(0.8, 0.7, a: 0.1);
            settings.AddCenter(0.8, 0.3, a: 0.1);
            settings.AddCenter(0.5, 0.2, a: 0.1);
            settings.AddCenter(0.2, 0.3, a: 0.1);
            settings.AddCenter(0.5, 0.5, a: 0.0);
            TestDataUtils.ExecuteTest(settings, 0.0697955709486163);
        }

        [Test]
        public void Centers7_WithSymmetricA()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.AddCenter(0.2, 0.7, a: 0.2);
            settings.AddCenter(0.5, 0.8, a: 0.2);
            settings.AddCenter(0.8, 0.7, a: 0.2);
            settings.AddCenter(0.8, 0.3, a: 0.2);
            settings.AddCenter(0.5, 0.2, a: 0.2);
            settings.AddCenter(0.2, 0.3, a: 0.2);
            settings.AddCenter(0.5, 0.5, a: 0.0);
            TestDataUtils.ExecuteTest(settings, 0.0784969368835626, 0.0001);
        }

        [Test]
        public void Centers11_WithA()
        {
            var settings = TestDataUtils.GetDefaultSettings();
            settings.FuzzyPartitionFixedCentersSettings.PsiStartValue = 3;
            settings.AddCenter(0.2, 0.7, a: 0.2);
            settings.AddCenter(0.5, 0.8, a: 0.2);
            settings.AddCenter(0.8, 0.7, a: 0.2);
            settings.AddCenter(0.8, 0.3, a: 0.2);
            settings.AddCenter(0.5, 0.2, a: 0.2);
            settings.AddCenter(0.2, 0.3, a: 0.2);
            settings.AddCenter(0.5, 0.5, a: 0.2);
            settings.AddCenter(0.1, 0.5, a: 0.2);
            settings.AddCenter(0.9, 0.5, a: 0.2);
            settings.AddCenter(0.1, 0.1, a: 0.2);
            settings.AddCenter(0.9, 0.9, a: 0.2);
            TestDataUtils.ExecuteTest(settings, 0.05684687, 0.0001);
        }

        [Test]
        public void Centers41()
        {
            var settings = TestDataUtils.GetDefaultSettings();

            for (var i = 0; i <= 4; i++)
            {
                for (var j = 0; j <= 4; j++)
                {
                    var x = 0.1 + i * 0.2;
                    var y = 0.1 + j * 0.2;
                    settings.AddCenter(x, y);
                }
            }

            for (var i = 0; i <= 3; i++)
            {
                for (var j = 0; j <= 3; j++)
                {
                    var x = 0.2 + i * 0.2;
                    var y = 0.2 + j * 0.2;
                    settings.AddCenter(x, y);
                }
            }

            settings.FuzzyPartitionFixedCentersSettings.PsiStartValue = 5;
            TestDataUtils.ExecuteTest(settings, 0.00830753741833222, 0.0001);
        }
    }
}
