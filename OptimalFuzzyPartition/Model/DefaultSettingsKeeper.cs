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
                IsCenterPlacingTask = false,
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
                            IsFixed = true
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(6.66, 5),
                            IsFixed = true
                        },
                    }
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.01,
                    GradientStep = 0.01,
                    MaxIterationsCount = 500
                },
                FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
                {
                    CentersDeltaEpsilon = 0.5,
                    GaussLegendreIntegralOrder = 32
                },
                RAlgorithmSettings = new RAlgorithmSettings
                {
                    SpaceStretchFactor = 2,
                    H0 = 1,
                    MaxIterationsCount = 100
                }
            };
        }
    }
}