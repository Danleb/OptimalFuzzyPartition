﻿using System;

namespace OptimalFuzzyPartitionAlgorithm
{
    [Serializable]
    public class RAlgorithmSettings
    {
        /// <summary>
        /// Space stretching factor a.
        /// </summary>
        public double SpaceStretchFactor;

        /// <summary>
        /// Initial gradient step.
        /// </summary>
        public double H0;

        public int MaxIterationsCount;

        public int IterationsCountToIncreaseStep;

        public double StepDecreaseMultiplier;

        public double StepIncreaseMultiplier;

        public double PrecisionBySubgradient;
    }
}
