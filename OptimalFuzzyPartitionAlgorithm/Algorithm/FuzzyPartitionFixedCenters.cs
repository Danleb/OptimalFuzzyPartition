using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Рассчитывает значение функции принадлежности для всех центров во всех узлах сетки.
    /// Таким, образом, для данного набора центров решает задачу об оптимальном нечетком разбиении четкого множества без ограничений с заданными центрами на плоскости 2d.
    /// </summary>
    public class FuzzyPartitionFixedCenters
    {
        /// <summary>
        /// Экспоненциальный коэффициент m.
        /// </summary>
        public const int M = 2;

        /// <summary>
        /// Настройки нечеткого разбиения.
        /// </summary>
        public readonly PartitionSettings Settings;
        public readonly Func<int, double> StepFunction;

        public int GridPointsCountX => Settings.GridSize[0];
        public int GridPointsCountY => Settings.GridSize[1];

        public int PerformedIterationsCount { get; private set; } = 0;

        /// <summary>
        /// Список значение функции μ(x) для каждого центра.
        /// Так как расчеты численные, то функция μ(x) представлена в виде матрицы значений этой функции в точках плоскости.
        /// </summary>
        private readonly List<Matrix<double>> MuMatrices;

        private readonly Matrix<double> Psi;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partitionSettings">Настройки для построения разбиения.</param>
        /// <param name="stepFunction">Функция, которая для заданной итерации возвращает размер шага градиента</param>
        public FuzzyPartitionFixedCenters(PartitionSettings partitionSettings, Func<int, double> stepFunction)
        {
            if (partitionSettings.DimensionsCount != 2)
                throw new Exception("DimensionsCount must be 2");

            Settings = partitionSettings;

            //
            Psi = Matrix<double>.Build.Sparse(GridPointsCountX, GridPointsCountY);

            for (var xCounter = 0; xCounter < GridPointsCountX; xCounter++)
            {
                for (var yCounter = 0; yCounter < GridPointsCountY; yCounter++)
                {
                    Psi[xCounter, yCounter] = 1;
                }
            }

            //
            MuMatrices = new List<Matrix<double>>();

            var value = 1d / Settings.CentersCount;

            for (var centerIndex = 0; centerIndex < Settings.CentersCount; centerIndex++)
            {
                var muMatrix = Matrix<double>.Build.Sparse(GridPointsCountX, GridPointsCountY);

                for (var xCounter = 0; xCounter < GridPointsCountX; xCounter++)
                {
                    for (var yCounter = 0; yCounter < GridPointsCountY; yCounter++)
                    {
                        muMatrix[xCounter, yCounter] = value;
                    }
                }

                MuMatrices.Add(muMatrix);
            }
        }

        public void CreatePartition()
        {
            while (true)
            {
                DoIteration();

                if (IsStopConditionsSatisfied())
                    break;
            }
        }

        /// <summary>
        /// Пересчитать все функции принадлежности μ(x) для каждого центра.
        /// </summary>
        public void ReCalculateMuFunctions()
        {
            for (var centerIndex = 0; centerIndex < Settings.CentersCount; centerIndex++)
            {
                for (var xCounter = 0; xCounter < GridPointsCountX; xCounter++)
                {
                    for (var yCounter = 0; yCounter < GridPointsCountY; yCounter++)
                    {
                        var muValue = CalculateNormalizedMuValue(centerIndex, xCounter, yCounter);

                        MuMatrices[centerIndex][xCounter, yCounter] = muValue;
                    }
                }
            }
        }

        private double CalculateNormalizedMuValue(int centerIndex, int xCounter, int yCounter)
        {
            //считаем mu
            var xRatio = xCounter / (double)(GridPointsCountX - 1);
            var x = Settings.MinCorner[0] * (1d - xRatio) + Settings.MaxCorner[0] * xRatio;

            var yRatio = yCounter / (double)(GridPointsCountY - 1);
            var y = Settings.MinCorner[1] * (1d - yRatio) + Settings.MaxCorner[1] * yRatio;

            var point = Vector<double>.Build.SparseOfArray(new[] { x, y });

            var densityValue = Settings.Density(point);
            var centerPoint = Settings.CenterPositions[centerIndex];
            var distanceValue = Settings.Distance(point, centerPoint);
            var w = Settings.MultiplicativeCoefficients[centerIndex];
            var a = Settings.AdditiveCoefficients[centerIndex];
            var psi = Psi[xCounter, yCounter];

            var distanceExpressionValue = distanceValue / w + a;
            var muValue = -psi / (densityValue * distanceExpressionValue);

            //нормализуем
            if (muValue >= 0 && muValue <= 1)
                return muValue;

            var previousIterationMu = MuMatrices[centerIndex][xCounter, yCounter];
            var grad = psi + M * previousIterationMu * distanceExpressionValue * densityValue;

            var normalized = 0.5d * (1d - Math.Sign(grad));

            return normalized;
        }

        private bool IsStopConditionsSatisfied()
        {
            for (var xCounter = 0; xCounter < GridPointsCountX; xCounter++)
            {
                for (var yCounter = 0; yCounter < GridPointsCountY; yCounter++)
                {
                    //var grad

                    if (false)
                        return false;
                }
            }

            return true;
        }

        private void DoIteration()
        {
            ReCalculateMuFunctions();

            var lambda = StepFunction(PerformedIterationsCount);

            for (var xCounter = 0; xCounter < GridPointsCountX; xCounter++)
            {
                for (var yCounter = 0; yCounter < GridPointsCountY; yCounter++)
                {
                    var psi = Psi[xCounter, yCounter];
                    var psi1 = psi + lambda * CalculateGradientByPsiZero(xCounter, yCounter);

                    Psi[xCounter, yCounter] = psi1;
                }
            }

            PerformedIterationsCount++;
        }

        /// <summary>
        /// Посчитать градиент функционала Лагранжа этой задачи по пси-ноль.
        /// Тут градиент это число, а не вектор.
        /// (xCounter, yCounter) - Точка, в которой надо посчитать значение градиента.
        /// </summary>
        /// <returns>Значение градиента</returns>
        private double CalculateGradientByPsiZero(int xCounter, int yCounter)
        {
            var muSum = 0d;

            for (var centerIndex = 0; centerIndex < Settings.CentersCount; centerIndex++)
                muSum += MuMatrices[centerIndex][xCounter, yCounter];

            var grad = muSum - 1;

            return grad;
        }
    }
}