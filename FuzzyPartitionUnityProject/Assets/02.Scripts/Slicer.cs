using UnityEngine;

namespace Assets._02.Scripts
{
    public class Slicer : MonoBehaviour//, ISlicer
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

        //public void Save()
        //{
        //    Texture3D export = new Texture3D(voxelSize, voxelSize, voxelSize, TextureFormat.ARGB32, false);
        //    RenderTexture selectedRenderTexture;

        //    if (useA)
        //        selectedRenderTexture = renderA;
        //    else
        //        selectedRenderTexture = renderB;

        //    RenderTexture[] layers = new RenderTexture[voxelSize];
        //    for (int i = 0; i < 64; i++)
        //        layers[i] = Copy3DSliceToRenderTexture(selectedRenderTexture, i);

        //    Texture2D[] finalSlices = new Texture2D[voxelSize];
        //    for (int i = 0; i < 64; i++)
        //        finalSlices[i] = ConvertFromRenderTexture(layers[i]);

        //    Texture3D output = new Texture3D(voxelSize, voxelSize, voxelSize, TextureFormat.ARGB32, true);
        //    output.filterMode = FilterMode.Trilinear;
        //    Color[] outputPixels = output.GetPixels();

        //    for (int k = 0; k < voxelSize; k++)
        //    {
        //        Color[] layerPixels = finalSlices[k].GetPixels();

        //        for (int i = 0; i < voxelSize; i++)
        //            for (int j = 0; j < voxelSize; j++)
        //            {
        //                outputPixels[i + j * voxelSize + k * voxelSize * voxelSize] = layerPixels[i + j * voxelSize];
        //            }
        //    }

        //    output.SetPixels(outputPixels);
        //    output.Apply();

        //    AssetDatabase.CreateAsset(output, "Assets/" + nameOfTheAsset + ".asset");
        //}
    }
}