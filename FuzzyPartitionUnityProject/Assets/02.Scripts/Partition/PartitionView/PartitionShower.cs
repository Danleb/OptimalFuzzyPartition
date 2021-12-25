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

        [SerializeField] private Image _partitionImage;
        [SerializeField] private FuzzyPartitionImageCreator _partitionTextureCreator;
        [SerializeField] private CentersInfoShower _centersInfoShower;
        [SerializeField] private ColorsGenerator _colorsGenerator;
        [SerializeField] private RenderingSettings _renderingSettings;

        private ComputeBuffer _muGrids;
        private PartitionSettings _partitionSettings;
        private Texture2D _partitionTexture;

        public RenderingSettings RenderingSettings
        {
            get => _renderingSettings;
            set => _renderingSettings = value;
        }

        public void Show()
        {
            _partitionImage.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _partitionImage.gameObject.SetActive(false);
        }

        public Texture2D CreatePartitionAndShow(PartitionSettings partitionSettings, ComputeBuffer muGrids)
        {
            _muGrids = muGrids;
            _partitionSettings = partitionSettings;
            _partitionTextureCreator.Init(_partitionSettings, _renderingSettings, _colorsGenerator.GetColors(_partitionSettings.CentersSettings.CentersCount));
            _centersInfoShower.Init(_renderingSettings, partitionSettings, muGrids);

            return CreatePartitionAndShow();
        }

        [Button("Redraw partition image with current settings")]
        public Texture2D CreatePartitionAndShow()
        {
            Logger.Debug("Creating partition texture and showing it.");
            var renderTexture = _partitionTextureCreator.CreatePartitionTexture(_muGrids, _renderingSettings);

            if (_partitionTexture?.width != renderTexture.width || _partitionTexture?.height != renderTexture.height)
            {
                Logger.Debug("Texture2D allocation.");
                _partitionTexture = new Texture2D(renderTexture.width, renderTexture.height) { wrapMode = TextureWrapMode.Clamp };
            }

            RenderTexture.active = renderTexture;
            _partitionTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            _partitionTexture.Apply();
            var sprite = _partitionTexture.ToSprite();
            _partitionImage.sprite = sprite;

            _centersInfoShower.UpdateView(_renderingSettings);

            return _partitionTexture;
        }

        [Button("Draw test partition image")]
        public void DrawTestPartitionImage()
        {
            //var m
        }
    }
}
