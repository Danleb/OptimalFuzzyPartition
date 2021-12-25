using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithmTests.Utils.GridValue
{
    [TestFixture]
    public class GridValueInterpolatorTests
    {
        [Test]
        public void Test1()
        {
            var matrix = Matrix<double>.Build.SparseOfArray(new double[,]
            {
                { 1,2,3 },
                { 4,5,6 },
                { 7,8,9 },
            });

            var spaceSettings = new SpaceSettings
            {
                GridSize = new List<int> { 3, 3 },
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(10, 10),
                MetricsType = MetricsType.Euclidean,
                DensityType = DensityType.Everywhere1
            };

            var getter = new MatrixGridValueGetter(matrix);
            var interpolator = new GridValueInterpolator(spaceSettings, getter);

            Assert.AreEqual(1, interpolator.GetGridValueAtPoint(0, 0));
            Assert.AreEqual(2, interpolator.GetGridValueAtPoint(5, 0));
            Assert.AreEqual(3, interpolator.GetGridValueAtPoint(10, 0));
            Assert.AreEqual(4, interpolator.GetGridValueAtPoint(0, 5));
            Assert.AreEqual(5, interpolator.GetGridValueAtPoint(5, 5));
            Assert.AreEqual(6, interpolator.GetGridValueAtPoint(10, 5));
            Assert.AreEqual(7, interpolator.GetGridValueAtPoint(0, 10));
            Assert.AreEqual(8, interpolator.GetGridValueAtPoint(5, 10));
            Assert.AreEqual(9, interpolator.GetGridValueAtPoint(10, 10));
            Assert.AreEqual(9, interpolator.GetGridValueAtPoint(10, 10));


            //#TODO add more cases
        }
    }
}
