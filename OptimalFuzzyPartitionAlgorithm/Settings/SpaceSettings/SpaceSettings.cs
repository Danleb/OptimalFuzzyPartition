﻿using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm
{
    [Serializable]
    public class SpaceSettings
    {
        public Vector<double> MinCorner;

        public Vector<double> MaxCorner;

        public List<int> GridSize;

        public DensityType DensityType;

        public Func<Vector<double>, double> CustomDensityFunction;

        public MetricsType MetricsType;

        public Func<Vector<double>, Vector<double>, double> CustomDistanceFunction;

        [JsonIgnore]
        public int DimensionsCount => MinCorner.Count;

        [JsonIgnore]
        public double Ratio => Width / Height;

        [JsonIgnore]
        public double Width => MaxCorner[0] - MinCorner[0];

        [JsonIgnore]
        public double Height => MaxCorner[1] - MinCorner[1];
    }
}
