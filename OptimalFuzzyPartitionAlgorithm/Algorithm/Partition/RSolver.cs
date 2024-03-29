﻿using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using Vector = MathNet.Numerics.LinearAlgebra.Vector<double>;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.Partition
{
    // L2 - Euclidean

    public struct Options
    {
        public double InitialStep;
        public double PrecisionBySubgradient;
        public double IterationsCountToIncreaseStep;
        public double StepIncreaseMultiplier;
        public double StepDecreaseMultiplier;
        public double SpaceStretchCoefficient;
        public double MaximumIterationsCount;
        public double PrecisionByVariable;
    }

    public class RSolver
    {
        public bool IsFinished { get; private set; }
        public int PerformedIterationCount { get; private set; }

        readonly Options options;
        Vector x0;
        Vector x;
        double step;
        Matrix h;
        bool isMaximizationTask;
        Func<Vector, Vector> subgradientEvaluator;
        Vector subgradient;
        Vector newSubgradient;

        public RSolver(Vector initialVector, bool isMaximizationTask, Options options, Func<Vector, Vector> subgradientEvaluator)
        {
            this.isMaximizationTask = isMaximizationTask;
            this.options = options;
            this.subgradientEvaluator = subgradientEvaluator;

            x0 = initialVector;
            x = x0;
            step = options.InitialStep;

            h = Matrix.Build.Dense(x0.Count, x0.Count);
            for (int i = 0; i < x0.Count; ++i)
                h[i, i] = 1;

            subgradient = subgradientEvaluator(x0);
            newSubgradient = subgradient;
        }

        public Vector GetCurrentX()
        {
            return x.Clone();
        }

        public void DoIteration()
        {
            if (IsFinished)
            {
                throw new Exception("Unexpected DoIteration() call. Algorithm already finished its work.");
            }

            //iteration body
            x0 = x;
            subgradient = newSubgradient;

            //if (subgradient.L2Norm() < options.PrecisionBySubgradient)
            //{
            //    // finish by subgradient precision
            //    IsFinished = true;
            //    return;
            //}

            var direction = (h * subgradient.ToColumnMatrix()).Column(0);
            direction /= Math.Sqrt(direction * subgradient);

            var stepsCount = 0;
            var ch = 0d;
            do
            {
                ++stepsCount;

                if (isMaximizationTask)
                    x += step * direction;
                else
                    x -= step * direction;

                newSubgradient = subgradientEvaluator(x);

                if (stepsCount > options.IterationsCountToIncreaseStep)
                    step *= options.StepIncreaseMultiplier;

                var v = 0.0;
                for (int i = 0; i < direction.Count; i++)
                {
                    var m = direction[i] * newSubgradient[i];
                    v += m;
                }

                ch = direction * newSubgradient;
            }
            while (ch > 0);

            if (stepsCount == 1)
                step *= options.StepDecreaseMultiplier;

            var gamma = newSubgradient - subgradient;

            if (gamma.L2Norm() > 1e-8)
            {
                var hGamma = h * gamma.ToColumnMatrix();
                h = h - (1 - 1 / options.SpaceStretchCoefficient / options.SpaceStretchCoefficient) * (hGamma * hGamma.Transpose()) / (gamma.ToRowMatrix() * hGamma)[0, 0];
            }

            PerformedIterationCount++;

            // finish condition check -
            // distance between current and previous iteration positions is less then epsilon
            // or iterations count exceeds max iterations count
            //var variableDiff = (x - x0).L2Norm();
            var diffs = new List<double>();
            for (var i = 0; i < x.Count / 2; i++)
            {
                var currentPos = VectorUtils.CreateVector(x[i * 2], x[i * 2 + 1]);
                var previousPos = VectorUtils.CreateVector(x0[i * 2], x0[i * 2 + 1]);
                var diff = (currentPos - previousPos).L2Norm();
                diffs.Add(diff);
            }
            var variableDiff = diffs.Max();

            IsFinished = variableDiff <= options.PrecisionByVariable
                      || PerformedIterationCount >= options.MaximumIterationsCount;
        }
    }
}
