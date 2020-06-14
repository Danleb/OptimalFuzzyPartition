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

        private const string PartitionDrawingKernel = "DrawPartition";

        private PartitionSettings _settings;
        private Color[] _centersColors;

        private RenderTexture _partitionRenderTexture;
        private int _partitionDrawingKernel;
        private ComputeBuffer _colorsComputeBuffer;

        private readonly Vector3Int ShaderNumThreads = new Vector3Int(8, 8, 1);

        public void Init(PartitionSettings partitionSettings, Color[] centersColors)
        {
            _settings = partitionSettings;
            _centersColors = centersColors;

            _partitionDrawingKernel = _partitionDrawingShader.FindKernel(PartitionDrawingKernel);

            _partitionRenderTexture = new RenderTexture(_settings.SpaceSettings.GridSize[0], _settings.SpaceSettings.GridSize[1], 0)
            {
                format = RenderTextureFormat.ARGB32,
                enableRandomWrite = true
            };
            _partitionRenderTexture.Create();

            _colorsComputeBuffer = new ComputeBuffer(centersColors.Length, sizeof(float) * 4, ComputeBufferType.Default);
            _colorsComputeBuffer.SetData(_centersColors);
        }

        public Texture2D CreatePartitionAndShow(IterationData iterationData, RenderTexture muRenderTexture)
        {
            var renderTexture = CreatePartitionTexture(iterationData, muRenderTexture);

            var partitionTexture2D = renderTexture.ToTexture2D();

            var sprite = partitionTexture2D.ToSprite();
            _outputImage.sprite = sprite;

            return partitionTexture2D;
        }

        public RenderTexture CreatePartitionTexture(IterationData iterationData, RenderTexture muRenderTexture)
        {
            _partitionDrawingShader.SetInt("CentersCount", _settings.CentersSettings.CentersCount);
            _partitionDrawingShader.SetBuffer(_partitionDrawingKernel, "CentersColors", _colorsComputeBuffer);
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "MuGrids", muRenderTexture);
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "Result", _partitionRenderTexture);

            _partitionDrawingShader.Dispatch(_partitionDrawingKernel, _settings.SpaceSettings.GridSize[0] / ShaderNumThreads.x, _settings.SpaceSettings.GridSize[1] / ShaderNumThreads.y, 1);

            return _partitionRenderTexture;
        }

        public void Hide()
        {
            _outputImage.gameObject.SetActive(false);
        }

        public void Release()
        {
            _colorsComputeBuffer.Release();
        }
    }
}