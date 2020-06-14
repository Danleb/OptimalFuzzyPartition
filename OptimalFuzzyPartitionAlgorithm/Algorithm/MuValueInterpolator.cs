using MathNet.Numerics.LinearAlgebra;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class MuValueInterpolator
    {
        private readonly SpaceSettings _spaceSettings;
        private readonly Matrix<double> _muGrid;

        public MuValueInterpolator(SpaceSettings spaceSettings, Matrix<double> muGrid)
        {
            _spaceSettings = spaceSettings;
            _muGrid = muGrid;
        }

        public double GetMuValueAtPoint(double x, double y)
        {
            var xGlobalRatio = (x - _spaceSettings.MinCorner[0]) /
                               (_spaceSettings.MaxCorner[0] - _spaceSettings.MinCorner[0]);

            var xIndexFractional = (_spaceSettings.GridSize[0] - 1) * xGlobalRatio;

            var yGlobalRatio = (y - _spaceSettings.MinCorner[1]) /
                               (_spaceSettings.MaxCorner[1] - _spaceSettings.MinCorner[1]);

            var yIndexFractional = (_spaceSettings.GridSize[1] - 1) * yGlobalRatio;

            var x1 = (int)xIndexFractional;
            var x2 = x1 + 1;
            var y1 = (int)yIndexFractional;
            var y2 = y1 + 1;

            var localXRatio = xIndexFractional - x1;
            var localYRatio = yIndexFractional - y1;

            //bilinear interpolation
            var pX1Y1 = _muGrid[x1, y1];
            var pX1Y2 = _muGrid[x1, y2];
            var pX2Y1 = _muGrid[x2, y1];
            var pX2Y2 = _muGrid[x2, y2];

            var l1 = pX1Y1 * (1d - localXRatio) + pX2Y1 * localXRatio;
            var l2 = pX1Y2 * (1d - localXRatio) + pX2Y2 * localXRatio;

            var muValue = l1 * (1d - localYRatio) + l2 * localYRatio;

            return muValue;
        }
    }
}