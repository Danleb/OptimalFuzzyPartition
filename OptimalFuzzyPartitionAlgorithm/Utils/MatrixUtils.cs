using System;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public class MatrixUtils
    {
        public static void TraceMatrix(Matrix<double> matrix)
        {
            foreach (var (i, vector) in matrix.EnumerateRowsIndexed())
            {
                foreach (var d in vector)
                {
                    Trace.Write(d.ToString("0.00") + " ");
                }

                Trace.Write(Environment.NewLine);
            }
        }
    }
}