namespace OptimalFuzzyPartition.ViewModel
{
    /// <summary>
    /// Type of the space metrics (space norm).
    /// </summary>
    public enum MetricsType
    {
        /// <summary>
        /// Euclidean (out default metrics).
        /// </summary>
        Euclidean,

        /// <summary>
        /// Manhattan metrics (L1) (city blocks distance).
        /// </summary>
        Manhattan,

        /// <summary>
        /// Chebyshev metrics (max(x(i) - y(i)).
        /// </summary>
        Chebyshev,

        /// <summary>
        /// Custom metrics type specified by function.
        /// </summary>
        CustomFunction
    }
}