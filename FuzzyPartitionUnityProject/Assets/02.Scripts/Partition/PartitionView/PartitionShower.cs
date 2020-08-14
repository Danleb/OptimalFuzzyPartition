﻿using FuzzyPartitionVisualizing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace PartitionView
{
    public class PartitionShower : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private FuzzyPartitionImageCreator _partitionImageCreator;
        [SerializeField] private CentersInfoShower _centersInfoShower;
        [SerializeField] private ColorsGenerator _colorsGenerator;
        [SerializeField] public bool _drawWithMistrustRate;
        [SerializeField] public float _mistrustRateValue;
        [SerializeField] public bool _alwaysShowCentersInfo;

        private RenderTexture _muRenderTexture;
        private PartitionSettings _partitionSettings;

        public bool AlwaysShowCentersInfo
        {
            get => _alwaysShowCentersInfo;
            set => _alwaysShowCentersInfo = value;
        }

        public bool DrawWithMistrustRate
        {
            get => _drawWithMistrustRate;
            set => _drawWithMistrustRate = value;
        }

        public float MistrustRate
        {
            get => _mistrustRateValue;
            set => _mistrustRateValue = value;
        }

        public void Show()
        {
            _image.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _image.gameObject.SetActive(false);
        }

        public Texture2D CreatePartitionAndShow(PartitionSettings partitionSettings, RenderTexture muRenderTexture)
        {
            _muRenderTexture = muRenderTexture;
            _partitionSettings = partitionSettings;
            _partitionImageCreator.Init(_partitionSettings, _colorsGenerator.GetColors(_partitionSettings.CentersSettings.CentersCount));
            _centersInfoShower.Init(partitionSettings);

            return CreatePartitionAndShow();
        }

        [Button("Redraw partition image with current settings")]
        public Texture2D CreatePartitionAndShow()
        {
            var renderTexture = _partitionImageCreator.CreatePartitionTexture(_muRenderTexture, DrawWithMistrustRate, MistrustRate);
            var partitionTexture2D = new Texture2D(renderTexture.width, renderTexture.height) { wrapMode = TextureWrapMode.Clamp };
            RenderTexture.active = renderTexture;
            partitionTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            partitionTexture2D.Apply();
            var sprite = partitionTexture2D.ToSprite();
            _image.sprite = sprite;

            if (AlwaysShowCentersInfo)
                _centersInfoShower.EnableShowAlways();
            else
                _centersInfoShower.DisableShowAlways();

            return partitionTexture2D;
        }
    }
}