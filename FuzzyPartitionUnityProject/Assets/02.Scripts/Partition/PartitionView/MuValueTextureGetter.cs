using OptimalFuzzyPartitionAlgorithm.Algorithm;
using Unity.Collections;

namespace FuzzyPartitionComputing
{
    public class MuValueTextureCalculator : IGridCellValueGetter
    {
        private readonly NativeArray<float> _muGrid;
        private readonly int _gridSizeX;

        public MuValueTextureCalculator(NativeArray<float> muGrid, int gridSizeX)
        {
            _muGrid = muGrid;
            _gridSizeX = gridSizeX;
        }

        public double GetValue(int rowIndex, int columnIndex)
        {
            float muValue = _muGrid[rowIndex * _gridSizeX + columnIndex];
            return muValue;
        }
    }
}