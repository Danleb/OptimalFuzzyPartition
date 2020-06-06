using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Рассчитывает значение функции принадлежности для всех центров во всех узлах сетки.
    /// </summary>
    public class MembershipFunctionCalculator
    {
        private PartitionSettings Settings { get; }

        public MembershipFunctionCalculator(PartitionSettings settings)
        {
            Settings = settings;
        }

        public List<Matrix<double>> CalculateMembershipFunctions()
        {
            var list = new List<Matrix<double>>();

            for (var centerIndex = 0; centerIndex < Settings.CentersCount; centerIndex++)
            {
                var matrix = CalculateMembersFunction(centerIndex);
                list.Add(matrix);
            }

            return list;
        }

        private Matrix<double> CalculateMembersFunction(int centerIndex)
        {
            var matrix = Matrix<double>.Build.Sparse(Settings.GridSize[0], Settings.GridSize[1]);

            for (var xCounter = 0; xCounter < Settings.GridSize[0]; xCounter++)
            {
                for (var yCounter = 0; yCounter < Settings.GridSize[1]; yCounter++)
                {
                    var value = 0;

                    matrix[xCounter, yCounter] = value;
                }
            }

            return matrix;
        }

        private double Normalize(double rawValue)
        {
            if (rawValue >= 0 && rawValue <= 1)
                return rawValue;

            var grad = 0;

            //var normalized = 0.5d * (1 - Math.Sign());

            return 0;
        }
    }
}
