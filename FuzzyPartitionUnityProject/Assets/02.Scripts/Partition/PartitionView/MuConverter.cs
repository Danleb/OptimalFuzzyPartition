using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using System.Collections.Generic;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace FuzzyPartitionComputing
{
    /// <summary>
    /// Converts MuGrids RenderTexture to the list of matrices for each center.
    /// </summary>
    public class MuConverter : MonoBehaviour
    {
        [SerializeField] private Slicer _slicer;

        public List<IMuValueGetter> GetMuGridsInterpolators(RenderTexture muGridsRenderTexture, PartitionSettings partitionSettings)
        {
            var list = new List<IMuValueGetter>();

            for (var centerIndex = 0; centerIndex < partitionSettings.CentersSettings.CentersCount; centerIndex++)
            {
                var data = GetTextureNativeArray(muGridsRenderTexture, centerIndex);

                Assert.AreEqual(data.Length, partitionSettings.SpaceSettings.GridSize[0] * partitionSettings.SpaceSettings.GridSize[1]);

                var muValueTextureGetter = new MuValueTextureCalculator(data, partitionSettings.SpaceSettings.GridSize[0]);

                list.Add(muValueTextureGetter);
            }

            return list;
        }

        public List<IMuValueGetter> ConvertMuGridsTexture(RenderTexture muGridsRenderTexture, PartitionSettings partitionSettings)
        {
            var list = new List<IMuValueGetter>();

            for (var centerIndex = 0; centerIndex < partitionSettings.CentersSettings.CentersCount; centerIndex++)
            {
                var matrix = Matrix<double>.Build.Sparse(partitionSettings.SpaceSettings.GridSize[0], partitionSettings.SpaceSettings.GridSize[1]);
                var data = GetTextureNativeArray(muGridsRenderTexture, centerIndex);

                Assert.AreEqual(data.Length, partitionSettings.SpaceSettings.GridSize[0] * partitionSettings.SpaceSettings.GridSize[1]);

                for (var rowIndex = 0; rowIndex < partitionSettings.SpaceSettings.GridSize[0]; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < partitionSettings.SpaceSettings.GridSize[1]; columnIndex++)
                    {
                        float muValue = data[columnIndex * partitionSettings.SpaceSettings.GridSize[0] + rowIndex];
                        matrix[rowIndex, columnIndex] = muValue;
                    }
                }

                var muGridValueGetter = new MuGridValueGetter(matrix);

                list.Add(muGridValueGetter);
            }

            return list;
        }

        private NativeArray<float> GetTextureNativeArray(RenderTexture muGridsRenderTexture, int centerIndex)
        {
            var sliceTexture = _slicer.Copy3DSliceToRenderTexture(muGridsRenderTexture, centerIndex);
            var texture2D = sliceTexture.ToTexture2D();
            var data = texture2D.GetRawTextureData<float>();
            return data;
        }
    }
}