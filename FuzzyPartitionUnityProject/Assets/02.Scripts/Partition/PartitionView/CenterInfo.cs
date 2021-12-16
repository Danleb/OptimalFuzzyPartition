using OptimalFuzzyPartitionAlgorithm;
using TMPro;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CenterInfo : MonoBehaviour
    {
        [SerializeField] private GameObject _centerInfoBlock;
        [SerializeField] private TMP_Text _textInfo;
        [SerializeField] private bool _isAlwaysShow;

        public void Init(PartitionSettings partitionSettings, int centerIndex)
        {
            var data = partitionSettings.CentersSettings.CenterDatas[centerIndex];
            _textInfo.text = $"№{centerIndex + 1}\n(X; Y) = ({data.Position[0]:0.00}; {data.Position[1]:0.00})\nA = {data.A}\nW = {data.W}";
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
            if (!_isAlwaysShow)
                _centerInfoBlock.SetActive(false);
        }

        public void SetAlwaysShow(bool isAlwaysShow)
        {
            _isAlwaysShow = isAlwaysShow;
            _centerInfoBlock.SetActive(isAlwaysShow);
        }
    }
}
