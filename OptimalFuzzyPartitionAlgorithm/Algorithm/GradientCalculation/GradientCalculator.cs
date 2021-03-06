﻿using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Calculated the gradient vector for a single center (one gradient vector for one point-generator).
    /// </summary>
    public class GradientCalculator
    {
        private PartitionSettings Settings { get; }

        private Vector<double> _centerPosition;

        public GradientCalculator(PartitionSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Calculated gradient.
        /// </summary>
        /// <param name="centerPosition">Current optimal center position, τ*.</param>
        /// <returns>Gradient vector</returns>
        public Vector<double> CalculateGradientForCenter(Vector<double> centerPosition, GridValueInterpolator muValueInterpolator)
        {
            _centerPosition = centerPosition;

            if (Settings.SpaceSettings.MetricsType != MetricsType.Euclidean)
                throw new NotImplementedException($"Визначення градієнту для метрики {Settings.SpaceSettings.MetricsType} не реалізовано");

            var vector = Vector<double>.Build.Sparse(Settings.SpaceSettings.DimensionsCount);

            for (var i = 0; i < Settings.SpaceSettings.DimensionsCount; i++)
            {
                var dimensionIndex = i;

                var value = GaussLegendreRule.Integrate(
                    (x, y) =>
                    {
                        var densityValue = 1d;
                        var mu = muValueInterpolator.GetGridValueAtPoint(x, y);

                        var point = VectorUtils.CreateVector(x, y);
                        var distanceGradientValue = CalculateDistanceGradientValue(point, dimensionIndex);

                        var integralFunctionValue = distanceGradientValue * densityValue * mu * mu;

                        return integralFunctionValue;
                    },
                    Settings.SpaceSettings.MinCorner[0],
                    Settings.SpaceSettings.MaxCorner[0],
                    Settings.SpaceSettings.MinCorner[1],
                    Settings.SpaceSettings.MaxCorner[1],
                    Settings.FuzzyPartitionPlacingCentersSettings.GaussLegendreIntegralOrder
                );

                vector[i] = value;
            }

            return vector;
        }

        /// <summary>
        /// Calculate gradient value of the function of the Euclidean distance c(x, τ).
        /// </summary>
        private double CalculateDistanceGradientValue(Vector<double> point, int dimensionIndex)
        {
            var distance = (_centerPosition - point).L2Norm();
            var diff = _centerPosition[dimensionIndex] - point[dimensionIndex];
            var value = diff / distance;
            return value;
        }
    }
}