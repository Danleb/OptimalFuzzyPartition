using MathNet.Numerics.LinearAlgebra;
using OptimalFuzzyPartitionAlgorithm.Utils;
using UnityEngine;

namespace Utils
{
    public static class VectorExtensions
    {
        public static Vector<double> ToVector(this Vector2 vector2)
        {
            return VectorUtils.CreateVector(vector2.x, vector2.y);
        }

        public static Vector2 ToVector2(this Vector<double> vector)
        {
            return new Vector2((float)vector[0], (float)vector[1]);
        }

        public static float[] ToArray(this Vector2 vector2)
        {
            return new[] { vector2.x, vector2.y };
        }
    }
}
