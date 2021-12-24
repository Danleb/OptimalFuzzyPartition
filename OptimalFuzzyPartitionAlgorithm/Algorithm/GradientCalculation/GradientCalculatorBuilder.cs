using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vector = MathNet.Numerics.LinearAlgebra.Vector<double>;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm.GradientCalculation
{
    public class GradientCalculatorBuilder
    {
        public static Func<Vector, Vector> CreateGradientEvaluator(PartitionSettings settings, Func<CentersSettings, List<GridValueInterpolator>> partitionCreator)
        {
            Func<Vector, Vector> subgradientEvaluator = vector =>
            {
                var centerDatas = FuzzyPartitionPlacingCentersAlgorithm.GetCenterDatas(settings.CentersSettings, vector);
                var centersSettings = new CentersSettings { CenterDatas = centerDatas };
                var muValueGetters = partitionCreator(centersSettings);
                var placingCenters = centerDatas.Select((data, indexInInputVector) => (data, indexInInputVector)).Where(tuple => !tuple.data.IsFixed).ToList();
                var gradientVector = Vector.Build.Dense(centersSettings.PlacingCentersCount * 2);
                Parallel.For(0, centersSettings.PlacingCentersCount, indexInOutputVector =>
                {
                    var (data, indexInInputVector) = placingCenters[indexInOutputVector];
                    var gradientEvaluator = new GradientCalculator(settings.SpaceSettings, settings.GaussLegendreIntegralOrder);
                    var centerGradient = gradientEvaluator.CalculateGradientForCenter(data.Position, muValueGetters[indexInInputVector]);
                    gradientVector[indexInOutputVector * 2] = centerGradient[0];
                    gradientVector[indexInOutputVector * 2 + 1] = centerGradient[1];
                });
                return gradientVector;
            };
            return subgradientEvaluator;
        }

        public static Func<Vector, Vector> CreateGradientEvaluatorCPU(PartitionSettings settings)
        {
            return CreateGradientEvaluator(settings, centersSettings =>
            {
                var fuzzyFixedPartitionEvaluator = new FuzzyPartitionFixedCentersAlgorithm(settings.SpaceSettings, centersSettings, settings.FuzzyPartitionFixedCentersSettings);
                var partition = fuzzyFixedPartitionEvaluator.BuildPartition();
                var muValueGetters = partition.CreateGridValueInterpolators(settings.SpaceSettings);
                return muValueGetters;
            });
        }
    }
}
