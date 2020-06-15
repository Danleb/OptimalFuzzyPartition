using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using System.Collections.Generic;
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

        public List<Matrix<double>> ConvertMuGridsTexture(RenderTexture muGridsRenderTexture, PartitionSettings partitionSettings)
        {
            var list = new List<Matrix<double>>();

            for (var centerIndex = 0; centerIndex < partitionSettings.CentersSettings.CentersCount; centerIndex++)
            {
                var matrix = Matrix<double>.Build.Sparse(partitionSettings.SpaceSettings.GridSize[0], partitionSettings.SpaceSettings.GridSize[1]);
                var sliceTexture = _slicer.Copy3DSliceToRenderTexture(muGridsRenderTexture, centerIndex);
                var texture2D = sliceTexture.ToTexture2D();
                var data = texture2D.GetRawTextureData<float>();

                Assert.AreEqual(data.Length, partitionSettings.SpaceSettings.GridSize[0] * partitionSettings.SpaceSettings.GridSize[1]);

                for (var rowIndex = 0; rowIndex < partitionSettings.SpaceSettings.GridSize[0]; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < partitionSettings.SpaceSettings.GridSize[1]; columnIndex++)
                    {
                        float muValue = data[columnIndex * partitionSettings.SpaceSettings.GridSize[0] + rowIndex];
                        matrix[rowIndex, columnIndex] = muValue;
                    }
                }

                list.Add(matrix);
            }

            return list;
        }
    }
}