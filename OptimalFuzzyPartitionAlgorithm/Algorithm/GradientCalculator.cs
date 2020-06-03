using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Высчитывает значение градиента.
    /// </summary>
    public class GradientCalculator
    {
        private PartitionSettings Settings { get; }



        private readonly Vector<double> GridSize;

        public GradientCalculator(PartitionSettings settings)
        {
            Settings = settings;



            GridSize = Settings.GridSize.Map(v => (double)v);
        }

        public Vector CalculateGradient(IterationData iterationData)
        {



            return null;
        }
    }
}