using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Slice a 3D RenderTexture to get one 2D RenderTexture at specified index.
    /// Requires ComputeShader to be set.
    /// </summary>
    public class Slicer : MonoBehaviour
    {
        [SerializeField] private ComputeShader _slicerShader;

        public RenderTexture Copy3DSliceToRenderTexture(RenderTexture sourceRenderTexture, int layer)
        {
            RenderTexture targetRenderTexture = new RenderTexture(sourceRenderTexture.width, sourceRenderTexture.height, 0, RenderTextureFormat.ARGB32);
            targetRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            targetRenderTexture.enableRandomWrite = true;
            targetRenderTexture.wrapMode = TextureWrapMode.Clamp;
            targetRenderTexture.Create();

            int kernelIndex = _slicerShader.FindKernel("CSMain");
            _slicerShader.SetTexture(kernelIndex, "voxels", sourceRenderTexture);
            _slicerShader.SetInt("layer", layer);
            _slicerShader.SetTexture(kernelIndex, "Result", targetRenderTexture);
            _slicerShader.Dispatch(kernelIndex, sourceRenderTexture.width / 32, sourceRenderTexture.height / 32, 1);

            return targetRenderTexture;
        }
    }
}