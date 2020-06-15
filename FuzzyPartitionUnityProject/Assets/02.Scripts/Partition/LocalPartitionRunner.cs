using FuzzyPartitionVisualizing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Utils;
using Debug = System.Diagnostics.Debug;

namespace FuzzyPartitionComputing
{
    public class LocalPartitionRunner : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionPlacingCentersComputer _partitionPlacingCentersComputer;
        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;
        [SerializeField] private FuzzyPartitionImageCreator _partitionDrawer;
        [SerializeField] private CentersInfoShower _centersInfoShower;

        [Header("Space settings:")]
        [SerializeField] private RAlgorithmSettings rAlgorithmSettings;
        [SerializeField] private FuzzyPartitionPlacingCentersSettings fuzzyPartitionPlacingCentersSettings;

        [SerializeField] private ColorsGenerator _colorsGenerator;

        [SerializeField] private Vector2 minCorner;
        [SerializeField] private Vector2 maxCorner;
        [SerializeField] private Vector2Int gridSize;

        [Header("Centers settings:")]
        [SerializeField] private int centerCount;
        [SerializeField] private Vector2[] Centers;
        [SerializeField] private double[] AdditiveCoefficients;
        [SerializeField] private double[] MultiplicativeCoefficients;

        [SerializeField] private double _fixedPartitionGradientStep;
        [SerializeField] private double _fixedPartitionGradientEpsilon;
        [SerializeField] private int _fixedPartitionMaxIterationsCount;

        [SerializeField] private bool autoStart;
        [SerializeField] private bool endlessComputing;

        [SerializeField] private CommandType commandType;

        [SerializeField] private bool trace;
        [SerializeField] private bool debug;
        private readonly UnityConsoleTraceListener _unityConsoleListener = new UnityConsoleTraceListener();

        private void Start()
        {
            if (debug)
                Debug.Listeners.Add(_unityConsoleListener);
            if (trace)
            {
                Trace.Listeners.Add(_unityConsoleListener);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Logs.txt");
                if (File.Exists(path))
                    File.Delete(path);
                Trace.Listeners.Add(new TextWriterTraceListener(path));
                Trace.WriteLine("!!!");

            }

            Debug.WriteLine("Local partition runner init");

            if (autoStart)
            {
                if (commandType == CommandType.CreateFuzzyPartitionWithoutCentersPlacing)
                    CreateFuzzyPartitionWithFixedCenters();
                else if (commandType == CommandType.CreateFuzzyPartitionWithCentersPlacing)
                    CreateFuzzyWithPlacingCenters();
            }
        }

        private void Update()
        {
            if (endlessComputing)
            {
                CreateFuzzyPartitionWithFixedCenters();
            }
        }

        [Button("Create fuzzy partition with placing centers")]
        public void CreateFuzzyWithPlacingCenters()
        {
            var settings = GetPartitionSettings();

            _partitionPlacingCentersComputer.Init(settings);
            var centersPositions = _partitionPlacingCentersComputer.Run();

            for (var index = 0; index < centersPositions.Count; index++)
            {
                var centersPosition = centersPositions[index];
                Debug.WriteLine($"Tau #{index + 1} = ({centersPosition[0]}; {centersPosition[1]})");
                settings.CentersSettings.CenterDatas[index].Position = centersPosition;
            }

            CreateFuzzyPartitionWithFixedCenters(settings);
        }

        [Button("Create fuzzy partition with fixed centers")]
        public void CreateFuzzyPartitionWithFixedCenters()
        {
            var partitionSettings = GetPartitionSettings();
            CreateFuzzyPartitionWithFixedCenters(partitionSettings);
        }

        public void CreateFuzzyPartitionWithFixedCenters(PartitionSettings settings)
        {
            _partitionFixedCentersComputer.Init(settings);
            var muGridsRenderTexture = _partitionFixedCentersComputer.Run();

            _partitionDrawer.Init(settings, _colorsGenerator.GetColors(settings.CentersSettings.CentersCount));

            _partitionDrawer.CreatePartitionAndShow(muGridsRenderTexture);

            _centersInfoShower.Init(settings);

            _partitionFixedCentersComputer.Release();
            _partitionDrawer.Release();
        }

        private PartitionSettings GetPartitionSettings()
        {
            var partitionSettings = new PartitionSettings
            {
                RAlgorithmSettings = rAlgorithmSettings,
                FuzzyPartitionPlacingCentersSettings = fuzzyPartitionPlacingCentersSettings,
                IsCenterPlacingTask = false,
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(minCorner.x, minCorner.y),
                    MaxCorner = VectorUtils.CreateVector(maxCorner.y, maxCorner.y),
                    GridSize = new List<int> { gridSize.x, gridSize.y },
                },
                CentersSettings = new CentersSettings
                {
                    CentersCount = centerCount,
                    CenterDatas = Centers.Take(centerCount).Select((v, i) => new CenterData
                    {
                        Position = VectorUtils.CreateVector(Centers[i].x, Centers[i].y),
                        A = AdditiveCoefficients[i],
                        W = MultiplicativeCoefficients[i],
                        IsFixed = false
                    }).ToList()
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientStep = _fixedPartitionGradientStep,
                    GradientEpsilon = _fixedPartitionGradientEpsilon,
                    MaxIterationsCount = _fixedPartitionMaxIterationsCount
                }
            };
            return partitionSettings;
        }

        [Button]
        public void EnableTracing()
        {
            if (!Trace.Listeners.Contains(_unityConsoleListener))
                Trace.Listeners.Add(_unityConsoleListener);
        }

        [Button]
        public void DisableTracing()
        {
            Trace.Listeners.Remove(_unityConsoleListener);
        }
    }
}