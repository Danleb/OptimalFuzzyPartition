using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Calculates the value of the target functional that we minimize.
    /// </summary>
    public class TargetFunctionalCalculator
    {
        public PartitionSettings Settings { get; }

        public TargetFunctionalCalculator(PartitionSettings partitionSettings)
        {
            Settings = partitionSettings;
        }

        public double CalculateFunctionalValue(List<Matrix<double>> muGrids)
        {
            var muValueCalculators = muGrids.Select(v => new MuValueInterpolator(Settings.SpaceSettings, v)).ToList();

            var value = GaussLegendreRule.Integrate((x, y) =>
                {
                    var functionValue = 0d;
                    var densityValue = 1d;

                    for (var centerIndex = 0; centerIndex < Settings.CentersSettings.CentersCount; centerIndex++)
                    {
                        var centerData = Settings.CentersSettings.CenterDatas[centerIndex];
                        var center = centerData.Position;
                        var w = centerData.W;
                        var a = centerData.A;
                        var point = VectorUtils.CreateVector(x, y);
                        var distanceValue = (point - center).L2Norm();
                        var muValue = muValueCalculators[centerIndex].GetMuValueAtPoint(x, y);
                        functionValue += Math.Pow(muValue, 2) * (distanceValue / w + a) * densityValue;
                    }

                    return functionValue;
                },
                Settings.SpaceSettings.MinCorner[0],
                Settings.SpaceSettings.MaxCorner[0],
                Settings.SpaceSettings.MinCorner[1],
                Settings.SpaceSettings.MaxCorner[1],
                Settings.FuzzyPartitionPlacingCentersSettings.GaussLegendreIntegralOrder
                );

            return value;
        }
    }
}