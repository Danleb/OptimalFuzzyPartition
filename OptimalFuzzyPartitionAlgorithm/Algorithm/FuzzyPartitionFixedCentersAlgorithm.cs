using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class FuzzyPartitionFixedCentersAlgorithm
    {
        public int PerformedIterationsCount { get; private set; }

        private readonly PartitionSettings _settings;
        private List<Matrix<double>> _muGrids;
        private Matrix<double> _psiGrid;
        private double _maxGradientValue;

        private int WidthX => _settings.SpaceSettings.GridSize[0];
        private int WidthY => _settings.SpaceSettings.GridSize[1];

        public FuzzyPartitionFixedCentersAlgorithm(PartitionSettings partitionSettings)
        {
            _settings = partitionSettings;
        }

        public List<Matrix<double>> BuildPartition()
        {
            Init();

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

        private void Init()
        {
            PerformedIterationsCount = 0;

            _muGrids = new List<Matrix<double>>();
            var value = 1d / _settings.CentersSettings.CentersCount;

            for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
            {
                var m = Matrix<double>.Build.Sparse(WidthX, WidthY);

                for (var i = 0; i < WidthX; i++)
                {
                    for (var j = 0; j < WidthY; j++)
                    {
                        m[i, j] = value;
                    }
                }

                _muGrids.Add(m);
            }

            _psiGrid = Matrix<double>.Build.Sparse(WidthX, WidthY);

            for (var xIndex = 0; xIndex < WidthX; xIndex++)
            {
                for (var yIndex = 0; yIndex < WidthY; yIndex++)
                {
                    _psiGrid[xIndex, yIndex] = 1d;
                }
            }
        }

        private void UpdateMuValues()
        {
            for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
            {
                for (var xIndex = 0; xIndex < WidthX; xIndex++)
                {
                    for (var yIndex = 0; yIndex < WidthY; yIndex++)
                    {
                        var densityValue = 1d;
                        var m = 2d;
                        var data = _settings.CentersSettings.CenterDatas[centerIndex];
                        var a = data.A;
                        var w = data.W;
                        var centerPosition = data.Position;
                        var point = GetPoint(xIndex, yIndex);
                        var distance = (point - centerPosition).L2Norm();
                        var psiValue = _psiGrid[xIndex, yIndex];
                        var oldMuValue = _muGrids[centerIndex][xIndex, yIndex];

                        var newMuValue = -psiValue / (m * densityValue * (distance / w + a));

                        if (newMuValue <= 0 || newMuValue >= 1)
                        {
                            var muGradient = psiValue + m * oldMuValue * (distance / w + a) * densityValue;
                            newMuValue = 0.5d * (1d - Math.Sign(muGradient));
                        }

                        _muGrids[centerIndex][xIndex, yIndex] = newMuValue;
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

                    for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
                    {
                        psiGradient += _muGrids[centerIndex][xIndex, yIndex];
                    }

                    psiGradient -= 1;

                    var oldPsi = _psiGrid[xIndex, yIndex];
                    var newPsi = oldPsi + GetGradientStep() * psiGradient;

                    _psiGrid[xIndex, yIndex] = newPsi;

                    if (Math.Abs(psiGradient) > _maxGradientValue)
                    {
                        _maxGradientValue = Math.Abs(psiGradient);
                    }
                }
            }
        }

        private bool IsStopConditionSatisfied()
        {
            if (PerformedIterationsCount >= _settings.FuzzyPartitionFixedCentersSettings.MaxIterationsCount)
                return true;

            //if (_maxGradientValue <= _settings.FuzzyPartitionFixedCentersSettings.GradientEpsilon)
                //return true;

            return false;
        }

        private Vector<double> GetPoint(int xIndex, int yIndex)
        {
            var xRatio = (double)xIndex / (WidthX - 1d);
            var yRatio = (double)yIndex / (WidthY - 1d);
            var ratioPoint = VectorUtils.CreateVector(xRatio, yRatio);
            var point = _settings.SpaceSettings.MinCorner + ratioPoint.PointwiseMultiply(_settings.SpaceSettings.MaxCorner - _settings.SpaceSettings.MinCorner);
            return point;
        }

        private double GetGradientStep()
        {
            const double c = 1d;
            return 0.1;//c / (PerformedIterationsCount + 1);
        }
    }
}