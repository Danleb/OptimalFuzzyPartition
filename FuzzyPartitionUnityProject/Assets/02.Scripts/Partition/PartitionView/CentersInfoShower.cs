using FuzzyPartitionComputing;
using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CentersInfoShower : MonoBehaviour
    {
        [SerializeField] private CenterInfoView _centerInfoPrefab;
        [SerializeField] private GameObject _minCorner;
        [SerializeField] private GameObject _maxCorner;
        [SerializeField] private List<CenterInfoView> _centerInfos;

        private PartitionSettings _partitionSettings;
        private List<Vector<double>> _gradients = null;

        public void Init(RenderingSettings renderingSettings, PartitionSettings partitionSettings, ComputeBuffer muGrids = null)
        {
            _partitionSettings = partitionSettings;

            foreach (var centerInfo in _centerInfos)
                centerInfo.Hide();

            while (_centerInfos.Count < partitionSettings.CentersSettings.CentersCount)
            {
                _centerInfos.Add(Instantiate(_centerInfoPrefab, transform, true));
            }

            if (muGrids != null)
            {
                _gradients = new List<Vector<double>>();
                var muValueInterpolators = ComputeBufferToGridConverter.GetGridValueInterpolators(muGrids, partitionSettings);

                for (var i = 0; i < partitionSettings.CentersSettings.CenterDatas.Count; i++)
                {
                    var centerData = partitionSettings.CentersSettings.CenterDatas[i];
                    var gradientCalculator = new GradientCalculator(partitionSettings.SpaceSettings, partitionSettings.GaussLegendreIntegralOrder);
                    var gradient = gradientCalculator.CalculateGradientForCenter(centerData.Position, muValueInterpolators[i]);
                    _gradients.Add(gradient);
                }
            }

            for (var i = 0; i < partitionSettings.CentersSettings.CentersCount; i++)
            {
                var centerData = partitionSettings.CentersSettings.CenterDatas[i];
                var centerInfoView = _centerInfos[i];
                var gradient = _gradients?[i];
                centerInfoView.Init(renderingSettings, centerData, i, gradient);
            }

            SetCenterInfoPositions();
        }

        public void UpdateView(RenderingSettings renderingSettings)
        {
            foreach (var centerInfo in _centerInfos)
            {
                centerInfo.UpdateView(renderingSettings);
            }
        }

        private void Update()
        {
            SetCenterInfoPositions();
        }

        private void SetCenterInfoPositions()
        {
            if (_partitionSettings == null) return;

            for (int i = 0; i < _partitionSettings.CentersSettings.CentersCount; i++)
            {
                var centerInfo = _centerInfos[i];
                var data = _partitionSettings.CentersSettings.CenterDatas[i];
                var p = data.Position;
                var ratio = (p - _partitionSettings.SpaceSettings.MinCorner) /
                            (_partitionSettings.SpaceSettings.MaxCorner - _partitionSettings.SpaceSettings.MinCorner);
                var ratioV = new Vector3((float)ratio[0], (float)ratio[1], 0);
                ratioV.Scale(_maxCorner.transform.position - _minCorner.transform.position);
                centerInfo.gameObject.transform.position = _minCorner.transform.position + ratioV;
            }
        }
    }
}
