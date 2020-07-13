using OptimalFuzzyPartitionAlgorithm.Algorithm;
using Unity.Collections;

namespace FuzzyPartitionComputing
{
    public class GridValueTextureCalculator : IGridCellValueGetter
    {
        private readonly NativeArray<float> _gridTexture;
        private readonly int _gridSizeX;

        public GridValueTextureCalculator(NativeArray<float> gridTexture, int gridSizeX)
        {
            _gridTexture = gridTexture;
            _gridSizeX = gridSizeX;
        }

        public double GetValue(int rowIndex, int columnIndex)
        {
            float gridValue = _gridTexture[rowIndex * _gridSizeX + columnIndex];
            return gridValue;
        }
    }
}