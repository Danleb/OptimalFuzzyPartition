using OptimalFuzzyPartitionAlgorithm.Algorithm.Partition;
using OptimalFuzzyPartitionAlgorithm.Settings;
using System;
using System.Collections.Generic;
using Vector = MathNet.Numerics.LinearAlgebra.Vector<double>;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    public class FuzzyPartitionPlacingCentersAlgorithm
    {
        public static List<CenterData> GetCenterDatas(CentersSettings centersSettings, Vector currentX)
        {
            var list = new List<CenterData>();
            var placingCentersProcessed = 0;

            for (var i = 0; i < centersSettings.CentersCount; i++)
            {
                if (centersSettings.CenterDatas[i].IsFixed)
                {
                    list.Add(centersSettings.CenterDatas[i]);
                }
                else
                {
                    var position = Vector.Build.Dense(2);
                    position[0] = currentX[2 * placingCentersProcessed];
                    position[1] = currentX[2 * placingCentersProcessed + 1];
                    var centerData = new CenterData
                    {
                        Position = position,
                        A = centersSettings.CenterDatas[i].A,
                        W = centersSettings.CenterDatas[i].W,
                        IsFixed = centersSettings.CenterDatas[i].IsFixed,
                    };
                    list.Add(centerData);
                    placingCentersProcessed++;
                }
            }

            return list;
        }

        public int PerformedIterationCount => _rSolver.PerformedIterationCount;
        public bool IsFinished => _rSolver.IsFinished;

        private readonly PartitionSettings _settings;
        private RSolver _rSolver;

        public FuzzyPartitionPlacingCentersAlgorithm(PartitionSettings partitionSettings, Func<Vector, Vector> subgradientEvaluator)
        {
            _settings = partitionSettings;

            const int DimensionsCount = 2;
            var valuesCount = _settings.CentersSettings.PlacingCentersCount * DimensionsCount;
            var initialVector = Vector.Build.Dense(valuesCount, 0);

            for (var i = 0; i < _settings.CentersSettings.PlacingCentersCount; i++)
            {
                var value = 1d / _settings.CentersSettings.PlacingCentersCount * i;
                initialVector[i * 2] = value;
            }

            var options = new Options
            {
                SpaceStretchCoefficient = partitionSettings.RAlgorithmSettings.SpaceStretchFactor,
                InitialStep = partitionSettings.RAlgorithmSettings.H0,
                MaximumIterationsCount = partitionSettings.RAlgorithmSettings.MaxIterationsCount,

                PrecisionByVariable = partitionSettings.FuzzyPartitionPlacingCentersSettings.CentersDeltaEpsilon,

                IterationsCountToIncreaseStep = 3,
                PrecisionBySubgradient = 0.001,
                StepDecreaseMultiplier = 0.9,
                StepIncreaseMultiplier = 1.1
            };

            _rSolver = new RSolver(initialVector, false, options, subgradientEvaluator);
        }

        public void DoIteration()
        {
            _rSolver.DoIteration();
        }

        /// <summary>
        /// Returns list of current positions of generation centers.
        /// If center position is fixed, it will remain the same in all calls.
        /// If center position is placing, it will be changing throught the iterations.
        /// </summary>
        /// <returns>List of points in 2d Euclidean space.</returns>
        public List<CenterData> GetCurrentCenters()
        {
            var currentX = _rSolver.GetCurrentX();
            return GetCenterDatas(_settings.CentersSettings, currentX);
        }
    }
}