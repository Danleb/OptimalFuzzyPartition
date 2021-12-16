using FuzzyPartitionVisualizing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace PartitionView
{
    /// <summary>
    /// Shows the partition in the UI.
    /// </summary>
    public class PartitionShower : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private Image _image;
        [SerializeField] private FuzzyPartitionImageCreator _partitionImageCreator;
        [SerializeField] private CentersInfoShower _centersInfoShower;
        [SerializeField] private ColorsGenerator _colorsGenerator;
        [SerializeField] private RenderingSettings _renderingSettings;

        private RenderTexture _muRenderTexture;
        private PartitionSettings _partitionSettings;
        private Texture2D _partitionTexture;

        public RenderingSettings RenderingSettings
        {
            get => _renderingSettings;
            set => _renderingSettings = value;
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
            _partitionImageCreator.Init(_partitionSettings, _renderingSettings, _colorsGenerator.GetColors(_partitionSettings.CentersSettings.CentersCount));
            _centersInfoShower.Init(partitionSettings);

            return CreatePartitionAndShow();
        }

        [Button("Redraw partition image with current settings")]
        public Texture2D CreatePartitionAndShow()
        {
            Logger.Debug("Creating partition texture and showing it.");
            var renderTexture = _partitionImageCreator.CreatePartitionTexture(_muRenderTexture, _renderingSettings);

            if (_partitionTexture?.width != renderTexture.width || _partitionTexture?.height != renderTexture.height)
            {
                Logger.Debug("Texture2D allocation.");
                _partitionTexture = new Texture2D(renderTexture.width, renderTexture.height) { wrapMode = TextureWrapMode.Clamp };
            }

            RenderTexture.active = renderTexture;
            _partitionTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            _partitionTexture.Apply();
            var sprite = _partitionTexture.ToSprite();
            _image.sprite = sprite;

            _centersInfoShower.SetShowAlways(_renderingSettings.AlwaysShowCentersInfo);

            return _partitionTexture;
        }
    }
}
