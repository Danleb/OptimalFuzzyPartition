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

        public RenderTexture Copy3DSliceToRenderTexture(RenderTexture sourceRenderTexture, int layer)
        {
            RenderTexture targetRenderTexture = new RenderTexture(sourceRenderTexture.width, sourceRenderTexture.height, 0, sourceRenderTexture.graphicsFormat, 0);
            targetRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            targetRenderTexture.enableRandomWrite = true;
            targetRenderTexture.wrapMode = TextureWrapMode.Clamp;
            targetRenderTexture.Create();

            int kernelIndex = _slicerShader.FindKernel("CSMain");
            _slicerShader.SetTexture(kernelIndex, "voxels", sourceRenderTexture);
            _slicerShader.SetInt("layer", layer);
            _slicerShader.SetTexture(kernelIndex, "Result", targetRenderTexture);
            _slicerShader.Dispatch(kernelIndex, sourceRenderTexture.width / _slicerNumThreads.x, sourceRenderTexture.height / _slicerNumThreads.y, 1);

            return targetRenderTexture;
        }
    }
}