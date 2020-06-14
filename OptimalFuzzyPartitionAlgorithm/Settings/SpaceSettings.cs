using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm
{
    [Serializable]
    public class SpaceSettings
    {
        public int DimensionsCount => MinCorner.Count;

        public Vector<double> MinCorner;

        public Vector<double> MaxCorner;

        public List<int> GridSize;

        public DensityType DensityType;

        public Func<Vector<double>, double> CustomDensityFunction;

        public MetricsType MetricsType;

        public Func<Vector<double>, Vector<double>, double> CustomDistanceFunction;
    }
}