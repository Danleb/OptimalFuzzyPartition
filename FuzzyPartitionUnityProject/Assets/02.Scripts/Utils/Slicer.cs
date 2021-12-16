using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Slice a 3D RenderTexture to get one 2D RenderTexture at specified index.
    /// Requires ComputeShader to be set.
    /// </summary>
    public class Slicer : MonoBehaviour
    {
        [SerializeField] private Vector2Int _slicerNumThreads;
        [SerializeField] private ComputeShader _slicerShader;

        private int _kernelIndex;
        private RenderTexture _tempRenderTexture;

        private void Awake()
        {
            _kernelIndex = _slicerShader.FindKernel("CSMain");
        }

        public RenderTexture Copy3DSliceToRenderTexture(RenderTexture sourceRenderTexture, int layer)
        {
            if (_tempRenderTexture?.width != sourceRenderTexture.width || _tempRenderTexture?.height != sourceRenderTexture.height)
            {
                if (_tempRenderTexture != null)
                {
                    _tempRenderTexture.Release();
                    _tempRenderTexture = null;
                }

                _tempRenderTexture = new RenderTexture(sourceRenderTexture.width, sourceRenderTexture.height, 0, sourceRenderTexture.graphicsFormat, 0);
                _tempRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
                _tempRenderTexture.enableRandomWrite = true;
                _tempRenderTexture.wrapMode = TextureWrapMode.Clamp;
                _tempRenderTexture.Create();
            }

            _slicerShader.SetTexture(_kernelIndex, "voxels", sourceRenderTexture);
            _slicerShader.SetInt("layer", layer);
            _slicerShader.SetTexture(_kernelIndex, "Result", _tempRenderTexture);
            _slicerShader.Dispatch(_kernelIndex, sourceRenderTexture.width / _slicerNumThreads.x, sourceRenderTexture.height / _slicerNumThreads.y, 1);

            return _tempRenderTexture;
        }
    }
}
