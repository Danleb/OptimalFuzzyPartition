using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;

namespace OptimalFuzzyPartition.Model
{
    public static class DefaultSettingsBuilder
    {
        public static PartitionSettings GetPartitionSettings()
        {
            return new PartitionSettings
            {
                IsCenterPlacingTask = false,
                SpaceSettings = new SpaceSettings
                {
                    MinCorner = VectorUtils.CreateVector(0, 0),
                    MaxCorner = VectorUtils.CreateVector(1, 1),
                    GridSize = new List<int> { 128, 128 },
                    DensityType = DensityType.Everywhere1,
                    MetricsType = MetricsType.Euclidean,
                    CustomDensityFunction = null,
                    CustomDistanceFunction = null
                },
                CentersSettings = new CentersSettings
                {
                    CenterDatas = new List<CenterData>
                    {
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            //Position = VectorUtils.CreateVector(0.5, 0.5),
                            Position = VectorUtils.CreateVector(0.33, 0.5),
                            IsFixed = false
                        },
                        new CenterData
                        {
                            A = 0,
                            W = 1,
                            Position = VectorUtils.CreateVector(0.66, 0.5),
                            IsFixed = false
                        },
                    }
                },
                FuzzyPartitionFixedCentersSettings = new FuzzyPartitionFixedCentersSettings
                {
                    GradientEpsilon = 0.01,
                    GradientStep = 1,
                    MaxIterationsCount = 100,
                    PsiStartValue = 1
                },
                FuzzyPartitionPlacingCentersSettings = new FuzzyPartitionPlacingCentersSettings
                {
                    CentersDeltaEpsilon = 0.001
                },
                RAlgorithmSettings = new RAlgorithmSettings
                {
                    SpaceStretchFactor = 2,
                    H0 = 0.4,
                    MaxIterationsCount = 100,
                    IterationsCountToIncreaseStep = 3,
                    PrecisionBySubgradient = 0.00001,
                    StepDecreaseMultiplier = 0.9,
                    StepIncreaseMultiplier = 1.1,
                },
                GaussLegendreIntegralOrder = 32
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
