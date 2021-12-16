using MathNet.Numerics.Integration;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Calculates the value of the target functional that we minimize.
    /// </summary>
    public class TargetFunctionalCalculator
    {
        public SpaceSettings SpaceSettings { get; }
        public CentersSettings CentersSettings { get; }
        public int GaussLegendreIntegralOrder { get; }

        public TargetFunctionalCalculator(SpaceSettings spaceSettings, CentersSettings centersSettings, int gaussLegendreIntegralOrder)
        {
            SpaceSettings = spaceSettings;
            CentersSettings = centersSettings;
            GaussLegendreIntegralOrder = gaussLegendreIntegralOrder;
        }

        public double CalculateFunctionalValue(List<GridValueInterpolator> muValueInterpolators)
        {
            var value = GaussLegendreRule.Integrate((x, y) =>
                {
                    var functionValue = 0d;
                    var densityValue = 1d;
                    var point = VectorUtils.CreateVector(x, y);

                    //var minDist = double.MaxValue;

                    for (var centerIndex = 0; centerIndex < CentersSettings.CentersCount; centerIndex++)
                    {
                        var centerData = CentersSettings.CenterDatas[centerIndex];
                        var center = centerData.Position;
                        var w = centerData.W;
                        var a = centerData.A;
                        var distanceValue = (point - center).L2Norm();
                        var muValue = muValueInterpolators[centerIndex].GetGridValueAtPoint(x, y);
                        //var muValue = 1;
                        var v = Math.Pow(muValue, 2) * (distanceValue / w + a) * densityValue;

                        //if (distanceValue < minDist)
                        //{
                        //    minDist = distanceValue;
                        //    functionValue = v;
                        //}
                        functionValue += v;
                    }

                    return functionValue;
                },
                SpaceSettings.MinCorner[0],
                SpaceSettings.MaxCorner[0],
                SpaceSettings.MinCorner[1],
                SpaceSettings.MaxCorner[1],
                GaussLegendreIntegralOrder
                );

            return value;
        }
    }
}