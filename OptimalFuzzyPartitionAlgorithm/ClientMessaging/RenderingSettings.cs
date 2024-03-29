﻿using System;

namespace OptimalFuzzyPartitionAlgorithm.ClientMessaging
{
    [Serializable]
    public class RenderingSettings
    {
        public bool AlwaysShowCentersInfo;

        public int IterationNumber;

        public bool DrawWithMistrustCoefficient;

        public bool DrawGrayscale;

        public double MistrustCoefficient;

        public int BorderWidth;

        public int PositionDigitsCount = 3;

        public bool ShowGradientInfo = false;
    }
}
