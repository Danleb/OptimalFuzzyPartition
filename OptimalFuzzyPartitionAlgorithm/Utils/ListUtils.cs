using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public static class ListUtils
    {
        public static void ResizeList<T>(this List<T> list, int newSize)
        {
            var delta = newSize - list.Count;

            if (delta < 0)
                list.RemoveRange(list.Count - delta, delta);
            else
                for (var i = 0; i < delta; i++)
                    list.Add(default);
        }
    }
}