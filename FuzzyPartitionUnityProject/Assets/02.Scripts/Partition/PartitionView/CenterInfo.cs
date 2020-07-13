using OptimalFuzzyPartitionAlgorithm;
using TMPro;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CenterInfo : MonoBehaviour
    {
        [SerializeField] private GameObject _centerInfoBlock;
        [SerializeField] private TMP_Text TextInfo;
        [SerializeField] private bool _alwaysShow;

        public void Init(PartitionSettings partitionSettings, int centerIndex)
        {
            var data = partitionSettings.CentersSettings.CenterDatas[centerIndex];
            TextInfo.text = $"№{centerIndex + 1}\n(X; Y) = ({data.Position[0]:0.00}; {data.Position[1]:0.00})\nA = {data.A}\nW = {data.W}";
            gameObject.SetActive(true);
            _centerInfoBlock.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void OnHover()
        {
            _centerInfoBlock.SetActive(true);
        }

        public void OnUnhover()
        {
            if (!_alwaysShow)
                _centerInfoBlock.SetActive(false);
        }

        public void EnableAlwaysShow()
        {
            _alwaysShow = true;
            _centerInfoBlock.SetActive(true);
        }

        public void DisableAlwaysShow()
        {
            _alwaysShow = false;
            _centerInfoBlock.SetActive(false);
        }
    }
}