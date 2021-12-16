using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation
{
    /// <summary>
    /// Calculates value of gradient in point.
    /// </summary>
    public interface IDistanceGradientCalculator
    {
        double GetGradientDimensionValue(Vector<double> point, int dimensionIndex);
        Vector<double> GetGradientVector(Vector<double> point);
    }
}