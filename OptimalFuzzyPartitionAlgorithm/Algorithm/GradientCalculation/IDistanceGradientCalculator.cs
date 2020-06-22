using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation
{
    public interface IDistanceGradientCalculator
    {
        double GetGradientDimensionValue(Vector<double> point, int dimensionIndex);
        Vector<double> GetGradientVector(Vector<double> point);
    }
}