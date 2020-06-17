using OptimalFuzzyPartitionAlgorithm.Algorithm;
using Unity.Collections;

namespace FuzzyPartitionComputing
{
    public class MuValueTextureCalculator : IMuValueGetter
    {
        private readonly NativeArray<float> _muGrid;
        private readonly int _gridSizeX;

        public MuValueTextureCalculator(NativeArray<float> muGrid, int gridSizeX)
        {
            _muGrid = muGrid;
            _gridSizeX = gridSizeX;
        }

        public double GetMuValue(int xIndex, int yIndex)
        {
            float muValue = _muGrid[yIndex * _gridSizeX + xIndex];
            return muValue;
        }
    }
}