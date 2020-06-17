using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class MuGridValueGetter : IMuValueGetter
    {
        private readonly Matrix<double> _muGrid;

        public MuGridValueGetter(Matrix<double> muGrid)
        {
            _muGrid = muGrid;
        }

        public double GetMuValue(int rowIndex, int columnIndex)
        {
            return _muGrid[rowIndex, columnIndex];
        }
    }
}