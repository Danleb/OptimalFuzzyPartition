using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using PartitionView;
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

        public PartitionResult CreateFuzzyPartitionWithFixedCenters(PartitionSettings partitionSettings, bool drawMistrustRate, double mistrustRate, bool alwaysShowCentersInfo)
        {
            _partitionFixedCentersComputer.Init(partitionSettings);

            var muRenderTexture = _partitionFixedCentersComputer.Run(out var psiGridTexture);

            var result = new PartitionResult
            {
                TargetFunctionalValue = CalculateTargetFunctionalValue(partitionSettings, muRenderTexture),
                DualFunctionalValue = CalculateDualFunctionalValue(partitionSettings, psiGridTexture),
                WorkFinished = true
            };

            _partitionImageShower.AlwaysShowCentersInfo = alwaysShowCentersInfo;
            _partitionImageShower.DrawWithMistrustRate = drawMistrustRate;
            _partitionImageShower.MistrustRate = (float)mistrustRate;
            _partitionImageShower.CreatePartitionAndShow(partitionSettings, muRenderTexture);

            return result;
        }

        public PartitionResult CreateFuzzyPartitionWithCentersPlacing(PartitionSettings partitionSettings, bool drawMistrustRate, double mistrustRate, bool alwaysShowCentersInfo)
        {
            _partitionPlacingCentersComputer.Init(partitionSettings);

            var centersPositions = _partitionPlacingCentersComputer.Run();

            for (var i = 0; i < centersPositions.Count; i++)
                partitionSettings.CentersSettings.CenterDatas[i].Position = centersPositions[i];

            var result = CreateFuzzyPartitionWithFixedCenters(partitionSettings, drawMistrustRate, mistrustRate, alwaysShowCentersInfo);
            result.CentersPositions = centersPositions.ToArray();
            result.WorkFinished = true;

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

        public void RedrawPartitionWithSettings(bool drawMistrustRate, double mistrustRate)
        {
            _partitionImageShower.DrawWithMistrustRate = drawMistrustRate;
            _partitionImageShower.MistrustRate = (float)mistrustRate;
            _partitionImageShower.CreatePartitionAndShow();
        }

        private double CalculateTargetFunctionalValue(PartitionSettings partitionSettings, RenderTexture muRenderTexture)
        {
            if (!_calculateTargetFunctionalValue)
                return double.MinValue;

            var muGridGetters = _textureToGridConverter.GetGridValueInterpolators(muRenderTexture, partitionSettings);
            var targetFunctionalValue = new TargetFunctionalCalculator(partitionSettings).CalculateFunctionalValue(muGridGetters);
            Logger.Info($"Target functional value = {targetFunctionalValue}");
            return targetFunctionalValue;
        }

        private double CalculateDualFunctionalValue(PartitionSettings partitionSettings, RenderTexture psiGridTexture)
        {
            if (!_calculateDualFunctionalValue)
                return double.MinValue;

            var psiGridValueGetter = _textureToGridConverter.GetGridValueInterpolator(psiGridTexture, partitionSettings);
            var dualFunctionalValue = new DualFunctionalCalculator(partitionSettings, psiGridValueGetter).CalculateFunctionalValue();
            Logger.Info($"Dual functional value = {dualFunctionalValue}");
            return dualFunctionalValue;
        }
    }
}