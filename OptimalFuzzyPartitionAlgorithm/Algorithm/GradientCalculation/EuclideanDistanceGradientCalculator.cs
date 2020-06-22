using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation
{
    public class EuclideanDistanceGradientCalculator : IDistanceGradientCalculator
    {
        public double GetGradientDimensionValue(Vector<double> point, int dimensionIndex)
        {
            return 0;
        }

        public Vector<double> GetGradientVector(Vector<double> point)
        {
            throw new System.NotImplementedException();
        }
    }
}