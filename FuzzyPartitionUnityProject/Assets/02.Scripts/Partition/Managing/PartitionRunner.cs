using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using PartitionView;
using System.Linq;
using UnityEngine;
using Utils;

namespace FuzzyPartitionComputing
{
    public class PartitionRunner : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private FuzzyPartitionPlacingCentersComputer _partitionPlacingCentersComputer;
        [SerializeField] private FuzzyPartitionFixedCentersComputer _partitionFixedCentersComputer;
        [SerializeField] private ScreenshotTaker _screenshotTaker;
        [SerializeField] private PartitionShower _partitionImageShower;
        [SerializeField] private TextureToGridConverter _textureToGridConverter;

        [SerializeField] private bool _calculateTargetFunctionalValue;
        [SerializeField] private bool _calculateDualFunctionalValue;

        public PartitionResult CreateFuzzyPartitionWithFixedCenters(PartitionSettings partitionSettings, RenderingSettings renderingSettings)
        {
            _partitionFixedCentersComputer.Init(partitionSettings);

            var muRenderTexture = _partitionFixedCentersComputer.Run(out var psiGridTexture);

            var result = new PartitionResult
            {
                TargetFunctionalValue = CalculateTargetFunctionalValue(partitionSettings, muRenderTexture),
                DualFunctionalValue = CalculateDualFunctionalValue(partitionSettings, psiGridTexture),
                WorkFinished = true
            };

            _partitionImageShower.RenderingSettings = renderingSettings;
            _partitionImageShower.CreatePartitionAndShow(partitionSettings, muRenderTexture);

            return result;
        }

        public PartitionResult CreateFuzzyPartitionWithCentersPlacing(PartitionSettings partitionSettings, RenderingSettings renderingSettings)
        {
            _partitionPlacingCentersComputer.Init(partitionSettings);
            var centerDatas = _partitionPlacingCentersComputer.Run(out var iterationsCount);
            partitionSettings.CentersSettings.CenterDatas = centerDatas;
            var centersPositions = centerDatas.Select(v => v.Position);

            var result = CreateFuzzyPartitionWithFixedCenters(partitionSettings, renderingSettings);
            result.CentersPositions = centersPositions.ToArray();
            result.WorkFinished = true;
            result.PerformedIterationsCount = iterationsCount;

            return result;
        }

        public void SavePartitionImage(string path = null)
        {
            if (path == null)
                _screenshotTaker.TakeAndSaveScreenshot();
            else
                _screenshotTaker.TakeAndSaveScreenshot(path);
        }

        public void DrawPartitionAtIteration(int iterationNumber)
        {
            Logger.Info($"Drawing partition at iteration {iterationNumber}");
        }

        public void RedrawPartitionWithSettings(RenderingSettings renderingSettings)
        {
            _partitionImageShower.RenderingSettings = renderingSettings;
            _partitionImageShower.CreatePartitionAndShow();
        }

        private double CalculateTargetFunctionalValue(PartitionSettings partitionSettings, RenderTexture muRenderTexture)
        {
            if (!_calculateTargetFunctionalValue)
                return double.MinValue;

            var muGridGetters = _textureToGridConverter.GetGridValueInterpolators(muRenderTexture, partitionSettings);
            var targetFunctionalValue = new TargetFunctionalCalculator(partitionSettings.SpaceSettings, partitionSettings.CentersSettings, partitionSettings.GaussLegendreIntegralOrder).CalculateFunctionalValue(muGridGetters);
            Logger.Info($"Target functional value = {targetFunctionalValue}");
            return targetFunctionalValue;
        }

        private double CalculateDualFunctionalValue(PartitionSettings partitionSettings, RenderTexture psiGridTexture)
        {
            if (!_calculateDualFunctionalValue)
                return double.MinValue;

            var psiGridValueGetter = _textureToGridConverter.GetGridValueInterpolator(psiGridTexture, partitionSettings);
            var dualFunctionalValue = new DualFunctionalCalculator(partitionSettings.SpaceSettings, partitionSettings.CentersSettings, partitionSettings.GaussLegendreIntegralOrder, psiGridValueGetter).CalculateFunctionalValue();
            Logger.Info($"Dual functional value = {dualFunctionalValue}");
            return dualFunctionalValue;
        }
    }
}
