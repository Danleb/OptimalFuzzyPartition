using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace FuzzyPartitionComputing
{
    /// <summary>
    /// Converts Grids 3D RenderTexture to the list of matrices for each center.
    /// </summary>
    public class TextureToGridConverter : MonoBehaviour
    {
        [SerializeField] private Slicer _slicer;

        public GridValueInterpolator GetGridValueInterpolator(RenderTexture renderTexture2d, PartitionSettings partitionSettings)
        {
            var gridSizeX = partitionSettings.SpaceSettings.GridSize[0];
            var textureArray = GetTextureNativeArray(renderTexture2d);
            var interpolator = new GridValueInterpolator(partitionSettings.SpaceSettings, new GridValueTextureCalculator(textureArray, gridSizeX));
            return interpolator;
        }

        public List<GridValueInterpolator> GetGridValueInterpolators(RenderTexture gridsRenderTexture, PartitionSettings partitionSettings)
        {
            var gridValueGetters = GetGridCellsGetters(gridsRenderTexture, partitionSettings);
            var interpolators = gridValueGetters.Select(v => new GridValueInterpolator(partitionSettings.SpaceSettings, v)).ToList();
            return interpolators;
        }

        public List<IGridCellValueGetter> GetGridCellsGetters(RenderTexture gridsRenderTexture, PartitionSettings partitionSettings)
        {
            var list = new List<IGridCellValueGetter>();

            for (var centerIndex = 0; centerIndex < partitionSettings.CentersSettings.CentersCount; centerIndex++)
            {
                var data = GetTextureNativeArray(gridsRenderTexture, centerIndex);

                Assert.AreEqual(data.Length, partitionSettings.SpaceSettings.GridSize[0] * partitionSettings.SpaceSettings.GridSize[1]);

                var valueTextureGetter = new GridValueTextureCalculator(data, partitionSettings.SpaceSettings.GridSize[0]);

                list.Add(valueTextureGetter);
            }

            return list;
        }

        private NativeArray<float> GetTextureNativeArray(RenderTexture gridsRenderTexture, int centerIndex)
        {
            var sliceTexture = _slicer.Copy3DSliceToRenderTexture(gridsRenderTexture, centerIndex);
            var data = GetTextureNativeArray(sliceTexture);
            return data;
        }

        private static NativeArray<float> GetTextureNativeArray(RenderTexture texture)
        {
            var texture2D = texture.ToTexture2D();
            var data = texture2D.GetRawTextureData<float>();
            return data;
        }
    }
}