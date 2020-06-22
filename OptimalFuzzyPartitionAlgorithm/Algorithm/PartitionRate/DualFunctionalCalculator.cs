using MathNet.Numerics.Integration;
using OptimalFuzzyPartitionAlgorithm.Utils;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate
{
    /// <summary>
    /// Calculation the value of the functional that is dual to the target functional.
    /// </summary>
    public class DualFunctionalCalculator
    {
        private readonly PartitionSettings _settings;
        private readonly GridValueInterpolator _psiGridValueGetter;

        public DualFunctionalCalculator(PartitionSettings partitionSettings, GridValueInterpolator psiGridValueGetter)
        {
            _settings = partitionSettings;
            _psiGridValueGetter = psiGridValueGetter;
        }

        public double CalculateFunctionalValue()
        {
            var integralValue = GaussLegendreRule.Integrate((x, y) =>
                {
                    var psi = -_psiGridValueGetter.GetGridValueAtPoint(x, y);
                    var densityValue = 1d;
                    var functionValue = 0d;
                    var point = VectorUtils.CreateVector(x, y);

                    for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
                    {
                        var data = _settings.CentersSettings.CenterDatas[centerIndex];
                        var position = data.Position;
                        var distance = (point - position).L2Norm();
                        var a = data.A;
                        var w = data.W;
                        var value = (psi * psi) / ((distance / w + a) * densityValue);
                        functionValue += value;
                    }

                    return functionValue;
                },
                _settings.SpaceSettings.MinCorner[0],
                _settings.SpaceSettings.MaxCorner[0],
                _settings.SpaceSettings.MinCorner[1],
                _settings.SpaceSettings.MaxCorner[1],
                _settings.FuzzyPartitionPlacingCentersSettings.GaussLegendreIntegralOrder
            );

            var functionalValue = 0.25d * integralValue;

            return functionalValue;
        }

    }
}