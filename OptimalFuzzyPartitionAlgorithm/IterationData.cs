using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm
{
    public class IterationData
    {
        /// <summary>
        /// Номер итерации, на которой были получены ти результаты.
        /// </summary>
        public int IterationNumber;

        public List<Vector<double>> Centers;
    }
}