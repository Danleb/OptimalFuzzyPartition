using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;

namespace OptimalFuzzyPartition.Model
{
    public static class DefaultSettingsKeeper
    {
        public static PartitionSettings GetPartitionSettings()
        {
            return new PartitionSettings
            {
                IsCenterPlacingTask = true,
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(0, 0),
                    MaxCorner = VectorUtils.CreateVector(10, 10),
                    GridSize = new List<int> { 128, 128 },
                    DensityType = DensityType.Everywhere1,
                    MetricsType = MetricsType.Euclidean,
                    CustomDensityFunction = null,
                    CustomDistanceFunction = null
                },
                CentersSettings = new CentersSettings
                {
                    CentersCount = 2,
                    CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(3.33, 5),
                            IsFixed = false
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(6.66, 5),
                            IsFixed = false
                        },
                    }
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.001,
                    GradientStep = 10,
                    MaxIterationsCount = 400
                },
                FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
                {
                    CentersDeltaEpsilon = 0.5,
                    GaussLegendreIntegralOrder = 32
                },
                RAlgorithmSettings = new RAlgorithmSettings
                {
                    SpaceStretchFactor = 2,
                    H0 = 9,
                    MaxIterationsCount = 40
                }
            };
        }

        public static CenterData GetDefaultCenterData(PartitionSettings settings)
        {
            var data = new CenterData
            {
                IsFixed = false,
                Position = (settings.SpaceSettings.MaxCorner + settings.SpaceSettings.MinCorner) / 2,
                A = 0,
                W = 1
            };

            return data;
        }
    }
}