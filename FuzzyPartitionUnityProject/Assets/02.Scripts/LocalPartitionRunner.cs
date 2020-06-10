using FuzzyPartitionComputing;
using FuzzyPartitionVisualizing;
using UnityEngine;

namespace Assets._02.Scripts
{
    public class LocalPartitionRunner : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionPlacingCentersComputer _fuzzyPartitionPlacingCentersComputer;
        [SerializeField] private FuzzyPartitionFixedCentersComputer _fuzzyPartitionFixedCentersComputer;
        [SerializeField] private FuzzyPartitionDrawer _fuzzyPartitionDrawer;

        private void Start()
        {

        }
    }
}