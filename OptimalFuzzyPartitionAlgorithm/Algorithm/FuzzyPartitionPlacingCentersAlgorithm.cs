using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class FuzzyPartitionPlacingCentersAlgorithm
    {
        public int PerformedIterationCount { get; private set; }

        public event Action<int> OnIterationPerformed;

        private readonly PartitionSettings _settings;
        private readonly List<RAlgorithmSolverBForm> _rAlgorithmSolvers;

        public FuzzyPartitionPlacingCentersAlgorithm(PartitionSettings partitionSettings, List<Vector<double>> zeroTaus)
        {
            _settings = partitionSettings;
            PerformedIterationCount = 0;

            _rAlgorithmSolvers = new List<RAlgorithmSolverBForm>();
            for (var centerIndex = 0; centerIndex < partitionSettings.CentersSettings.CentersCount; centerIndex++)
            {
                var zeroTau = zeroTaus[centerIndex];
                var rAlgorithm = new RAlgorithmSolverBForm(zeroTau, null, _settings.RAlgorithmSettings.SpaceStretchFactor);
                _rAlgorithmSolvers.Add(rAlgorithm);
            }
        }

        public void DoIteration()
        {
            SetGradients();

            for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
            {

            }
        }

        private void SetGradients()
        {
            
        }

        public bool IsStopConditionSatisfied()
        {
            return PerformedIterationCount < _settings.FuzzyPartitionPlacingCentersSettings.MaxIterationsCount;
        }
    }
}