using OptimalFuzzyPartitionAlgorithm;
using UnityEngine;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionPlacingCentersComputer : MonoBehaviour
    {
        private PartitionSettings _partitionSettings;

        public void Run(PartitionSettings partitionSettings)
        {
            _partitionSettings = partitionSettings;


        }
    }
}