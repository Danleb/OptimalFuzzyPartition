using MathNet.Numerics.LinearAlgebra;
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

        public FuzzyPartitionPlacingCentersAlgorithm(PartitionSettings partitionSettings, List<Vector<double>> zeroCenters, List<GridValueInterpolator> zeroMuGrids)
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

        public void DoIteration(List<GridValueInterpolator> muGrids)
        {
            for (var centerIndex = 0; centerIndex < _settings.CentersSettings.CentersCount; centerIndex++)
            {
                if (_settings.CentersSettings.CenterDatas[centerIndex].IsFixed)
                    continue;

                var rAlgorithm = _rAlgorithmSolvers[centerIndex];

                _previousTaus[centerIndex] = rAlgorithm.CurrentX;

                var centerIndex2 = centerIndex;

                rAlgorithm.FunctionGradient = vector =>
                {
                    var gradientCalculator = new GradientCalculator(_settings);
                    var gradientVector = gradientCalculator.CalculateGradientForCenter(rAlgorithm.CurrentX, muGrids[centerIndex2]);
                    return gradientVector;
                };

                rAlgorithm.h = _settings.RAlgorithmSettings.H0;// / (PerformedIterationCount + 1);// 0.5;//TODO adaptive step

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
            if (_settings.RAlgorithmSettings.MaxIterationsCount == 0)
                return true;
            if (PerformedIterationCount == 0)
                return false;

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
                //Trace.WriteLine("Algorithm stop by centers max delta");
                //return true;
            }

            return false;
        }
    }
}