using FuzzyPartitionComputing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using Partition.PartitionView;
using UnityEngine;

namespace Partition.Managing
{
    public class LocalCommandRunner : MonoBehaviour
    {
        [SerializeField] private PartitionSettingsHolder _partitionSettingsHolder;
        [SerializeField] private PartitionRunner _partitionRunner;
        [SerializeField] private bool _drawMistrustRate;
        [SerializeField] private double _mistrustRate;
        [SerializeField] private bool _alwaysShowCentersInfo;

        [Button("Create fuzzy partition with fixed centers")]
        public void CreateFuzzyPartitionWithFixedCenters()
        {
            _partitionRunner.CreateFuzzyPartitionWithFixedCenters(GetSettings(), _drawMistrustRate, _mistrustRate, _alwaysShowCentersInfo);
        }

        [Button("Create fuzzy partition with centers placing")]
        public void CreateFuzzyPartitionWithCentersPlacing()
        {
            _partitionRunner.CreateFuzzyPartitionWithCentersPlacing(GetSettings(), _drawMistrustRate, _mistrustRate, _alwaysShowCentersInfo);
        }

        [Button("Redraw partition with current settings")]
        public void RedrawPartitionWithSettings()
        {
            _partitionRunner.RedrawPartitionWithSettings(_drawMistrustRate, _mistrustRate);
        }

        [Button]
        public void SavePartitionImage(string path = null)
        {
            _partitionRunner.SavePartitionImage(path);
        }

        private PartitionSettings GetSettings()
        {
            return _partitionSettingsHolder.GetPartitionSettings();
        }
    }
}