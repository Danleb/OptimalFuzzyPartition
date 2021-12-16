using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FuzzyPartitionComputing
{
    /// <summary>
    /// Retrieves data from native ComputeBuffer to managed side and creates grid value getters.
    /// </summary>
    public static class ComputeBufferToGridConverter
    {
        public static ArrayGridCellValueGetter GetGridCellValueGetter(ComputeBuffer grid2d, PartitionSettings partitionSettings)
        {
            var array = new float[grid2d.count];
            grid2d.GetData(array);
            var gridSizeX = partitionSettings.SpaceSettings.GridSize[0];
            var calculator = new ArrayGridCellValueGetter(array, gridSizeX);
            return calculator;
        }

        public static List<IGridCellValueGetter> GetGridCellsGetters(ComputeBuffer grid3d, PartitionSettings partitionSettings)
        {
            var list = new List<IGridCellValueGetter>();
            var muGridsArray = new float[grid3d.count];
            grid3d.GetData(muGridsArray);

            for (var centerIndex = 0; centerIndex < partitionSettings.CentersSettings.CentersCount; centerIndex++)
            {
                var valueTextureGetter = new ArrayGridCellValueGetter(muGridsArray, partitionSettings.SpaceSettings.GridSize[0], partitionSettings.SpaceSettings.GridSize[1], centerIndex);
                list.Add(valueTextureGetter);
            }

            return list;
        }

        public static GridValueInterpolator GetGridValueInterpolator(ComputeBuffer grid2d, PartitionSettings partitionSettings)
        {
            var calculator = GetGridCellValueGetter(grid2d, partitionSettings);
            var interpolator = new GridValueInterpolator(partitionSettings.SpaceSettings, calculator);
            return interpolator;
        }

        public static List<GridValueInterpolator> GetGridValueInterpolators(ComputeBuffer grid3d, PartitionSettings partitionSettings)
        {
            var gridValueGetters = GetGridCellsGetters(grid3d, partitionSettings);
            var interpolators = gridValueGetters.Select(v => new GridValueInterpolator(partitionSettings.SpaceSettings, v)).ToList();
            return interpolators;
        }
    }
}
