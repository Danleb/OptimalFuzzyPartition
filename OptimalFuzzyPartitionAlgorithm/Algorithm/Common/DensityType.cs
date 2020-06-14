namespace OptimalFuzzyPartitionAlgorithm.Algorithm.Common
{
    /// <summary>
    /// Space density type.
    /// </summary>
    public enum DensityType
    {
        /// <summary>
        /// Space have the same density in all its points.
        /// </summary>
        Everywhere1,

        /// <summary>
        /// Density specified by function.
        /// </summary>
        CustomFunction,

        /// <summary>
        /// Density specified by the grid of numbers.
        /// Density in between points is interpolated.
        /// </summary>
        ByPointsGrid
    }
}