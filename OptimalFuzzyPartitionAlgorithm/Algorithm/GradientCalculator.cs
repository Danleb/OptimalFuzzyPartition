using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartition.ViewModel;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Высчитывает значение градиента.
    /// </summary>
    public class GradientCalculator
    {
        private PartitionSettings Settings { get; }

        private List<Vector<double>> _centersPositions;

        public GradientCalculator(PartitionSettings settings)
        {
            Settings = settings;
        }

        public Vector<double> CalculateGradientForCenter(int centerIndex, List<Vector<double>> centersPositions, Matrix<double> psi, List<Matrix<double>> mu)
        {
            for (var i = 0; i < psi.RowCount; i++)
            {
                for (var u = 0; u < psi.ColumnCount; u++)
                {
                    psi[i, u] *= psi[i, u];
                }
            }

            _centersPositions = centersPositions;

            if (Settings.SpaceSettings.MetricsType != MetricsType.Euclidean)
                throw new NotImplementedException($"Визначення градієнту метрики {Settings.SpaceSettings.MetricsType} не реалізовано");

            var vector = Vector<double>.Build.Sparse(Settings.SpaceSettings.DimensionsCount);

            for (var i = 0; i < Settings.SpaceSettings.DimensionsCount; i++)
            {
                var value = GaussLegendreRule.Integrate(
                    (x, y) =>
                    {
                        //var distanceVector

                        return;
                    },
                    Settings.SpaceSettings.MinCorner[0],
                    Settings.SpaceSettings.MaxCorner[0],
                    Settings.SpaceSettings.MinCorner[1],
                    Settings.SpaceSettings.MaxCorner[1]
                );

                value /= 4d;

                vector[i] = value;
            }

            return vector;
        }

        private Vector<double> CalculateDistanceGradientVector(int centerIndex, Vector<double> point)
        {
            var vector = Vector<double>.Build.Sparse(Settings.SpaceSettings.DimensionsCount);//to init?
            var centerPosition = _centersPositions[centerIndex];
            var distance = (centerPosition - point).L2Norm();

            for (var i = 0; i < Settings.SpaceSettings.DimensionsCount; i++)
            {
                var diff = point[i] - centerPosition[i];
                vector[i] = diff / distance;
            }

            return vector;
        }
    }
}