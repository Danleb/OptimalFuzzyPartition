using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class MatrixGridValueGetter : IGridCellValueGetter
    {
        private readonly Matrix<double> _muGrid;

        public MatrixGridValueGetter(Matrix<double> muGrid)
        {
            _muGrid = muGrid;
        }

        public double GetValue(int rowIndex, int columnIndex)
        {
            return _muGrid[rowIndex, columnIndex];
        }
    }
}