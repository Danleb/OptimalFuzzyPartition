using MathNet.Numerics.Integration;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate
{
    /// <summary>
    /// Calculation the value of the functional that is dual to the target functional.
    /// </summary>
    public class DualFunctionalCalculator
    {
        public SpaceSettings SpaceSettings { get; }
        public CentersSettings CentersSettings { get; }
        public int GaussLegendreIntegralOrder { get; }

        private readonly GridValueInterpolator _psiGridValueGetter;

        public DualFunctionalCalculator(SpaceSettings spaceSettings, CentersSettings centersSettings, int gaussLegendreIntegralOrder, GridValueInterpolator psiGridValueGetter)
        {
            SpaceSettings = spaceSettings;
            CentersSettings = centersSettings;
            GaussLegendreIntegralOrder = gaussLegendreIntegralOrder;
            _psiGridValueGetter = psiGridValueGetter;
        }

        public double CalculateFunctionalValue()
        {
            var integralValue = GaussLegendreRule.Integrate((x, y) =>
                {
                    var functionValue = 0d;
                    var densityValue = 1d;
                    var psi = _psiGridValueGetter.GetGridValueAtPoint(x, y);
                    var point = VectorUtils.CreateVector(x, y);

                    for (var centerIndex = 0; centerIndex < CentersSettings.CentersCount; centerIndex++)
                    {
                        var data = CentersSettings.CenterDatas[centerIndex];
                        var position = data.Position;
                        var distance = (point - position).L2Norm();
                        var a = data.A;
                        var w = data.W;
                        var value = (psi * psi) / ((distance / w + a) * densityValue);
                        functionValue += value;
                    }

                    return functionValue;
                },
                SpaceSettings.MinCorner[0],
                SpaceSettings.MaxCorner[0],
                SpaceSettings.MinCorner[1],
                SpaceSettings.MaxCorner[1],
                GaussLegendreIntegralOrder
            );

            var functionalValue = 0.25d * integralValue;

            return functionalValue;
        }
    }
}