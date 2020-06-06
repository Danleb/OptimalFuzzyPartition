using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Вычисляет значение функционала G(teta)=1/4 I(1/(ro(x)*c(x,teta)+a)dx.
    /// </summary>
    public class FunctionalGCalculator
    {
        public PartitionSettings Settings { get; }
        public IterationData IterationData { get; }

        public FunctionalGCalculator(PartitionSettings partitionSettings, IterationData iterationData)
        {
            Settings = partitionSettings;
            IterationData = iterationData;
        }

        public double CalculateFunctionalValue()
        {
            var integralCalculator = new IntegralCalculator(Settings.MinCorner, Settings.MaxCorner, Settings.GridSize, GetUnderIntegralValue);

            var integralValue = integralCalculator.CalculateIntegral();
            var value = 0.25d * integralValue; //0.5??/

            return value;
        }

        private double GetUnderIntegralValue(Vector<double> point)
        {
            var underIntegralValue = 0d;

            for (var i = 0; i < Settings.CentersCount; i++)
            {
                var center = IterationData.Centers[i];
                var w = Settings.MultiplicativeCoefficients[i];
                var a = Settings.AdditiveCoefficients[i];

                var density = Settings.Density(point);
                var distance = Settings.Distance(point, center) / w + a;

                var value = 1d / (density * distance);

                underIntegralValue += value;
            }

            return underIntegralValue;
        }
    }
}