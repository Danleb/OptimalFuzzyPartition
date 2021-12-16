using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using Matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using Vector = MathNet.Numerics.LinearAlgebra.Vector<double>;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Calculates fuzzy partition with fixed centers on CPU.
    /// </summary>
    public class FuzzyPartitionFixedCentersAlgorithm
    {
        public int PerformedIterationsCount { get; private set; }

        private SpaceSettings SpaceSettings { get; }
        private CentersSettings CentersSettings { get; }
        private FuzzyPartitionFixedCentersSettings PartitionSettings { get; }
        private int WidthX => SpaceSettings.GridSize[0];
        private int WidthY => SpaceSettings.GridSize[1];

        private double _maxGradientValue;
        private Matrix _psiGrid;
        private List<Matrix> _muGrids;

        public FuzzyPartitionFixedCentersAlgorithm(SpaceSettings spaceSettings, CentersSettings centersSettings, FuzzyPartitionFixedCentersSettings partitionSettings)
        {
            SpaceSettings = spaceSettings;
            CentersSettings = centersSettings;
            PartitionSettings = partitionSettings;
        }

        /// <summary>
        /// Returns list of Mu grids for each center.
        /// </summary>
        /// <param name="psiGrid">Result values of Psi grid.</param>
        /// <returns></returns>
        public List<Matrix> BuildPartition(out Matrix psiGrid)
        {
            Init();
            psiGrid = _psiGrid;

            // in case of one center the solution is trivial
            if (CentersSettings.CentersCount == 1)
            {
                SetSingleCenterPsi();
                return _muGrids;
            }

            while (true)
            {
                UpdateMuValues();
                UpdatePsiValues();

                PerformedIterationsCount++;

                if (IsStopConditionSatisfied())
                    break;
            }

            return _muGrids;
        }

        public List<Matrix> BuildPartition()
        {
            return BuildPartition(out _);
        }

        private void Init()
        {
            PerformedIterationsCount = 0;

            _muGrids = new List<Matrix>();
            var value = 1d / CentersSettings.CentersCount;

            for (var centerIndex = 0; centerIndex < CentersSettings.CentersCount; centerIndex++)
            {
                var m = Matrix.Build.Dense(WidthX, WidthY);

                for (var i = 0; i < WidthX; i++)
                {
                    for (var j = 0; j < WidthY; j++)
                    {
                        m[i, j] = value;
                    }
                }

                _muGrids.Add(m);
            }

            _psiGrid = Matrix.Build.Dense(WidthY, WidthX);

            for (var xIndex = 0; xIndex < WidthX; xIndex++)
            {
                for (var yIndex = 0; yIndex < WidthY; yIndex++)
                {
                    _psiGrid[yIndex, xIndex] = PartitionSettings.PsiStartValue;
                }
            }
        }

        private void UpdateMuValues()
        {
            for (var centerIndex = 0; centerIndex < CentersSettings.CentersCount; centerIndex++)
            {
                for (var xIndex = 0; xIndex < WidthX; xIndex++)
                {
                    for (var yIndex = 0; yIndex < WidthY; yIndex++)
                    {
                        var densityValue = 1d;
                        var m = 2d;
                        var data = CentersSettings.CenterDatas[centerIndex];
                        var a = data.A;
                        var w = data.W;
                        var centerPosition = data.Position;
                        var point = GetPoint(xIndex, yIndex);
                        var distance = (point - centerPosition).L2Norm();
                        var psiValue = _psiGrid[yIndex, xIndex];
                        var oldMuValue = _muGrids[centerIndex][yIndex, xIndex];
                        var newMuValue = -psiValue / (m * densityValue * (distance / w + a));

                        if (double.IsNaN(newMuValue) || newMuValue <= 0 || newMuValue >= 1)
                        {
                            var muGradient = psiValue + m * oldMuValue * (distance / w + a) * densityValue;
                            newMuValue = 0.5d * (1d - Math.Sign(muGradient));
                        }

                        _muGrids[centerIndex][yIndex, xIndex] = newMuValue;
                    }
                }
            }
        }

        private void UpdatePsiValues()
        {
            _maxGradientValue = double.MinValue;

            for (var xIndex = 0; xIndex < WidthX; xIndex++)
            {
                for (var yIndex = 0; yIndex < WidthY; yIndex++)
                {
                    var psiGradient = 0d;

                    for (var centerIndex = 0; centerIndex < CentersSettings.CentersCount; centerIndex++)
                    {
                        psiGradient += _muGrids[centerIndex][yIndex, xIndex];
                    }

                    psiGradient -= 1;

                    var oldPsi = _psiGrid[yIndex, xIndex];
                    var newPsi = oldPsi + GetLambdaStep() * psiGradient;

                    _psiGrid[yIndex, xIndex] = newPsi;

                    if (Math.Abs(psiGradient) > _maxGradientValue)
                    {
                        _maxGradientValue = Math.Abs(psiGradient);
                    }
                }
            }
        }

        private bool IsStopConditionSatisfied()
        {
            if (PerformedIterationsCount >= PartitionSettings.MaxIterationsCount)
                return true;

            if (_maxGradientValue <= PartitionSettings.GradientEpsilon)
                return true;

            return false;
        }

        /// <summary>
        /// Converts point in mu grid into world space point.
        /// </summary>
        /// <param name="xIndex">x coordinate of point as index in mu grid.</param>
        /// <param name="yIndex">y coordinate of point as index in mu grid.</param>
        /// <returns></returns>
        private Vector GetPoint(int xIndex, int yIndex)
        {
            var xRatio = (double)xIndex / (WidthX - 1d);
            var yRatio = (double)yIndex / (WidthY - 1d);
            var ratioPoint = VectorUtils.CreateVector(xRatio, yRatio);
            var point = SpaceSettings.MinCorner + ratioPoint.PointwiseMultiply(SpaceSettings.MaxCorner - SpaceSettings.MinCorner);
            return point;
        }

        /// <summary>
        /// Returns lambda step value.
        /// Speed and stability of convergence of method depends on the rule the step is calculated by.
        /// </summary>
        /// <returns></returns>
        private double GetLambdaStep()
        {
            return PartitionSettings.GradientStep / (PerformedIterationsCount + 1d);
        }

        private void SetSingleCenterPsi()
        {
            for (var xIndex = 0; xIndex < WidthX; xIndex++)
            {
                for (var yIndex = 0; yIndex < WidthY; yIndex++)
                {
                    var densityValue = 1d;
                    var m = 2d;
                    var data = CentersSettings.CenterDatas[0];
                    var a = data.A;
                    var w = data.W;
                    var centerPosition = data.Position;
                    var point = GetPoint(xIndex, yIndex);
                    var distance = (point - centerPosition).L2Norm();
                    var psi = -m * densityValue * (distance / w + a);
                    _psiGrid[yIndex, xIndex] = psi;
                }
            }
        }
    }
}
