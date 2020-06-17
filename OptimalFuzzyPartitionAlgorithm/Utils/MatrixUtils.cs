using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;

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

        public static void TraceMatrix(int columnsCount, int rowsCount, Func<int, int, double> m)
        {
            for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    var v = m(rowIndex, columnIndex);
                    Trace.Write($"{v:0.00} ");
                }

                Trace.Write(Environment.NewLine);
            }
        }
    }
}