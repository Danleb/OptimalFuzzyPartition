using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionPlacingCentersComputer : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;
        [SerializeField] private MuConverter _muConverter;

        private PartitionSettings _settings;

        private FuzzyPartitionPlacingCentersAlgorithm _algorithm;

        public void Init(PartitionSettings settings)
        {
            _settings = settings;

            _partitionFixedCentersComputer.Init(_settings);

            var zeroTaus = new List<Vector<double>>();
            for (var i = 0; i < settings.CentersSettings.CentersCount; i++)
                zeroTaus.Add(settings.SpaceSettings.MinCorner.Clone());
            SetCentersPositions(zeroTaus);

            var muGrids = GetMuGrids(settings);

            _algorithm = new FuzzyPartitionPlacingCentersAlgorithm(_settings, zeroTaus, muGrids);
        }

        private List<Matrix<double>> GetMuGrids(PartitionSettings settings)
        {
            var partitionTexture = _partitionFixedCentersComputer.Run();
            var muGrids = _muConverter.ConvertMuGridsTexture(partitionTexture, settings);
            return muGrids;
        }

        public List<Vector<double>> Run()
        {
            do
            {
                Trace.WriteLine($"Iteration number {_algorithm.PerformedIterationCount + 1}");

                var centers = _algorithm.GetCenters();
                SetCentersPositions(centers);

                var muGrids = GetMuGrids(_settings);

                _algorithm.DoIteration(muGrids);

            } while (!_algorithm.IsStopConditionSatisfied());

            return _algorithm.GetCenters();
        }

        private void SetCentersPositions(List<Vector<double>> centers)
        {
            for (var i = 0; i < _settings.CentersSettings.CentersCount; i++)
            {
                _settings.CentersSettings.CenterDatas[i].Position = centers[i];
                Trace.WriteLine($"Tau #{i + 1} = ({centers[i][0]}; {centers[i][1]})");
            }
        }
    }
}