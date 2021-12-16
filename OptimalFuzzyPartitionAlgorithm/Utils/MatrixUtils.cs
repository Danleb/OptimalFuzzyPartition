using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
using System.Text;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public static class MatrixUtils
    {
        public static void WriteMatrix(Matrix<double> matrix, Action<string> writeLine, int decimals = 2)
        {
            var sb = new StringBuilder();
            var format = "0." + Enumerable.Range(0, decimals).Select(v => "0").Aggregate(string.Concat);

            foreach (var (i, vector) in matrix.EnumerateRowsIndexed())
            {
                sb.Clear();

                foreach (var d in vector)
                {
                    sb.Append(d.ToString(format) + " ");
                }

                writeLine(sb.ToString());
            }
            writeLine(Environment.NewLine);
        }

        public static void WriteMatrix(int columnsCount, int rowsCount, Func<int, int, double> m, Action<string> writeLine, int decimals = 2)
        {
            var sb = new StringBuilder();
            var format = "0." + Enumerable.Range(0, decimals).Select(v => "0").Aggregate((a, b) => a + b);

            for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                sb.Clear();

                for (var columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    var v = m(rowIndex, columnIndex);
                    sb.Append(v.ToString(format) + " ");
                }

                writeLine(sb.ToString());
            }
        }

        public static IGridCellValueGetter ToGridCellValueGetter(this Matrix<double> matrix)
        {
            return new MatrixGridValueGetter(matrix);
        }

        public static GridValueInterpolator ToGridValueInterpolator(this Matrix<double> matrix, SpaceSettings spaceSettings)
        {
            return new GridValueInterpolator(spaceSettings, new MatrixGridValueGetter(matrix));
        }

        public static List<GridValueInterpolator> CreateGridValueInterpolators(this List<Matrix<double>> partition, SpaceSettings spaceSettings)
        {
            return partition.Select(v => new GridValueInterpolator(spaceSettings, new MatrixGridValueGetter(v))).ToList();
        }
    }
}