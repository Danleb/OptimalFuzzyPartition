using FuzzyPartitionVisualizing;
using OptimalFuzzyPartitionAlgorithm;
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

        private void Start()
        {
            if (autoStart)
                CreatePartition();
        }

        private void Update()
        {
            if (endlessComputing)
                CreatePartition();
        }

        public void CreatePartition()
        {
            var partitionSettings = new PartitionSettings
            {
                IsCenterPlacingTask = false,
                AdditiveCoefficients = AdditiveCoefficients.ToList(),
                MultiplicativeCoefficients = MultiplicativeCoefficients.ToList(),
                CenterPositions = Centers.Select(v => VectorUtils.CreateVector(v.x, v.y)).ToList(),
                CentersCount = Centers.Length,
                MinCorner = VectorUtils.CreateVector(minCorner.x, minCorner.y),
                MaxCorner = VectorUtils.CreateVector(maxCorner.y, maxCorner.y),
                GridSize = new List<int> { gridSize.x, gridSize.y },
                FixedPartitionGradientStep = _fixedPartitionGradientStep,
                FixedPartitionGradientEpsilon = _fixedPartitionGradientEpsilon,
                FixedPartitionMaxIterationsCount = _fixedPartitionMaxIterationsCount
            };

            _fuzzyPartitionFixedCentersComputer.Init(partitionSettings);
            var muGridsRenderTexture = _fuzzyPartitionFixedCentersComputer.Run();

            _fuzzyPartitionDrawer.Init(partitionSettings, _colorsGenerator.GetColors(partitionSettings.CentersCount));

            _fuzzyPartitionDrawer.CreatePartitionAndShow(null, muGridsRenderTexture);

            _fuzzyPartitionFixedCentersComputer.Release();
            _fuzzyPartitionDrawer.Release();
        }
    }
}