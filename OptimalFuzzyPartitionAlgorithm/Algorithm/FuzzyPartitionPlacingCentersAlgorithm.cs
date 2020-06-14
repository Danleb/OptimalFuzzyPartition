﻿using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class FuzzyPartitionPlacingCentersAlgorithm
    {
        public int PerformedIterationCount { get; private set; }

        private readonly PartitionSettings _settings;
        private readonly List<RAlgorithmSolverBForm> _rAlgorithmSolvers;
        private readonly List<Vector<double>> _previousTaus = new List<Vector<double>>();

        public FuzzyPartitionPlacingCentersAlgorithm(PartitionSettings partitionSettings, List<Vector<double>> zeroCenters, List<Matrix<double>> zeroMuGrids)
        {
            PerformedIterationCount = 0;
            _settings = partitionSettings;
            _rAlgorithmSolvers = new List<RAlgorithmSolverBForm>();
            _previousTaus.AddRange(Enumerable.Repeat(default(Vector<double>), _settings.CentersSettings.CentersCount));

            for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
            {
                var zeroCenter = zeroCenters[centerIndex];
                var gradientCalculator = new GradientCalculator(_settings);
                var gradientVector = gradientCalculator.CalculateGradientForCenter(zeroCenter, zeroMuGrids[centerIndex]);
                var rAlgorithm = new RAlgorithmSolverBForm(zeroCenter, _ => gradientVector, _settings.RAlgorithmSettings.SpaceStretchFactor);
                _rAlgorithmSolvers.Add(rAlgorithm);
            }
        }

        public void DoIteration(List<Matrix<double>> muGrids)
        {
            for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
            {
                var rAlgorithm = _rAlgorithmSolvers[centerIndex];

                _previousTaus[centerIndex] = rAlgorithm.CurrentX;

                rAlgorithm.FunctionGradient = vector =>
                {
                    var gradientCalculator = new GradientCalculator(_settings);
                    var gradientVector = gradientCalculator.CalculateGradientForCenter(rAlgorithm.CurrentX, muGrids[centerIndex]);
                    return gradientVector;
                };

                rAlgorithm.h = 0.5;//TODO adaptive step

                rAlgorithm.DoIteration();
            }

            PerformedIterationCount++;
        }

        public List<Vector<double>> GetCenters()
        {
            return _rAlgorithmSolvers.Select(v => v.CurrentX).ToList();
        }

        public bool IsStopConditionSatisfied()
        {
            var tausDeltas = _previousTaus.Select((v, i) => (v - _rAlgorithmSolvers[i].CurrentX).L2Norm());
            var tausDelta = tausDeltas.Max();

            Trace.WriteLine($"Taus max delta = {tausDelta}");
            Trace.WriteLine($"Iteration Number = {PerformedIterationCount}; Max = {_settings.RAlgorithmSettings.MaxIterationsCount}");

            if (PerformedIterationCount >= _settings.RAlgorithmSettings.MaxIterationsCount)
            {
                Trace.WriteLine("Algorithm stop by iterations count.");
                return true;
            }

            if (tausDelta < _settings.FuzzyPartitionPlacingCentersSettings.CentersDeltaEpsilon)
            {
                Trace.WriteLine("Algorithm stop by centers max delta");
                return true;
            }

            return false;
        }
    }
}