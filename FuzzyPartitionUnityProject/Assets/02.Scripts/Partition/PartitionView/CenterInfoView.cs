using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm.Settings;
using System;
using TMPro;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CenterInfoView : MonoBehaviour
    {
        [SerializeField] private GameObject _centerInfoBlock;
        [SerializeField] private TMP_Text _textInfo;
        [SerializeField] private bool _isAlwaysShow;
        [SerializeField] private GameObject _gradientDirectionArrow;

        public void Init(CenterData data, int centerIndex, Vector<double> gradient)
        {
            _textInfo.text = $"№{centerIndex + 1}\n(X; Y) = ({data.Position[0]:0.000000}; {data.Position[1]:0.000000})\nA = {data.A}\nW = {data.W}";
            gameObject.SetActive(true);
            _centerInfoBlock.SetActive(false);

            if (gradient == null)
            {
                _gradientDirectionArrow.SetActive(false);
            }
            else
            {
                _gradientDirectionArrow.SetActive(true);
                var z = Math.Atan2(gradient[1], gradient[0]);
                var rot = Quaternion.Euler(0, 0, Mathf.Rad2Deg * (float)z);
                _gradientDirectionArrow.transform.rotation = rot;
                _textInfo.text += $"\nGr({gradient[0]:0.000000}; {gradient[1]:0.000000})";
            }
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
