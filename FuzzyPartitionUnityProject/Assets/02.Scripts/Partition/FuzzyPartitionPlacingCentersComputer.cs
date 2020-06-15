using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionPlacingCentersComputer : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;
        [SerializeField] private MuConverter _muConverter;
        [SerializeField] private Vector2[] _zeroTausManual;

        private PartitionSettings _settings;

        private FuzzyPartitionPlacingCentersAlgorithm _algorithm;

        public void Init(PartitionSettings settings)
        {
            _settings = settings;

            _partitionFixedCentersComputer.Init(_settings);

            var zeroTaus = GetZeroIterationCentersPositions(settings);

            SetCentersPositions(zeroTaus);

            var muGrids = GetMuGrids(settings);

            _algorithm = new FuzzyPartitionPlacingCentersAlgorithm(_settings, zeroTaus, muGrids);
        }

        private List<Vector<double>> GetZeroIterationCentersPositions(PartitionSettings settings)
        {
            var zeroTaus = new List<Vector<double>>();

            var p1 = settings.SpaceSettings.MinCorner;
            var p2 = VectorUtils.CreateVector(p1[0], settings.SpaceSettings.MaxCorner[1]);

            for (var i = 0; i < settings.CentersSettings.CentersCount; i++)
            {
                //var zeroTau = settings.SpaceSettings.MinCorner.Clone();
                var zeroTau = p1 + ((i + 1) / (settings.CentersSettings.CentersCount + 1d)) * (p2 - p1);

                zeroTaus.Add(zeroTau);
            }

            //zeroTaus = _zeroTausManual.Select(v => VectorUtils.CreateVector(v.x, v.y)).ToList();
            return zeroTaus;
        }

        private List<Matrix<double>> GetMuGrids(PartitionSettings settings)
        {
			return new FuzzyPartitionFixedCentersAlgorithm(_settings).BuildPartition();
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

            _partitionFixedCentersComputer.Release();

            return _algorithm.GetCenters();
        }

        private void SetCentersPositions(List<Vector<double>> centers)
        {
            for (var i = 0; i < _settings.CentersSettings.CentersCount; i++)
            {
                _settings.CentersSettings.CenterDatas[i].Position = centers[i];
                Trace.WriteLine($"Center #{i + 1} = ({centers[i][0]}; {centers[i][1]})");
            }
        }
    }
}