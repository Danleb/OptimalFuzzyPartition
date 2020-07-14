using NUnit.Framework;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate;
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
        public void MuGridsFixedPartition8x8Test()
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
                    GridSize = new List<int> { 80, 80 },
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
                            Position = VectorUtils.CreateVector(1, 1),
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            //Position = VectorUtils.CreateVector(2, 4),
                            Position = VectorUtils.CreateVector(9, 1),
                            IsFixed = true
                        },
                    }
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.001,
                    GradientStep = 10,
                    MaxIterationsCount = 400
                },
                FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
                {
                    GaussLegendreIntegralOrder = 4
                }
            };

            var calculator = new FuzzyPartitionFixedCentersAlgorithm(settings);
            var partition = calculator.BuildPartition(out var psiGrid);
            Trace.WriteLine($"PerformedIterationsCount = {calculator.PerformedIterationsCount}");
            var muValueGetters = partition.Select(v => new GridValueInterpolator(settings.SpaceSettings, new MatrixGridValueGetter(v))).ToList();

            var targetFunctionalCalculator = new TargetFunctionalCalculator(settings);
            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muValueGetters);
            Trace.WriteLine($"Target functional value = {targetFunctionalValue}\n");

            var dualFunctionalCalculator = new DualFunctionalCalculator(settings, psiGrid.ToGridValueInterpolator(settings));
            var dualFunctionalValue = dualFunctionalCalculator.CalculateFunctionalValue();
            Trace.WriteLine($"Dual functional value = {dualFunctionalValue}\n");

            Trace.WriteLine("Center #1 Mu matrix:");
            MatrixUtils.WriteMatrix(partition[0], WriteLine, 3);

            Trace.WriteLine("Center #2 Mu matrix:");
            MatrixUtils.WriteMatrix(partition[1], WriteLine, 3);

            var sum = partition.Aggregate((a, b) => a + b);
            Trace.WriteLine("Sum mu matrix:");
            MatrixUtils.WriteMatrix(sum, WriteLine, 3);
            Trace.Flush();

            Assert.AreEqual(targetFunctionalValue, dualFunctionalValue, 1d);

            var q = 5;
        }

        private void WriteLine(string s) => Trace.WriteLine(s);
    }
}