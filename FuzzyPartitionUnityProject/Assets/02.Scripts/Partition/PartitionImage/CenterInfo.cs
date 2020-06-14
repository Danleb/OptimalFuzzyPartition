using TMPro;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CenterInfo : MonoBehaviour
    {
        [SerializeField] private GameObject CenterInfoBlock;
        [SerializeField] private TMP_Text TextCenterNumber;
        [SerializeField] private TMP_Text TextCoordinates;

        [SerializeField] private bool _alwaysShow;

        public void Init(int centerIndex)
        {
            TextCenterNumber.text = (centerIndex + 1).ToString();
        }

        public void OnHover()
        {
            CenterInfoBlock.SetActive(true);
        }

        public void OnUnhover()
        {
            CenterInfoBlock.SetActive(false);
        }

        public void EnableAlwaysShow()
        {
            _alwaysShow = true;
        }

        public void DisableAlwaysShow()
        {
            _alwaysShow = false;
        }
    }
}