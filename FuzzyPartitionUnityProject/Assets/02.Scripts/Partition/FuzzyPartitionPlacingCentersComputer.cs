using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionPlacingCentersComputer : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;
        [SerializeField] private MuConverter _muConverter;
        [SerializeField] private Vector2[] _zeroTausManual;

        private Stopwatch _timer;
        private PartitionSettings _settings;
        private FuzzyPartitionPlacingCentersAlgorithm _placingAlgorithm;

        public void Init(PartitionSettings settings)
        {
            _settings = settings;

            _timer = new Stopwatch();
            _timer.Start();

            _partitionFixedCentersComputer.Init(_settings);

            var zeroTaus = GetZeroIterationCentersPositions(settings);

            SetCentersPositions(zeroTaus);

            var muGrids = GetMuGrids(settings);

            _placingAlgorithm = new FuzzyPartitionPlacingCentersAlgorithm(_settings, zeroTaus, muGrids);
            _timer.Stop();
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

        private List<MuValueInterpolator> GetMuGrids(PartitionSettings settings)
        {
            //var muMatrices = new FuzzyPartitionFixedCentersAlgorithm(_settings).BuildPartition();
            //var interpolators = muMatrices.Select(m => new MuValueInterpolator(settings.SpaceSettings, new MuGridValueGetter(m))).ToList();
            //return interpolators;

            var partitionTexture = _partitionFixedCentersComputer.Run();
            //var muGrids = _muConverter.ConvertMuGridsTexture(partitionTexture, settings);
            var muGrids = _muConverter.ConvertMuGridsTexture(partitionTexture, settings);
            var muValueInterpolators = muGrids.Select(v => new MuValueInterpolator(settings.SpaceSettings, v)).ToList();
            return muValueInterpolators;
        }

        public List<Vector<double>> Run()
        {
            _timer.Start();

            do
            {
                Trace.WriteLine($"Iteration number {_placingAlgorithm.PerformedIterationCount + 1}");

                var centers = _placingAlgorithm.GetCenters();
                SetCentersPositions(centers);

                var muGrids = GetMuGrids(_settings);

                _placingAlgorithm.DoIteration(muGrids);

            } while (!_placingAlgorithm.IsStopConditionSatisfied());

            _partitionFixedCentersComputer.Release();
            _timer.Stop();

            var t = TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds);
            var timeString = $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s:{t.Milliseconds:D3}ms";
            Debug.WriteLine($"Optimal placing partition global time: {timeString}");
            Debug.Flush();

            return _placingAlgorithm.GetCenters();
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