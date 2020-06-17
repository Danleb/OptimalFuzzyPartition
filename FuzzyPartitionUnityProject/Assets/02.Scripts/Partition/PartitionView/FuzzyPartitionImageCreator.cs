using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace FuzzyPartitionVisualizing
{
    /// <summary>
    /// Creates fuzzy partition 2d image and shows it in UI.
    /// </summary>
    public class FuzzyPartitionImageCreator : MonoBehaviour
    {
        [SerializeField] private Image _outputImage;
        [SerializeField] private ComputeShader _partitionDrawingShader;

        [SerializeField] private bool DrawThresholdValue;
        [SerializeField] private float MuThresholdValue;
        [SerializeField] private bool DrawOnlyMaxValue;

        private PartitionSettings _settings;
        private Color[] _centersColors;

        private RenderTexture _partitionRenderTexture;
        private int _partitionDrawingKernel;
        private ComputeBuffer _colorsComputeBuffer;

        private const string PartitionDrawingKernel = "DrawPartition";
        private readonly Vector3Int ShaderNumThreads = new Vector3Int(8, 8, 1);
        private RenderTexture _muRenderTexture;

        public void Init(PartitionSettings partitionSettings, Color[] centersColors)
        {
            _settings = partitionSettings;
            _centersColors = centersColors;            

            _partitionRenderTexture = new RenderTexture(_settings.SpaceSettings.GridSize[0], _settings.SpaceSettings.GridSize[1], 0)
            {
                format = RenderTextureFormat.ARGB32,
                enableRandomWrite = true
            };
            _partitionRenderTexture.Create();

            if(_colorsComputeBuffer != null && _colorsComputeBuffer.IsValid())
                _colorsComputeBuffer.Release();

            _colorsComputeBuffer = new ComputeBuffer(centersColors.Length, sizeof(float) * 4, ComputeBufferType.Default);
            _colorsComputeBuffer.SetData(_centersColors);
        }

        public Texture2D CreatePartitionAndShow(RenderTexture muRenderTexture)
        {
            _muRenderTexture = muRenderTexture;
            return CreatePartitionAndShow();
        }

        private Texture2D CreatePartitionAndShow()
        {
            var renderTexture = CreatePartitionTexture(_muRenderTexture);

            var partitionTexture2D = new Texture2D(renderTexture.width, renderTexture.height);
            partitionTexture2D.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = renderTexture;
            partitionTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            partitionTexture2D.Apply();

            var sprite = partitionTexture2D.ToSprite();
            _outputImage.sprite = sprite;

            return partitionTexture2D;
        }

        public RenderTexture CreatePartitionTexture(RenderTexture muRenderTexture)
        {
            _partitionDrawingShader.SetInt("CentersCount", _settings.CentersSettings.CentersCount);
            _partitionDrawingShader.SetBuffer(_partitionDrawingKernel, "CentersColors", _colorsComputeBuffer);
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "MuGrids", muRenderTexture);
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "Result", _partitionRenderTexture);
            _partitionDrawingShader.SetBool("DrawThresholdValue", DrawThresholdValue);
            _partitionDrawingShader.SetFloat("MuThresholdValue", MuThresholdValue);
            _partitionDrawingShader.SetBool("DrawOnlyMaxValue", DrawOnlyMaxValue);

            _partitionDrawingShader.Dispatch(_partitionDrawingKernel, _settings.SpaceSettings.GridSize[0] / ShaderNumThreads.x, _settings.SpaceSettings.GridSize[1] / ShaderNumThreads.y, 1);

            return _partitionRenderTexture;
        }

        [Button("Redraw image with current settings")]
        public void ReDrawWithCurrentSettings()
        {
            CreatePartitionAndShow();
        }

        public void Hide()
        {
            _outputImage.gameObject.SetActive(false);
        }

        public void Release()
        {
            //_colorsComputeBuffer.Release();
        }
    }
}