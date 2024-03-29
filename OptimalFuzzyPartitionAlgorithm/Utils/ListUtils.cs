﻿using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public static class ListUtils
    {
        public static void ResizeList<T>(this List<T> list, int newSize, Func<T> getT = null)
        {
            var delta = newSize - list.Count;

            if (delta < 0)
                list.RemoveRange(list.Count + delta, -delta);
            else
                for (var i = 0; i < delta; i++)
                    list.Add(getT == null ? default : getT());
        }

        public static T MinElement<T>(this List<T> list, Func<T, double> func)
        {
            double minValue = double.MaxValue;
            T minElement = default;
            foreach (var v in list)
            {
                var value = func(v);
                if (value < minValue)
                {
                    minValue = value;
                    minElement = v;
                }
            }
            return minElement;
        }
    }
}