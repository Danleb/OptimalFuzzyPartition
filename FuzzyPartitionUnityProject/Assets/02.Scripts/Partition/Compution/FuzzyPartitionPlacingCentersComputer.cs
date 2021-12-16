using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation;
using OptimalFuzzyPartitionAlgorithm.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionPlacingCentersComputer : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;

        private Stopwatch _timer;
        private PartitionSettings _settings;
        private FuzzyPartitionPlacingCentersAlgorithm _placingAlgorithmExecutor;

        private void Awake()
        {
            _timer = new Stopwatch();
        }

        public void Init(PartitionSettings settings)
        {
            GC.Collect();

            _settings = settings;

            _timer.Restart();

            _partitionFixedCentersComputer.Init(_settings);

            _placingAlgorithmExecutor = new FuzzyPartitionPlacingCentersAlgorithm(_settings,
                 //GradientCalculatorBuilder.CreateGradientEvaluatorCPU(_settings)
                 GradientCalculatorBuilder.CreateGradientEvaluator(_settings, centersSettings =>
                 {
                     _settings.CentersSettings = centersSettings;
                     _partitionFixedCentersComputer.Init(_settings);
                     var partitionTexture = _partitionFixedCentersComputer.Run();
                     var muGridValueInterpolators = ComputeBufferToGridConverter.GetGridValueInterpolators(partitionTexture, settings);
                     return muGridValueInterpolators;
                 })
             );
        }

        public List<CenterData> Run(out int iteratiosCount)
        {
            while (!_placingAlgorithmExecutor.IsFinished)
            {
                Logger.Trace($"Iteration number {_placingAlgorithmExecutor.PerformedIterationCount + 1}");
                _placingAlgorithmExecutor.DoIteration();

                // todo yield and show current partition

            }

            _timer.Stop();

            var t = TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds);
            var timeString = $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s:{t.Milliseconds:D3}ms";
            Logger.Debug($"Optimal placing partition global time: {timeString}");

            iteratiosCount = _placingAlgorithmExecutor.PerformedIterationCount;
            Logger.Debug($"Performed iterations count: {iteratiosCount}");

            return _placingAlgorithmExecutor.GetCurrentCenters();
        }
    }
}
