﻿using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.PartitionRate;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Utils;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionFixedCentersComputer : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private ComputeShader _fuzzyPartitionShader;
        [SerializeField] private TextureToGridConverter _textureToGridConverter;

        [SerializeField] private Vector3Int _zeroInitNumThreads;
        [SerializeField] private Vector3Int _muUpdateNumThreads;
        [SerializeField] private Vector3Int _psiUpdateNumThreads;

        [SerializeField] private bool _showMuGridsInfo;
        [SerializeField] private bool _compareWithCpuMuGrid;

        private Vector3Int _zeroInitGroups;
        private Vector3Int _muUpdateGroups;
        private Vector3Int _psiUpdateGroups;

        private const string ZeroIterationInitName = "ZeroIterationInit";
        private const string UpdateMuKernelName = "UpdateMu";
        private const string UpdatePsiKernelName = "UpdatePsi";

        private int _zeroIterationInitKernelHandle;
        private int _updateMuKernelHandle;
        private int _updatePsiKernelHandle;

        private RenderTexture _muGridsTexture;
        private RenderTexture _psiGridTexture;

        private ComputeBuffer _additiveCoefficientsBuffer;
        private ComputeBuffer _multiplicativeCoefficientsBuffer;
        private ComputeBuffer _centersPositionsBuffer;

        private ComputeBuffer _stopConditionComputeBuffer;
        //private readonly int[] _stopConditionsArray = new int[1];

        private Stopwatch _iterationTimer;
        private Stopwatch _globalTimer;

        public int PerformedIterationsCount { get; private set; }

        public PartitionSettings Settings { get; private set; }

        private void Awake()
        {
            PlayStateNotifier.OnPlaymodeExit += PlayStateNotifier_OnPlaymodeExit;

            _zeroIterationInitKernelHandle = _fuzzyPartitionShader.FindKernel(ZeroIterationInitName);
            _updateMuKernelHandle = _fuzzyPartitionShader.FindKernel(UpdateMuKernelName);
            _updatePsiKernelHandle = _fuzzyPartitionShader.FindKernel(UpdatePsiKernelName);

            _iterationTimer = new Stopwatch();
            _globalTimer = new Stopwatch();

            _zeroInitGroups = new Vector3Int();
            _muUpdateGroups = new Vector3Int();
            _psiUpdateGroups = new Vector3Int();
        }

        private void PlayStateNotifier_OnPlaymodeExit()
        {
            PlayStateNotifier.OnPlaymodeExit -= PlayStateNotifier_OnPlaymodeExit;
            ReleaseBuffers();
        }

        private void ReleaseBuffers()
        {
            _centersPositionsBuffer?.Release();
            _additiveCoefficientsBuffer?.Release();
            _multiplicativeCoefficientsBuffer?.Release();
        }

        private void PrepareTextures(int targetWidth, int targetHeight, int depth)
        {
            if (_psiGridTexture?.width != targetWidth || _psiGridTexture?.height != targetHeight)
            {
                if (_psiGridTexture != null)
                {
                    _psiGridTexture.Release();
                }

                Logger.Debug("Allocate psi grid RenderTexture.");
                _psiGridTexture = new RenderTexture(Settings.SpaceSettings.GridSize[0], Settings.SpaceSettings.GridSize[1], 0, GraphicsFormat.R32_SFloat);
                _psiGridTexture.enableRandomWrite = true;
                _psiGridTexture.Create();
            }

            if (_muGridsTexture?.width != targetWidth || _muGridsTexture?.height != targetHeight || _muGridsTexture?.volumeDepth != depth)
            {
                if (_muGridsTexture != null)
                {
                    _muGridsTexture.Release();
                }

                Logger.Debug("Allocate mu grids RenderTexture.");
                _muGridsTexture = new RenderTexture(Settings.SpaceSettings.GridSize[0], Settings.SpaceSettings.GridSize[1], 0, GraphicsFormat.R32_SFloat);
                _muGridsTexture.dimension = TextureDimension.Tex3D;
                _muGridsTexture.volumeDepth = Settings.CentersSettings.CentersCount;
                _muGridsTexture.enableRandomWrite = true;
                _muGridsTexture.Create();
            }
        }

        public static void PrepareBuffer(ref ComputeBuffer computeBuffer, int targetCount, int stride)
        {
            if (computeBuffer?.count == targetCount && (computeBuffer?.IsValid() ?? false))
                return;

            if (computeBuffer != null)
                computeBuffer.Release();

            Logger.Debug($"Allocate buffer Count={targetCount}, stride={stride}.");
            computeBuffer = new ComputeBuffer(targetCount, stride, ComputeBufferType.Default);
        }

        public void PrepareBuffers(int centersCount)
        {
            PrepareBuffer(ref _centersPositionsBuffer, centersCount, sizeof(float) * 2);
            PrepareBuffer(ref _additiveCoefficientsBuffer, centersCount, sizeof(float));
            PrepareBuffer(ref _multiplicativeCoefficientsBuffer, centersCount, sizeof(float));
            //PrepareBuffer(ref _stopConditionComputeBuffer, 1, sizeof(int));            
        }

        public void Init(PartitionSettings partitionSettings)
        {
            Logger.Debug("FuzzyFixedPartition Initialization");

            Settings = partitionSettings;

            _iterationTimer.Reset();
            _globalTimer.Reset();

            SetGroupSizes();
            SetMinCornerToShader();
            SetDiffToShader();

            var gridSize = Settings.SpaceSettings.GridSize.Select(v => (float)(v - 1)).ToArray();
            _fuzzyPartitionShader.SetFloats("GridSize", gridSize);

            var targetWidth = Settings.SpaceSettings.GridSize[0];
            var targetHeight = Settings.SpaceSettings.GridSize[1];

            PrepareBuffers(Settings.CentersSettings.CentersCount);
            PrepareTextures(targetWidth, targetHeight, Settings.CentersSettings.CentersCount);

            _fuzzyPartitionShader.SetTexture(_zeroIterationInitKernelHandle, "MuGrids", _muGridsTexture);
            _fuzzyPartitionShader.SetTexture(_zeroIterationInitKernelHandle, "PsiGrid", _psiGridTexture);

            _fuzzyPartitionShader.Dispatch(_zeroIterationInitKernelHandle, _zeroInitGroups.x, _zeroInitGroups.y, _zeroInitGroups.z);

            var epsilon = (float)Settings.FuzzyPartitionFixedCentersSettings.GradientEpsilon;
            _fuzzyPartitionShader.SetFloat("GradientEpsilon", epsilon);
            _fuzzyPartitionShader.SetInt("CentersCount", Settings.CentersSettings.CentersCount);

            SetCentersPositionsToBuffer();
            SetAdditiveCoefficientsToBuffer();
            SetMultiplicativeCoefficientsToBuffer();

            _fuzzyPartitionShader.SetTexture(_updateMuKernelHandle, "MuGrids", _muGridsTexture);
            _fuzzyPartitionShader.SetTexture(_updateMuKernelHandle, "PsiGrid", _psiGridTexture);

            _fuzzyPartitionShader.SetTexture(_updatePsiKernelHandle, "MuGrids", _muGridsTexture);
            _fuzzyPartitionShader.SetTexture(_updatePsiKernelHandle, "PsiGrid", _psiGridTexture);
        }

        public RenderTexture Run() => Run(out _);

        public RenderTexture Run(out RenderTexture psiGridTexture)
        {
            _globalTimer.Start();
            _iterationTimer.Start();

            PerformedIterationsCount = 0;

            for (var i = 0; i < Settings.FuzzyPartitionFixedCentersSettings.MaxIterationsCount; i++)
            {
                _iterationTimer.Reset();
                _fuzzyPartitionShader.Dispatch(_updateMuKernelHandle,
                    _muUpdateGroups.x, _muUpdateGroups.y, _muUpdateGroups.z);

                //_stopConditionsArray[0] = 999;
                //_stopConditionComputeBuffer.SetData(_stopConditionsArray);
                //_fuzzyPartitionShader.SetBuffer(_updatePsiKernelHandle, "StopConditions", _stopConditionComputeBuffer);

                if (i != Settings.FuzzyPartitionFixedCentersSettings.MaxIterationsCount - 1)
                {
                    var lambda = (float)Settings.FuzzyPartitionFixedCentersSettings.GradientStep;
                    var step = lambda / (PerformedIterationsCount + 1f);
                    //_fuzzyPartitionShader.SetFloat("GradientLambdaStep", lambda);
                    _fuzzyPartitionShader.SetFloat("GradientLambdaStep", step);
                    _fuzzyPartitionShader.Dispatch(_updatePsiKernelHandle,
                        _psiUpdateGroups.x, _psiUpdateGroups.y, _psiUpdateGroups.z);
                }

                PerformedIterationsCount++;
                SetCentersPositionsToBuffer();//to init

                //_stopConditionComputeBuffer.GetData(_stopConditionsArray);
                //if (_stopConditionsArray[0] > 100)
                //    break;
            }

            _globalTimer.Stop();
            _iterationTimer.Stop();

            Logger.Debug($"Partition execution time: {_globalTimer.ElapsedMilliseconds}");

            //ShowMuGridsInfo();

            psiGridTexture = _psiGridTexture;

            return _muGridsTexture;

        }

        private void ShowMuGridsInfo()//IList<IGridCellValueGetter> muGrids, IGridCellValueGetter psiGrid)
        {
            if (!_showMuGridsInfo)
                return;

            List<Matrix<double>> muGridsCPU = null;
            if (_compareWithCpuMuGrid)
            {
                muGridsCPU = new FuzzyPartitionFixedCentersAlgorithm(Settings.SpaceSettings,
                                                                     Settings.CentersSettings,
                                                                     Settings.FuzzyPartitionFixedCentersSettings)
                    .BuildPartition(out var psiGrid);
                //var muGrids2 = algorithm.BuildPartition().Select(v => new MatrixGridValueGetter(v)).ToList();rg

                var targetFunctionalValueFromCpu = new TargetFunctionalCalculator(Settings.SpaceSettings, Settings.CentersSettings, Settings.GaussLegendreIntegralOrder)
                    .CalculateFunctionalValue(muGridsCPU.Select(v => new GridValueInterpolator(Settings.SpaceSettings, new MatrixGridValueGetter(v))).ToList());
                Logger.Trace($"Target functional value by CPU: {targetFunctionalValueFromCpu}");

                var dualFunctionalValueFromCpu = new DualFunctionalCalculator(Settings.SpaceSettings,
                            Settings.CentersSettings,
                            Settings.GaussLegendreIntegralOrder,
                            new GridValueInterpolator(Settings.SpaceSettings, new MatrixGridValueGetter(psiGrid)))
                    .CalculateFunctionalValue();
                Logger.Trace($"Dual functional value by CPU: {dualFunctionalValueFromCpu}");
            }

            var muGrids = _textureToGridConverter.GetGridCellsGetters(_muGridsTexture, Settings);

            for (var index = 0; index < muGrids.Count; index++)
            {
                if (_compareWithCpuMuGrid)
                {
                    var muGridCPU = muGridsCPU[index];
                    Logger.Trace($"Mu Matrix by CPU for the center #{index + 1}");
                    MatrixUtils.WriteMatrix(
                        Settings.SpaceSettings.GridSize[0],
                        Settings.SpaceSettings.GridSize[1],
                        (i, i1) => muGridCPU[i, i1],
                        Logger.Trace,
                        3);
                }

                var muGrid = muGrids[index];

                Logger.Trace($"Mu Matrix by GPU for the center #{index + 1}");
                MatrixUtils.WriteMatrix(
                    Settings.SpaceSettings.GridSize[0],
                    Settings.SpaceSettings.GridSize[1],
                    (i, i1) => muGrid.GetValue(i, i1),
                    Logger.Trace,
                    3);
            }

            Logger.Trace("Sum mu matrix:");
            MatrixUtils.WriteMatrix(
                Settings.SpaceSettings.GridSize[0],
                Settings.SpaceSettings.GridSize[1],
                (i, i1) =>
                {
                    var v = 0d;
                    foreach (var t in muGrids)
                        v += t.GetValue(i, i1);
                    return v;
                },
                Logger.Trace,
                3);

            var psiGridCalculator = _textureToGridConverter.GetGridValueTextureCalculator(_psiGridTexture, Settings);

            Logger.Trace("Psi matrix:");
            MatrixUtils.WriteMatrix(
                Settings.SpaceSettings.GridSize[0],
                Settings.SpaceSettings.GridSize[1],
                (i, i1) => psiGridCalculator.GetValue(i, i1),
                Logger.Trace,
                3);

            for (var i = 0; i < Settings.CentersSettings.CentersCount; i++)
            {
                Logger.Trace($"Computed Mu grid #{i + 1} from Psi grid:");
                MatrixUtils.WriteMatrix(
                    Settings.SpaceSettings.GridSize[0],
                    Settings.SpaceSettings.GridSize[1],
                    (xIndex, yIndex) =>
                    {
                        var psiValue = psiGridCalculator.GetValue(xIndex, yIndex);
                        var xRatio = (double)xIndex / (Settings.SpaceSettings.GridSize[0] - 1d);
                        var yRatio = (double)yIndex / (Settings.SpaceSettings.GridSize[1] - 1d);
                        var ratioPoint = VectorUtils.CreateVector(xRatio, yRatio);
                        var point = Settings.SpaceSettings.MinCorner + ratioPoint.PointwiseMultiply(Settings.SpaceSettings.MaxCorner - Settings.SpaceSettings.MinCorner);
                        var densityValue = 1d;
                        var distance = (Settings.CentersSettings.CenterDatas[i].Position - point).L2Norm();
                        var newMuValue = -psiValue / (2d * densityValue * (distance / 1d + 0d));
                        return newMuValue;
                    },
                    Logger.Trace,
                    3);
            }
        }

        private void SetGroupSizes()
        {
            _zeroInitGroups.x = Settings.SpaceSettings.GridSize[0] / _zeroInitNumThreads.x;
            _zeroInitGroups.y = Settings.SpaceSettings.GridSize[1] / _zeroInitNumThreads.y;
            _zeroInitGroups.z = Settings.CentersSettings.CentersCount / _zeroInitNumThreads.z;

            _muUpdateGroups.x = Settings.SpaceSettings.GridSize[0] / _muUpdateNumThreads.x;
            _muUpdateGroups.y = Settings.SpaceSettings.GridSize[1] / _muUpdateNumThreads.y;
            _muUpdateGroups.z = Settings.CentersSettings.CentersCount / _muUpdateNumThreads.z;

            _psiUpdateGroups.x = Settings.SpaceSettings.GridSize[0] / _psiUpdateNumThreads.x;
            _psiUpdateGroups.y = Settings.SpaceSettings.GridSize[1] / _psiUpdateNumThreads.y;
            _psiUpdateGroups.z = 1;
        }

        private void SetDiffToShader()
        {
            var diff = (Settings.SpaceSettings.MaxCorner - Settings.SpaceSettings.MinCorner).ToVector2();
            _fuzzyPartitionShader.SetFloats("Diff", diff.ToArray());
        }

        private void SetMinCornerToShader()
        {
            var minCorner = Settings.SpaceSettings.MinCorner.ToVector2();
            _fuzzyPartitionShader.SetFloats("MinCorner", minCorner.ToArray());
        }

        private void SetMultiplicativeCoefficientsToBuffer()
        {
            var multiplicativeCoefficients = Settings.CentersSettings.CenterDatas.Select(v => (float)v.W).ToArray();
            _multiplicativeCoefficientsBuffer.SetData(multiplicativeCoefficients);
            _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "MultiplicativeCoefficients", _multiplicativeCoefficientsBuffer);
        }

        private void SetAdditiveCoefficientsToBuffer()
        {
            var additiveCoefficients = Settings.CentersSettings.CenterDatas.Select(v => (float)v.A).ToArray();
            _additiveCoefficientsBuffer.SetData(additiveCoefficients);
            _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "AdditiveCoefficients", _additiveCoefficientsBuffer);
        }

        private void SetCentersPositionsToBuffer()
        {
            var centersArray = new float[Settings.CentersSettings.CentersCount * 2];
            for (var i = 0; i < Settings.CentersSettings.CentersCount; i++)
            {
                centersArray[i * 2] = (float)Settings.CentersSettings.CenterDatas[i].Position[0];
                centersArray[i * 2 + 1] = (float)Settings.CentersSettings.CenterDatas[i].Position[1];
            }

            _centersPositionsBuffer.SetData(centersArray);
            _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "CentersPositions", _centersPositionsBuffer);
        }
    }
}
