using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionPlacingCentersComputer : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;
        [SerializeField] private TextureToGridConverter _muConverter;
        [SerializeField] private Vector2[] _zeroTausManual;

        [SerializeField] private Vector2 _zeroTau;

        private Stopwatch _timer;
        private PartitionSettings _settings;

        public FuzzyPartitionPlacingCentersAlgorithm PlacingAlgorithm { get; private set; }

        public void Init(PartitionSettings settings)
        {
            _settings = settings;

            _timer = new Stopwatch();
            _timer.Start();

            _partitionFixedCentersComputer.Init(_settings);

            var zeroTaus = GetZeroIterationCentersPositions(settings);

            SetCentersPositions(zeroTaus);

            var muGrids = GetMuGrids(settings);

            PlacingAlgorithm = new FuzzyPartitionPlacingCentersAlgorithm(_settings, zeroTaus, muGrids);
            _timer.Stop();
        }

        private List<Vector<double>> GetZeroIterationCentersPositions(PartitionSettings settings)
        {
            var zeroTaus = new List<Vector<double>>();

            var p1 = settings.SpaceSettings.MinCorner;
            var p2 = settings.SpaceSettings.MaxCorner;//VectorUtils.CreateVector(p1[0], settings.SpaceSettings.MaxCorner[1]);

            for (var i = 0; i < settings.CentersSettings.CentersCount; i++)
            {
                //var zeroTau = settings.SpaceSettings.MinCorner.Clone();
                //
                var zeroTau = p1 + ((i + 1) / (settings.CentersSettings.CentersCount + 1d)) * (p2 - p1);
                //zeroTau += VectorUtils.CreateVector(Random.value - 0.5f, Random.value - 0.5f);
                //var zeroTau = VectorUtils.CreateVector(_zeroTau[0], _zeroTau[1]);
                zeroTaus.Add(zeroTau);
            }

            //zeroTaus = _zeroTausManual.Select(v => VectorUtils.CreateVector(v.x, v.y)).ToList();
            return zeroTaus;
        }

        private List<GridValueInterpolator> GetMuGrids(PartitionSettings settings)
        {
            //var muMatrices = new FuzzyPartitionFixedCentersAlgorithm(_settings).BuildPartition();
            //var interpolators = muMatrices.Select(m => new GridValueInterpolator(settings.SpaceSettings, new MatrixGridValueGetter(m))).ToList();
            //return interpolators;

            var partitionTexture = _partitionFixedCentersComputer.Run();
            var muGridValueInterpolators = _muConverter.GetGridValueInterpolators(partitionTexture, settings);
            return muGridValueInterpolators;
        }

        public List<Vector<double>> Run()
        {
            _timer.Start();

            //do
            while (!PlacingAlgorithm.IsStopConditionSatisfied())
            {
                Logger.Trace($"Iteration number {PlacingAlgorithm.PerformedIterationCount + 1}");

                var centers = PlacingAlgorithm.GetCenters();
                SetCentersPositions(centers);
                var muGrids = GetMuGrids(_settings);

                PlacingAlgorithm.DoIteration(muGrids);

            } //while (!PlacingAlgorithm.IsStopConditionSatisfied());

            _timer.Stop();

            var t = TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds);
            var timeString = $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s:{t.Milliseconds:D3}ms";
            Logger.Debug($"Optimal placing partition global time: {timeString}");

            return PlacingAlgorithm.GetCenters();
        }

        //public (List<Vector<double>> centers, bool finished) DoIteration()
        //{
        //    _timer.Start();

        //    Trace.WriteLine($"Iteration number {PlacingAlgorithm.PerformedIterationCount + 1}");

        //    var centers = PlacingAlgorithm.GetCenters();
        //    SetCentersPositions(centers);

        //    var muGrids = GetMuGrids(_settings);

        //    PlacingAlgorithm.DoIteration(muGrids);

        //    _timer.Stop();

        //    var newCenters = PlacingAlgorithm.GetCenters();

        //    if (PlacingAlgorithm.IsStopConditionSatisfied())
        //    {
        //        var t = TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds);
        //        var timeString = $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s:{t.Milliseconds:D3}ms";
        //        Debug.WriteLine($"Optimal placing partition global time: {timeString}");
        //        Debug.Flush();

        //        return (newCenters, true);
        //    }
        //    else
        //    {
        //        return (newCenters, false);
        //    }
        //}

        private void SetCentersPositions(IReadOnlyList<Vector<double>> centers)
        {
            for (var i = 0; i < _settings.CentersSettings.CentersCount; i++)
            {
                _settings.CentersSettings.CenterDatas[i].Position = centers[i];
                Logger.Trace($"Center #{i + 1} = ({centers[i][0]}; {centers[i][1]})");
            }
        }
    }
}