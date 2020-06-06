using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Высчитывает значение градиента.
    /// </summary>
    public class GradientCalculator
    {
        private PartitionSettings Settings { get; }

        public GradientCalculator(PartitionSettings settings)
        {
            Settings = settings;
            
        }

        /// <summary>
        /// Посчитать значение градиента функционала G по центрам.
        /// Градиент - это вектор размера количества центров.
        /// </summary>
        /// <param name="iterationData"></param>
        /// <returns></returns>
        private Vector<double> CalculateGradient(IterationData iterationData)
        {
            //var gradient = Matrix<double>.Build.Sparse(Settings.DimensionsCount, Settings.CentersCount);

            var gradient = Vector<double>.Build.Sparse(Settings.CentersCount);

            for (var centerIndex = 0; centerIndex < Settings.CentersCount; centerIndex++)
            {
                var index = centerIndex;
                var integralCalculator = new IntegralCalculator(Settings.MinCorner, Settings.MaxCorner, Settings.GridSize, point => GetUnderIntegralFunctionValue(point, index));
                var value = integralCalculator.CalculateIntegral();
                gradient[centerIndex] = value;
            }

            return gradient;
        }

        /// <summary>
        /// Посчитать i компоненту градиента. Это вектор, он зависит от centerIndex.
        /// </summary>
        /// <param name="centerIndex"></param>
        /// <returns></returns>
        private double GetGradientComponentValueForCenter(int centerIndex)
        {
            for (var i = 0; i < Settings.DimensionsCount; i++)
            {
                
            }


            return 0;
        }

        /// <summary>
        /// Найти вектор.
        /// </summary>
        /// <returns></returns>
        private Vector<double> GetGradientC()
        {


            return null;
        }

        private double GetUnderIntegralFunctionValue(Vector<double> point, int centerIndex)
        {
            //var diff = point - ;

            //var value = diff / ;

            return 0;
        }
    }
}