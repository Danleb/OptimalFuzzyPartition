using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Берет интеграл с помощью метода левый прямоугольников на n-мерном параллелепипеде.
    /// </summary>
    public class IntegralCalculator
    {
        public Vector<double> MinCorner { get; private set; }

        public Vector<double> MaxCorner { get; private set; }

        public IReadOnlyList<int> GridSize { get; private set; }

        private Vector<double> GridSizeDouble { get; set; }

        /// <summary>
        /// Разница между крайними точками области определения.
        /// Это вся область определения, n-мерный параллелепипед.
        /// </summary>
        public Vector<double> Diff { get; private set; }

        /// <summary>
        /// Размеры одной ячейки сетки. Сетка равномерно разбивает всю область определения.
        /// Для двумерного случая это размеры квадрата XxY.
        /// </summary>
        public Vector<double> SingleGridSquare { get; private set; }

        public readonly double SingleSquareVolume;

        public Func<Vector<double>, double> IntegralFunction { get; private set; }

        public IntegralCalculator(Vector<double> minCorner, Vector<double> maxCorner, IReadOnlyList<int> gridSize, Func<Vector<double>, double> integralFunction)
        {
            if (minCorner.Count != 2)
                throw new NotImplementedException($"Integral calculator supports only 2-dimensional integral, but {nameof(minCorner)} vector is of {minCorner.Count} dimensions.");

            MinCorner = minCorner;
            MaxCorner = maxCorner;
            GridSize = gridSize;
            IntegralFunction = integralFunction;

            GridSizeDouble = Vector<double>.Build.SparseOfEnumerable(gridSize.Select(v => (double)v));

            Diff = maxCorner - minCorner;
            SingleGridSquare = Diff / GridSizeDouble;
            SingleSquareVolume = SingleGridSquare.Aggregate((a, b) => a * b);

            Trace.WriteLine($"{nameof(Diff)}={Diff}");
            Trace.WriteLine($"{nameof(SingleGridSquare)}={SingleGridSquare}");
            Trace.WriteLine($"{nameof(SingleSquareVolume)}={SingleSquareVolume}");
        }

        public double CalculateIntegral()
        {
            var integralValue = 0d;

            for (var xCounter = 0; xCounter < GridSize[0]; xCounter++)
            {
                for (var yCounter = 0; yCounter < GridSize[1]; yCounter++)
                {
                    var leftPointValue = GetFunctionValueAtGridNode(xCounter, yCounter);
                    var rightPointValue = GetFunctionValueAtGridNode(xCounter + 1, yCounter + 1);
                    var value = (leftPointValue + rightPointValue) / 2d;

                    //var squareVolume = leftPointValue * SingleSquareVolume;
                    var squareVolume = value * SingleSquareVolume;

                    integralValue += squareVolume;
                }
            }

            return integralValue;
        }

        private double GetFunctionValueAtGridNode(int xCounter, int yCounter)
        {
            var pointCounter = VectorUtils.CreateVector((double)xCounter, yCounter);
            var pointRatioIndex = pointCounter / GridSizeDouble;
            var point = MinCorner + Diff.PointwiseMultiply(pointRatioIndex);
            var functionValue = IntegralFunction(point);
            Trace.WriteLine($"PointCounter={pointCounter} PointRatio={pointRatioIndex}; Point={point}; FunctionValue={functionValue}");
            return functionValue;
        }
    }
}