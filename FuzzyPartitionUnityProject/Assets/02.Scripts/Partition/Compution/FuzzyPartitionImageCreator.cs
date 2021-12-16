using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using UnityEngine;
using Utils;

namespace FuzzyPartitionVisualizing
{
    /// <summary>
    /// Generates fuzzy partition 2d RenderTexture.
    /// </summary>
    public class FuzzyPartitionImageCreator : MonoBehaviour
    {
        [SerializeField] private ComputeShader _partitionDrawingShader;

        private Color[] _centersColors;
        private PartitionSettings _settings;
        private RenderingSettings _renderingSettings;

        private RenderTexture _partitionRenderTexture;
        private int _partitionDrawingKernel;
        private ComputeBuffer _colorsComputeBuffer;

        private const int ColorBufferStride = sizeof(float) * 4;
        private const string PartitionDrawingKernel = "DrawPartition";
        private readonly Vector3Int _shaderNumThreads = new Vector3Int(8, 8, 1);

        private void Awake()
        {
            PlayStateNotifier.OnPlaymodeExit += PlayStateNotifier_OnPlaymodeExit;
            _partitionDrawingKernel = _partitionDrawingShader.FindKernel(PartitionDrawingKernel);
        }

        private void PlayStateNotifier_OnPlaymodeExit()
        {
            PlayStateNotifier.OnPlaymodeExit -= PlayStateNotifier_OnPlaymodeExit;
            CheckAndReleaseBuffer();
        }

        public void CheckAndReleaseBuffer()
        {
            if (_colorsComputeBuffer != null && _colorsComputeBuffer.IsValid())
                _colorsComputeBuffer.Release();
        }

        public void Init(PartitionSettings partitionSettings, RenderingSettings renderingSettings, Color[] centersColors)
        {
            _settings = partitionSettings;
            _renderingSettings = renderingSettings;
            _centersColors = centersColors;

            var targetWidth = _settings.SpaceSettings.GridSize[0];
            var targetHeight = _settings.SpaceSettings.GridSize[1];

            if (_partitionRenderTexture?.width != targetWidth || _partitionRenderTexture?.height != targetHeight)
            {
                _partitionRenderTexture = new RenderTexture(targetWidth, targetHeight, 0)
                {
                    format = RenderTextureFormat.ARGB32,
                    enableRandomWrite = true
                };
                _partitionRenderTexture.Create();
            }

            if (_colorsComputeBuffer == null || _colorsComputeBuffer.count != centersColors.Length)
            {
                CheckAndReleaseBuffer();
                _colorsComputeBuffer = new ComputeBuffer(centersColors.Length, ColorBufferStride, ComputeBufferType.Default);
                _colorsComputeBuffer.SetData(_centersColors);
            }

            _partitionDrawingShader.SetInt("CentersCount", _settings.CentersSettings.CentersCount);
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "Result", _partitionRenderTexture);
            _partitionDrawingShader.SetBuffer(_partitionDrawingKernel, "CentersColors", _colorsComputeBuffer);
            _partitionDrawingShader.SetInts("ImageSize", _settings.SpaceSettings.GridSize[0], _settings.SpaceSettings.GridSize[1]);
        }

        public RenderTexture CreatePartitionTexture(ComputeBuffer muGrids, RenderingSettings renderingSettings)
        {
            _renderingSettings = renderingSettings;
            _partitionDrawingShader.SetInt("BorderWidth", _renderingSettings.BorderWidth);
            _partitionDrawingShader.SetBool("DrawGrayscale", _renderingSettings.DrawGrayscale);
            _partitionDrawingShader.SetFloat("MuThresholdValue", (float)_renderingSettings.MistrustCoefficient);
            _partitionDrawingShader.SetBool("DrawThresholdValue", _renderingSettings.DrawWithMistrustCoefficient);
            _partitionDrawingShader.SetBuffer(_partitionDrawingKernel, "MuGrids", muGrids);

            _partitionDrawingShader.Dispatch(_partitionDrawingKernel, _settings.SpaceSettings.GridSize[0] / _shaderNumThreads.x, _settings.SpaceSettings.GridSize[1] / _shaderNumThreads.y, 1);

            return _partitionRenderTexture;
        }
    }
}
