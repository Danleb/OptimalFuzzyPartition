using MathNet.Numerics.LinearAlgebra;
using System;

namespace OptimalFuzzyPartitionAlgorithm.Settings
{
    [Serializable]
    public class CenterData
    {
        /// <summary>
        /// Center position in space.
        /// </summary>
        public Vector<double> Position;

        /// <summary>
        /// Additive coefficient a.
        /// </summary>
        public double A = 0;

        /// <summary>
        /// Multiplicative coefficient w.
        /// </summary>
        public double W = 1;

        /// <summary>
        /// Is center position fixed or can be moved.
        /// </summary>
        public bool IsFixed = false;
    }
}