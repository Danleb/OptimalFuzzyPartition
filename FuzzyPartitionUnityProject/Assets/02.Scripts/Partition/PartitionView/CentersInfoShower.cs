using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CentersInfoShower : MonoBehaviour
    {
        [SerializeField] private CenterInfo _centerInfoPrefab;
        [SerializeField] private GameObject _minCorner;
        [SerializeField] private GameObject _maxCorner;
        [SerializeField] private List<CenterInfo> _centerInfos;

        private PartitionSettings _partitionSettings;

        public void Init(PartitionSettings partitionSettings)
        {
            _partitionSettings = partitionSettings;

            foreach (var centerInfo in _centerInfos)
                centerInfo.Hide();

            while (_centerInfos.Count < partitionSettings.CentersSettings.CentersCount)
            {
                _centerInfos.Add(Instantiate(_centerInfoPrefab, transform, true));
            }

            for (var index = 0; index < partitionSettings.CentersSettings.CentersCount; index++)
            {
                var centerInfo = _centerInfos[index];
                centerInfo.Init(partitionSettings, index);
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
