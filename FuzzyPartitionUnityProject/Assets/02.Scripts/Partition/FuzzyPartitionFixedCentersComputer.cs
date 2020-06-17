using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utils;
using Debug = System.Diagnostics.Debug;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionFixedCentersComputer : MonoBehaviour
    {
        [SerializeField] private ComputeShader _fuzzyPartitionShader;
        [SerializeField] private Slicer _slicer;
        [SerializeField] private MuConverter _muConverter;

        #region DebugRes
        [SerializeField] private bool debug;
        [SerializeField] private Image[] rs;
        #endregion

        [SerializeField] private Vector3Int _zeroInitNumThreads;
        [SerializeField] private Vector3Int _muUpdateNumThreads;
        [SerializeField] private Vector3Int _psiUpdateNumThreads;

        private Vector3Int _zeroInitGroups;
        private Vector3Int _muUpdateGroups;
        private Vector3Int _psiUpdateGroups;

        private const string ZeroIterationInitName = "ZeroIterationInit";//TODO put out
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
        private readonly int[] _stopConditionsArray = new int[1];

        private Stopwatch _iterationTimer;
        private Stopwatch _globalTimer;

        public int PerformedIterationsCount { get; private set; }

        public PartitionSettings Settings { get; private set; }

        public void Init(PartitionSettings partitionSettings)
        {
            Debug.WriteLine("FuzzyFixedPartition Initialization");

            Settings = partitionSettings;

            _iterationTimer = new Stopwatch();
            _globalTimer = new Stopwatch();

            SetGroupSizes();

            SetMinCornerToShader();
            SetDiffToShader();

            var gridSize = Settings.SpaceSettings.GridSize.Select(v => (float)(v - 1)).ToArray();
            _fuzzyPartitionShader.SetFloats("GridSize", gridSize);

            _centersPositionsBuffer = new ComputeBuffer(Settings.CentersSettings.CentersCount, sizeof(float) * 2, ComputeBufferType.Default);

            _additiveCoefficientsBuffer = new ComputeBuffer(Settings.CentersSettings.CentersCount, sizeof(float), ComputeBufferType.Default);

            _multiplicativeCoefficientsBuffer = new ComputeBuffer(Settings.CentersSettings.CentersCount, sizeof(float), ComputeBufferType.Default);

            _stopConditionComputeBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Default);

            _zeroIterationInitKernelHandle = _fuzzyPartitionShader.FindKernel(ZeroIterationInitName);
            _updateMuKernelHandle = _fuzzyPartitionShader.FindKernel(UpdateMuKernelName);
            _updatePsiKernelHandle = _fuzzyPartitionShader.FindKernel(UpdatePsiKernelName);

            _psiGridTexture = new RenderTexture(Settings.SpaceSettings.GridSize[0], Settings.SpaceSettings.GridSize[1], 0, GraphicsFormat.R32_SFloat);
            _psiGridTexture.enableRandomWrite = true;
            _psiGridTexture.Create();

            _muGridsTexture = new RenderTexture(Settings.SpaceSettings.GridSize[0], Settings.SpaceSettings.GridSize[1], 0, GraphicsFormat.R32_SFloat);
            _muGridsTexture.dimension = TextureDimension.Tex3D;
            _muGridsTexture.volumeDepth = Settings.CentersSettings.CentersCount;
            _muGridsTexture.enableRandomWrite = true;
            _muGridsTexture.Create();

            _fuzzyPartitionShader.SetTexture(_zeroIterationInitKernelHandle, "MuGrids", _muGridsTexture);
            _fuzzyPartitionShader.SetTexture(_zeroIterationInitKernelHandle, "PsiGrid", _psiGridTexture);

            _fuzzyPartitionShader.Dispatch(_zeroIterationInitKernelHandle, _zeroInitGroups.x, _zeroInitGroups.y, _zeroInitGroups.z);

            ShowMuTextures();
        }

        private void SetGroupSizes()
        {
            _zeroInitGroups = new Vector3Int(
                Settings.SpaceSettings.GridSize[0] / _zeroInitNumThreads.x,
                Settings.SpaceSettings.GridSize[1] / _zeroInitNumThreads.y,
                Settings.CentersSettings.CentersCount / _zeroInitNumThreads.z
                );
            _muUpdateGroups = new Vector3Int(
                Settings.SpaceSettings.GridSize[0] / _muUpdateNumThreads.x,
                Settings.SpaceSettings.GridSize[1] / _muUpdateNumThreads.y,
                Settings.CentersSettings.CentersCount / _muUpdateNumThreads.z
            );
            _psiUpdateGroups = new Vector3Int(
                Settings.SpaceSettings.GridSize[0] / _psiUpdateNumThreads.x,
                Settings.SpaceSettings.GridSize[1] / _psiUpdateNumThreads.y,
                1
                );
        }

        public RenderTexture Run()
        {
            _globalTimer.Start();
            _iterationTimer.Start();

            PerformedIterationsCount = 0;

            for (var i = 0; i < Settings.FuzzyPartitionFixedCentersSettings.MaxIterationsCount; i++)
            {
                _iterationTimer.Reset();

                _fuzzyPartitionShader.SetInt("CentersCount", Settings.CentersSettings.CentersCount);

                SetCentersPositionsToBuffer();//to init
                _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "CentersPositions", _centersPositionsBuffer);

                SetAdditiveCoefficientsToBuffer();//to init
                _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "AdditiveCoefficients", _additiveCoefficientsBuffer);

                SetMultiplicativeCoefficientsToBuffer();//to init
                _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "MultiplicativeCoefficients", _multiplicativeCoefficientsBuffer);

                var lambda = (float)Settings.FuzzyPartitionFixedCentersSettings.GradientStep;
                _fuzzyPartitionShader.SetFloat("GradientLambdaStep", lambda);

                var epsilon = (float)Settings.FuzzyPartitionFixedCentersSettings.GradientEpsilon;
                _fuzzyPartitionShader.SetFloat("GradientEpsilon", epsilon);

                _fuzzyPartitionShader.SetTexture(_updateMuKernelHandle, "MuGrids", _muGridsTexture);
                _fuzzyPartitionShader.SetTexture(_updateMuKernelHandle, "PsiGrid", _psiGridTexture);

                _fuzzyPartitionShader.Dispatch(_updateMuKernelHandle, _muUpdateGroups.x, _muUpdateGroups.y, _muUpdateGroups.z);

                _fuzzyPartitionShader.SetTexture(_updatePsiKernelHandle, "MuGrids", _muGridsTexture);
                _fuzzyPartitionShader.SetTexture(_updatePsiKernelHandle, "PsiGrid", _psiGridTexture);

                _stopConditionsArray[0] = 999;
                _stopConditionComputeBuffer.SetData(_stopConditionsArray);
                _fuzzyPartitionShader.SetBuffer(_updatePsiKernelHandle, "StopConditions", _stopConditionComputeBuffer);

                _fuzzyPartitionShader.Dispatch(_updatePsiKernelHandle, _psiUpdateGroups.x, _psiUpdateGroups.y, _psiUpdateGroups.z);

                PerformedIterationsCount++;

                _stopConditionComputeBuffer.GetData(_stopConditionsArray);

                if (_stopConditionsArray[0] > 100)
                {
                    break;
                }

                ShowMuTextures();
            }

            Trace.WriteLine($"Partition execution time: {_globalTimer.ElapsedMilliseconds}");

            _globalTimer.Stop();
            _iterationTimer.Stop();

            //var muGrids2 = new FuzzyPartitionFixedCentersAlgorithm(Settings).BuildPartition();

            //var muGrids = _muConverter.ConvertMuGridsTexture(_muGridsTexture, Settings);

            //for (var index = 0; index < muGrids.Count; index++)
            //{
            //    var muGrid = muGrids[index];
            //    var muGrid2 = muGrids2[index];
            //    Trace.WriteLine($"Mu Matrix for the center #{index + 1}");
            //    MatrixUtils.TraceMatrix(Settings.SpaceSettings.GridSize[0], Settings.SpaceSettings.GridSize[1], (i, i1) => muGrid.GetMuValue(i, i1));

            //    Trace.WriteLine("Matrix done by CPU:");
            //    MatrixUtils.TraceMatrix(muGrid2);
            //}

            //var targetFunctionalCalculator = new TargetFunctionalCalculator(Settings);
            //var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muGrids);
            //Trace.WriteLine($"Target functional value = {targetFunctionalValue}\n");
           
            //var sum = muGrids.Aggregate((a, b) => a + b);
            //Trace.WriteLine("Sum mu matrix:");
            //MatrixUtils.TraceMatrix(sum);

            Debug.Flush();
            Trace.Flush();

            return _muGridsTexture;
        }

        private void ShowMuTextures()
        {
            if (debug)
            {
                var rt1 = _slicer.Copy3DSliceToRenderTexture(_muGridsTexture, 0);
                var sprite1 = rt1.ToSprite();
                rs[0].sprite = sprite1;

                var rt2 = _slicer.Copy3DSliceToRenderTexture(_muGridsTexture, 1);
                var sprite2 = rt2.ToSprite();
                rs[1].sprite = sprite2;
            }
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
        }

        private void SetAdditiveCoefficientsToBuffer()
        {
            var additiveCoefficients = Settings.CentersSettings.CenterDatas.Select(v => (float)v.A).ToArray();
            _additiveCoefficientsBuffer.SetData(additiveCoefficients);
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
        }

        public void Release()
        {
            _additiveCoefficientsBuffer?.Release();
            _centersPositionsBuffer?.Release();
            _multiplicativeCoefficientsBuffer?.Release();
            _stopConditionComputeBuffer?.Release();
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}