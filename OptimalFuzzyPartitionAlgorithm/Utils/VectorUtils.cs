using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public static class VectorUtils
    {
        public static Vector<double> CreateVector(params double[] coordinates) => Vector<double>.Build.DenseOfArray(coordinates);

        public static IReadOnlyList<int> CreateVectorInt(params int[] coordinates) => new List<int>(coordinates);
    }
}