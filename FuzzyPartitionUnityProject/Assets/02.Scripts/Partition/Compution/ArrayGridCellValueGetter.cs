using OptimalFuzzyPartitionAlgorithm.Algorithm;

namespace FuzzyPartitionComputing
{
    public class ArrayGridCellValueGetter : IGridCellValueGetter
    {
        private readonly float[] _grid;
        private readonly int _gridSizeX;
        private readonly int _layerBaseIndex;

        public ArrayGridCellValueGetter(float[] grid2d, int gridSizeX)
        {
            _grid = grid2d;
            _gridSizeX = gridSizeX;
        }

        public ArrayGridCellValueGetter(float[] grid3d, int gridSizeX, int gridSizeY, int layerIndex)
        {
            _grid = grid3d;
            _gridSizeX = gridSizeX;
            _layerBaseIndex = gridSizeX * gridSizeY * layerIndex;
        }

        public double GetValue(int rowIndex, int columnIndex)
        {
            var gridValue = _grid[_layerBaseIndex + rowIndex * _gridSizeX + columnIndex];
            return gridValue;
        }
    }
}
