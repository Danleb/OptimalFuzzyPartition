using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using OptimalFuzzyPartitionAlgorithm.Settings;
using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    public class CenterInfoView : MonoBehaviour
    {
        [SerializeField] private GameObject _centerInfoBlock;
        [SerializeField] private TMP_Text _textBlockSizeMaker;
        [SerializeField] private TMP_Text _textInfo;
        [SerializeField] private GameObject _gradientDirectionArrow;

        private RenderingSettings _renderingSettings;
        private Vector<double> _gradient;
        private CenterData _data;
        private int _centerIndex;

        public void Init(RenderingSettings renderingSettings, CenterData data, int centerIndex, Vector<double> gradient)
        {
            _renderingSettings = renderingSettings;
            _gradient = gradient;
            _data = data;
            _centerIndex = centerIndex;

            gameObject.SetActive(true);
            _centerInfoBlock.SetActive(false);

            UpdateView(renderingSettings);
        }

        public void UpdateView(RenderingSettings renderingSettings)
        {
            _renderingSettings = renderingSettings;
            UpdateTextInfo();
            UpateAlwaysShow();
            UpdateGradientArrow();
        }

        public void OnHover()
        {
            _centerInfoBlock.SetActive(true);
        }

        public void OnUnhover()
        {
            if (!_renderingSettings.AlwaysShowCentersInfo)
                _centerInfoBlock.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpateAlwaysShow()
        {
            _centerInfoBlock.SetActive(_renderingSettings.AlwaysShowCentersInfo);
        }

        private void UpdateGradientArrow()
        {
            _gradientDirectionArrow.SetActive(IsShowGradient());

            if (IsShowGradient())
            {
                _gradientDirectionArrow.SetActive(true);
                var z = Math.Atan2(_gradient[1], _gradient[0]);
                var rot = Quaternion.Euler(0, 0, Mathf.Rad2Deg * (float)z);
                _gradientDirectionArrow.transform.rotation = rot;
            }
        }

        private void UpdateTextInfo()
        {
            var positionFormat = "0." + Enumerable.Range(0, _renderingSettings.PositionDigitsCount).Select(_ => "0").Aggregate((x1, x2) => x1 + x2);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"№{_centerIndex + 1}");
            stringBuilder.Append(" (X; Y) = (");
            stringBuilder.Append(_data.Position[0].ToString(positionFormat));
            stringBuilder.Append(";");
            stringBuilder.Append(_data.Position[1].ToString(positionFormat));
            stringBuilder.AppendLine(") ");
            stringBuilder.AppendLine($"A = {_data.A}");
            stringBuilder.AppendLine($"W = {_data.W}");

            if (IsShowGradient())
            {
                stringBuilder.Append($"G=(");
                stringBuilder.Append(_gradient[0].ToString(positionFormat));
                stringBuilder.Append("; ");
                stringBuilder.Append(_gradient[1].ToString(positionFormat));
                stringBuilder.Append(") ");
            }

            _textBlockSizeMaker.text = stringBuilder.ToString();
            _textInfo.text = stringBuilder.ToString();
        }

        private bool IsShowGradient()
        {
            return _gradient != null && _renderingSettings.ShowGradientInfo;
        }
    }
}
