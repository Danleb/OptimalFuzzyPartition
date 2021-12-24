using FuzzyPartitionComputing;
using MathNet.Numerics.LinearAlgebra;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation;
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

        public void Init(PartitionSettings partitionSettings, ComputeBuffer muGrids = null)
        {
            _partitionSettings = partitionSettings;

            foreach (var centerInfo in _centerInfos)
                centerInfo.Hide();

            while (_centerInfos.Count < partitionSettings.CentersSettings.CentersCount)
            {
                _centerInfos.Add(Instantiate(_centerInfoPrefab, transform, true));
            }

            List<Vector<double>> gradients  = null;

            if (muGrids != null)
            {
                gradients = new List<Vector<double>>();
                var muValueInterpolators = ComputeBufferToGridConverter.GetGridValueInterpolators(muGrids, partitionSettings);

                for (var i = 0; i < partitionSettings.CentersSettings.CenterDatas.Count; i++)
                {
                    var centerData = partitionSettings.CentersSettings.CenterDatas[i];
                    var gradientCalculator = new GradientCalculator(partitionSettings.SpaceSettings, partitionSettings.GaussLegendreIntegralOrder);
                    var gradient = gradientCalculator.CalculateGradientForCenter(centerData.Position, muValueInterpolators[i]);
                    gradients.Add(gradient);
                }
            }

            for (var i = 0; i < partitionSettings.CentersSettings.CentersCount; i++)
            {
                var centerData = partitionSettings.CentersSettings.CenterDatas[i];
                var centerInfoView = _centerInfos[i];
                var gradient = gradients?[i];
                centerInfoView.Init(centerData, i, gradient);
            }

            SetCenterInfoPositions();
        }

        [Button]
        public void ShowAll()
        {
            _centerInfos.ForEach(v => v.Show());
        }

        [Button]
        public void HideAll()
        {
            _centerInfos.ForEach(v => v.Hide());
        }

        [Button]
        public void EnableShowAlways()
        {
            SetShowAlways(true);
        }

        [Button]
        public void DisableShowAlways()
        {
            SetShowAlways(false);
        }

        public void SetShowAlways(bool isAlwaysShowCentersInfo)
        {
            foreach (var centerInfo in _centerInfos)
            {
                centerInfo.SetAlwaysShow(isAlwaysShowCentersInfo);
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
