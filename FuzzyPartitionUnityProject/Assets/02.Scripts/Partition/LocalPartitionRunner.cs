using FuzzyPartitionVisualizing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace FuzzyPartitionComputing
{
    public class LocalPartitionRunner : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionPlacingCentersComputer _fuzzyPartitionPlacingCentersComputer;
        [SerializeField] private FuzzyPartitionFixedCentersComputer _fuzzyPartitionFixedCentersComputer;
        [SerializeField] private FuzzyPartitionImageCreator _fuzzyPartitionDrawer;

        [SerializeField] private ColorsGenerator _colorsGenerator;

        [SerializeField] private Vector2 minCorner;
        [SerializeField] private Vector2 maxCorner;
        [SerializeField] private Vector2Int gridSize;

        [SerializeField] private Vector2[] Centers;
        [SerializeField] private double[] AdditiveCoefficients;
        [SerializeField] private double[] MultiplicativeCoefficients;

        [SerializeField] private double _fixedPartitionGradientStep;
        [SerializeField] private double _fixedPartitionGradientEpsilon;
        [SerializeField] private int _fixedPartitionMaxIterationsCount;

        [SerializeField] private bool autoStart;
        [SerializeField] private bool endlessComputing;

        [SerializeField] private CommandType commandType;

        private void Start()
        {
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
                CreateFuzzyPartitionWithFixedCenters();
        }

        public void CreateFuzzyWithPlacingCenters()
        {

        }

        [Button("Create fuzzy partition with fixed centers")]
        public void CreateFuzzyPartitionWithFixedCenters()
        {
            var partitionSettings = GetPartitionSettings();

            _fuzzyPartitionFixedCentersComputer.Init(partitionSettings);
            var muGridsRenderTexture = _fuzzyPartitionFixedCentersComputer.Run();

            _fuzzyPartitionDrawer.Init(partitionSettings, _colorsGenerator.GetColors(partitionSettings.CentersSettings.CentersCount));

            _fuzzyPartitionDrawer.CreatePartitionAndShow(null, muGridsRenderTexture);

            _fuzzyPartitionFixedCentersComputer.Release();
            _fuzzyPartitionDrawer.Release();
        }

        private PartitionSettings GetPartitionSettings()
        {
            var partitionSettings = new PartitionSettings
            {
                IsCenterPlacingTask = false,
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(minCorner.x, minCorner.y),
                    MaxCorner = VectorUtils.CreateVector(maxCorner.y, maxCorner.y),
                    GridSize = new List<int> { gridSize.x, gridSize.y },
                },
                CentersSettings = new CentersSettings
                {
                    CentersCount = Centers.Length,
                    CenterDatas = Centers.Select((v, i) => new CenterData
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
    }
}