using Assets._02.Scripts;
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

        public int PerformedIterationsCount { get; private set; }

        public PartitionSettings Settings { get; private set; }

        private const string ZeroIterationInitName = "ZeroIterationInit";
        private const string UpdateMuKernelName = "UpdateMu";
        private const string UpdatePsiKernelName = "UpdatePsi";

        private int _zeroIterationInitKernelHandle;
        private int _updateMuKernelHandle;
        private int _updatePsiKernelHandle;

        [SerializeField] private Vector3Int _zeroInitGroups;
        [SerializeField] private Vector3Int _groupsMu;
        [SerializeField] private Vector3Int _groupsPsi;

        private RenderTexture _muGridsTexture;
        private RenderTexture _psiGridTexture;

        private ComputeBuffer _additiveCoefficientsBuffer;
        private ComputeBuffer _multiplicativeCoefficientsBuffer;
        private ComputeBuffer _centersPositionsBuffer;

        /// <summary>
        /// For debug purposes.
        /// </summary>
        [SerializeField] private Image[] rs;

        [SerializeField] private bool debug;

        public void Init(PartitionSettings partitionSettings)
        {
            Debug.Log("FuzzyFixedPartition Initialization");

            Settings = partitionSettings;

            _centersPositionsBuffer = new ComputeBuffer(Settings.CentersCount, sizeof(float) * 2, ComputeBufferType.Default);

            _additiveCoefficientsBuffer = new ComputeBuffer(Settings.CentersCount, sizeof(float), ComputeBufferType.Default);

            _multiplicativeCoefficientsBuffer = new ComputeBuffer(Settings.CentersCount, sizeof(float), ComputeBufferType.Default);

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

            _fuzzyPartitionShader.Dispatch(_zeroIterationInitKernelHandle, Settings.GridSize[0] / _zeroInitGroups.x, Settings.GridSize[1] / _zeroInitGroups.y, Settings.CentersCount / _zeroInitGroups.z);

            ShowMuTextures();
        }

        public void Run()
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

                //
                _fuzzyPartitionShader.Dispatch(_updateMuKernelHandle, _groupsMu.x, _groupsMu.y, _groupsMu.z);

                _fuzzyPartitionShader.Dispatch(_updatePsiKernelHandle, _groupsPsi.x, _groupsPsi.y, _groupsPsi.z);

                PerformedIterationsCount++;

                //_psiBuffer.GetData();

                if (true)
                {
                    //break;
                }
            }

            //_muBuffer.GetData();


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
                centersArray[i] = (float)Settings.CenterPositions[0][0];
                centersArray[i + 1] = (float)Settings.CenterPositions[0][1];
            }

            _centersPositionsBuffer.SetData(centersArray);
        }
    }
}