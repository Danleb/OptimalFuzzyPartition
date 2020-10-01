using FuzzyPartitionComputing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using Partition.PartitionView;
using UnityEngine;

namespace Partition.Managing
{
    public class LocalCommandRunner : MonoBehaviour
    {
        [SerializeField] private PartitionSettingsHolder _partitionSettingsHolder;
        [SerializeField] private PartitionRunner _partitionRunner;
        [SerializeField] private RenderingSettings _renderingSettings;

        [Button("Create fuzzy partition with fixed centers")]
        public void CreateFuzzyPartitionWithFixedCenters()
        {
            _partitionRunner.CreateFuzzyPartitionWithFixedCenters(GetSettings(), _renderingSettings);
        }

        [Button("Create fuzzy partition with centers placing")]
        public void CreateFuzzyPartitionWithCentersPlacing()
        {
            _partitionRunner.CreateFuzzyPartitionWithCentersPlacing(GetSettings(), _renderingSettings);
        }

        [Button("Redraw partition with current settings")]
        public void RedrawPartitionWithSettings()
        {
            _partitionRunner.RedrawPartitionWithSettings(_renderingSettings);
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