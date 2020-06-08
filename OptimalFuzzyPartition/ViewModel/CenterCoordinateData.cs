namespace OptimalFuzzyPartition.ViewModel
{
    public class CenterCoordinateData
    {
        public int CenterIndex { get; set; }
        public int CenterNumber => CenterIndex + 1;
        public double X { get; set; }
        public double Y { get; set; }
    }
}