using MathNet.Numerics.LinearAlgebra;
using System;

namespace OptimalFuzzyPartitionAlgorithm.ClientMessaging
{
    [Serializable]
    public class PartitionResult
    {
        public bool WorkFinished;
        public double TargetFunctionalValue;
        public double DualFunctionalValue;
        public Matrix<double> MuMatrix;
        public Matrix<double> PsiMatrix;
        public Vector<double>[] CentersPositions;
        public int PerformedIterationsCount;
    }
}