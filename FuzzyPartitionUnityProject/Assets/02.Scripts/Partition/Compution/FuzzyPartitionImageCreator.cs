using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using UnityEngine;
using Utils;

namespace FuzzyPartitionVisualizing
{
    /// <summary>
    /// Creates fuzzy partition 2d RenderTexture.
    /// </summary>
    public class FuzzyPartitionImageCreator : MonoBehaviour
    {
        [SerializeField] private ComputeShader _partitionDrawingShader;

        private PartitionSettings _settings;
        private RenderingSettings _renderingSettings;
        private Color[] _centersColors;

        private RenderTexture _partitionRenderTexture;
        private int _partitionDrawingKernel;
        private ComputeBuffer _colorsComputeBuffer;

        private const string PartitionDrawingKernel = "DrawPartition";
        private readonly Vector3Int _shaderNumThreads = new Vector3Int(8, 8, 1);

        private void Awake()
        {
            PlayStateNotifier.OnPlaymodeExit += PlayStateNotifier_OnPlaymodeExit;
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

            _partitionDrawingKernel = _partitionDrawingShader.FindKernel(PartitionDrawingKernel);

            _partitionRenderTexture = new RenderTexture(_settings.SpaceSettings.GridSize[0], _settings.SpaceSettings.GridSize[1], 0)
            {
                format = RenderTextureFormat.ARGB32,
                enableRandomWrite = true
            };
            _partitionRenderTexture.Create();

            CheckAndReleaseBuffer();
            _colorsComputeBuffer = new ComputeBuffer(centersColors.Length, sizeof(float) * 4, ComputeBufferType.Default);
            _colorsComputeBuffer.SetData(_centersColors);

            _partitionDrawingShader.SetInt("CentersCount", _settings.CentersSettings.CentersCount);
            _partitionDrawingShader.SetBuffer(_partitionDrawingKernel, "CentersColors", _colorsComputeBuffer);
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "Result", _partitionRenderTexture);
            _partitionDrawingShader.SetBool("DrawGrayscale", _renderingSettings.DrawGrayscale);
            _partitionDrawingShader.SetBool("DrawBorder", _renderingSettings.DrawGrayscale);
            const int borderWidth = 5;
            _partitionDrawingShader.SetInt("BorderWidth", borderWidth);
        }

        public RenderTexture CreatePartitionTexture(RenderTexture muRenderTexture, bool drawWithMuThreshold, float muThreshold = 0.35f)
        {
            _partitionDrawingShader.SetTexture(_partitionDrawingKernel, "MuGrids", muRenderTexture);
            _partitionDrawingShader.SetBool("DrawThresholdValue", drawWithMuThreshold);
            _partitionDrawingShader.SetFloat("MuThresholdValue", muThreshold);

            _partitionDrawingShader.Dispatch(_partitionDrawingKernel, _settings.SpaceSettings.GridSize[0] / _shaderNumThreads.x, _settings.SpaceSettings.GridSize[1] / _shaderNumThreads.y, 1);

            return _partitionRenderTexture;
        }
    }
}