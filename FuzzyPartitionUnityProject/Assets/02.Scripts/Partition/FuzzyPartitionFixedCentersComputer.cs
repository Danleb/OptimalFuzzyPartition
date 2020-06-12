using OptimalFuzzyPartitionAlgorithm;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utils;

namespace FuzzyPartitionComputing
{
    public class FuzzyPartitionFixedCentersComputer : MonoBehaviour
    {
        [SerializeField] private ComputeShader _fuzzyPartitionShader;
        [SerializeField] private Slicer _slicer;

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

        public int PerformedIterationsCount { get; private set; }

        public PartitionSettings Settings { get; private set; }

        public void Init(PartitionSettings partitionSettings)
        {
            Debug.Log("FuzzyFixedPartition Initialization");

            Settings = partitionSettings;

            SetGroupSizes();

            SetMinCornerToShader();
            SetDiffToShader();

            var gridSize = Settings.GridSize.Select(v => (float)(v - 1)).ToArray();
            _fuzzyPartitionShader.SetFloats("GridSize", gridSize);

            _centersPositionsBuffer = new ComputeBuffer(Settings.CentersCount, sizeof(float) * 2, ComputeBufferType.Default);

            _additiveCoefficientsBuffer = new ComputeBuffer(Settings.CentersCount, sizeof(float), ComputeBufferType.Default);

            _multiplicativeCoefficientsBuffer = new ComputeBuffer(Settings.CentersCount, sizeof(float), ComputeBufferType.Default);

            _stopConditionComputeBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Default);

            _zeroIterationInitKernelHandle = _fuzzyPartitionShader.FindKernel(ZeroIterationInitName);
            _updateMuKernelHandle = _fuzzyPartitionShader.FindKernel(UpdateMuKernelName);
            _updatePsiKernelHandle = _fuzzyPartitionShader.FindKernel(UpdatePsiKernelName);

            _psiGridTexture = new RenderTexture(Settings.GridSize[0], Settings.GridSize[1], 0, GraphicsFormat.R32_SFloat);
            _psiGridTexture.enableRandomWrite = true;
            _psiGridTexture.Create();

            _muGridsTexture = new RenderTexture(Settings.GridSize[0], Settings.GridSize[1], 0, GraphicsFormat.R32_SFloat);
            _muGridsTexture.dimension = TextureDimension.Tex3D;
            _muGridsTexture.volumeDepth = Settings.CentersCount;
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
                Settings.GridSize[0] / _zeroInitNumThreads.x,
                Settings.GridSize[1] / _zeroInitNumThreads.y,
                Settings.CentersCount / _zeroInitNumThreads.z
                );
            _muUpdateGroups = new Vector3Int(
                Settings.GridSize[0] / _muUpdateNumThreads.x,
                Settings.GridSize[1] / _muUpdateNumThreads.y,
                Settings.CentersCount / _muUpdateNumThreads.z
            );
            _psiUpdateGroups = new Vector3Int(
                Settings.GridSize[0] / _psiUpdateNumThreads.x,
                Settings.GridSize[1] / _psiUpdateNumThreads.y,
                1
                );
        }

        private void SetDiffToShader()
        {
            var diff = (Settings.MaxCorner - Settings.MinCorner).ToVector2();
            _fuzzyPartitionShader.SetFloats("Diff", diff.ToArray());
        }

        private void SetMinCornerToShader()
        {
            var minCorner = Settings.MinCorner.ToVector2();
            _fuzzyPartitionShader.SetFloats("MinCorner", minCorner.ToArray());
        }

        public RenderTexture Run()
        {
            PerformedIterationsCount = 0;

            for (var i = 0; i < Settings.FixedPartitionMaxIterationsCount; i++)
            {
                Debug.Log($"FuzzyFixedPartition Iteration={PerformedIterationsCount + 1}");

                _fuzzyPartitionShader.SetInt("CentersCount", Settings.CentersCount);

                SetCentersPositionsToBuffer();//to init
                _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "CentersPositions", _centersPositionsBuffer);

                SetAdditiveCoefficientsToBuffer();//to init
                _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "AdditiveCoefficients", _additiveCoefficientsBuffer);

                SetMultiplicativeCoefficientsToBuffer();//to init
                _fuzzyPartitionShader.SetBuffer(_updateMuKernelHandle, "MultiplicativeCoefficients", _multiplicativeCoefficientsBuffer);

                var lambda = (float)Settings.FixedPartitionGradientStep;
                _fuzzyPartitionShader.SetFloat("GradientLambdaStep", lambda);

                var epsilon = (float)Settings.FixedPartitionGradientEpsilon;
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

        private void SetMultiplicativeCoefficientsToBuffer()
        {
            var multiplicativeCoefficients = Settings.MultiplicativeCoefficients.Select(v => (float)v).ToArray();
            _multiplicativeCoefficientsBuffer.SetData(multiplicativeCoefficients);
        }

        private void SetAdditiveCoefficientsToBuffer()
        {
            var additiveCoefficients = Settings.AdditiveCoefficients.Select(v => (float)v).ToArray();
            _additiveCoefficientsBuffer.SetData(additiveCoefficients);
        }

        private void SetCentersPositionsToBuffer()
        {
            var centersArray = new float[Settings.CentersCount * 2];
            for (var i = 0; i < Settings.CentersCount; i++)
            {
                centersArray[i * 2] = (float)Settings.CenterPositions[i][0];
                centersArray[i * 2 + 1] = (float)Settings.CenterPositions[i][1];
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