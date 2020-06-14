using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace FuzzyPartitionComputing
{
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

                for (var rowIndex = 0; rowIndex < partitionSettings.SpaceSettings.GridSize[0]; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < partitionSettings.SpaceSettings.GridSize[1]; columnIndex++)
                    {

                        matrix[rowIndex, columnIndex] = 1;
                    }
                }

                list.Add(matrix);
            }

            return list;
        }
    }
}