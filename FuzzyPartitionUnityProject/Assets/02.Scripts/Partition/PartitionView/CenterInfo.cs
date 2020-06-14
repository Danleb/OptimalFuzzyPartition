using OptimalFuzzyPartitionAlgorithm;
using TMPro;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CenterInfo : MonoBehaviour
    {
        [SerializeField] private GameObject CenterInfoBlock;
        [SerializeField] private TMP_Text TextInfo;

        [SerializeField] private bool _alwaysShow;

        public void Init(PartitionSettings partitionSettings, int centerIndex)
        {
            var data = partitionSettings.CentersSettings.CenterDatas[centerIndex];
            TextInfo.text = $"№{centerIndex + 1}\n(X; Y) = ({data.Position[0].ToString("0.00")}; {data.Position[1].ToString("0.00")})\nA = {data.A}\n W = {data.W}";
            gameObject.SetActive(true);
            CenterInfoBlock.SetActive(false);
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
            CenterInfoBlock.SetActive(true);
        }

        public void OnUnhover()
        {
            if (!_alwaysShow)
                CenterInfoBlock.SetActive(false);
        }

        public void EnableAlwaysShow()
        {
            _alwaysShow = true;
            CenterInfoBlock.SetActive(true);
        }

        public void DisableAlwaysShow()
        {
            _alwaysShow = false;
            CenterInfoBlock.SetActive(false);
        }
    }
}