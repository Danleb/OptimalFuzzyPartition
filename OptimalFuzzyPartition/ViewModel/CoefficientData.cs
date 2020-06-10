namespace OptimalFuzzyPartition.ViewModel
{
    public class CoefficientData
    {
        public int CenterIndex { get; set; }
        public int CenterNumber => CenterIndex + 1;
        public double Coefficient { get; set; }
    }
}