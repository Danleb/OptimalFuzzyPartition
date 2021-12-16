using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Partition.PartitionView
{
    public class PartitionSettingsHolder : MonoBehaviour
    {
        [SerializeField] private bool _isCentersPlacingTask;

        [Header("Space settings")]
        [SerializeField] private Vector2 _minCorner;
        [SerializeField] private Vector2 _maxCorner;
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private DensityType _densityType;
        [SerializeField] private MetricsType _metricsType;

        [Header("Centers settings")]
        [SerializeField] private int _centersCount;
        [SerializeField] private Vector2[] _centerPositions;
        [SerializeField] private CenterData[] _centerDatas;

        [Header("R-algorithm settings")]
        [SerializeField] private RAlgorithmSettings _rAlgorithmSettings;

        [SerializeField] private FuzzyPartitionFixedCentersSettings _fuzzyPartitionFixedCentersSettings;
        [SerializeField] private FuzzyPartitionPlacingCentersSettings _fuzzyPartitionPlacingCentersSettings;

        [SerializeField] private int _gaussLegendreIntegralOrder;

        public PartitionSettings GetPartitionSettings()
        {
            if (_centersCount > _centerPositions.Length)
                throw new Exception($"Centers count ({_centersCount}) is bigger then center positions count ({_centerPositions.Length}).");

            if (_centersCount > _centerDatas.Length)
                throw new Exception($"Centers count ({_centersCount}) is bigger then center datas count ({_centerDatas.Length}).");

            var settings = new PartitionSettings
            {
                IsCenterPlacingTask = _isCentersPlacingTask,
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = _minCorner.ToVector(),
                    MaxCorner = _maxCorner.ToVector(),
                    GridSize = new List<int> { _gridSize.x, _gridSize.y },
                    DensityType = _densityType,
                    MetricsType = _metricsType
                },
                CentersSettings = new CentersSettings
                {
                    CenterDatas = _centerDatas
                        .Take(_centersCount)
                        .Select((v, i) =>
                        {
                            v.Position = _centerPositions[i].ToVector();
                            return v;
                        }).ToList()
                },
                RAlgorithmSettings = _rAlgorithmSettings,
                FuzzyPartitionFixedCentersSettings = _fuzzyPartitionFixedCentersSettings,
                FuzzyPartitionPlacingCentersSettings = _fuzzyPartitionPlacingCentersSettings,
                GaussLegendreIntegralOrder = _gaussLegendreIntegralOrder
            };

            return settings;
        }
    }
}
