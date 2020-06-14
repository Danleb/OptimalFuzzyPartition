using System.Collections.Generic;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CentersInfoShower : MonoBehaviour
    {
        [SerializeField] private CenterInfo _centerInfoPrefab;

        [SerializeField] private List<CenterInfo> _centerInfos;

        public void Init()
        {
            for (var index = 0; index < _centerInfos.Count; index++)
            {
                var centerInfo = _centerInfos[index];
                centerInfo.Init(index);
            }
        }

        public void EnableShowAlways()
        {
            foreach (var centerInfo in _centerInfos)
            {
                centerInfo.EnableAlwaysShow();
            }
        }

        public void DisableShowAlways()
        {
            foreach (var centerInfo in _centerInfos)
            {
                centerInfo.DisableAlwaysShow();
            }
        }
    }
}