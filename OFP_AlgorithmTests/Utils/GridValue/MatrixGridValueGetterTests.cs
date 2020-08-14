using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using OptimalFuzzyPartitionAlgorithm.Algorithm;

namespace OptimalFuzzyPartitionAlgorithmTests.Utils.GridValue
{
    [TestFixture]
    public class MatrixGridValueGetterTests
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

            var getter = new MatrixGridValueGetter(matrix);

            Assert.AreEqual(1, getter.GetValue(0, 0));
            Assert.AreEqual(2, getter.GetValue(0, 1));
            Assert.AreEqual(3, getter.GetValue(0, 2));
            Assert.AreEqual(4, getter.GetValue(1, 0));
            Assert.AreEqual(5, getter.GetValue(1, 1));
            Assert.AreEqual(6, getter.GetValue(1, 2));
            Assert.AreEqual(7, getter.GetValue(2, 0));
            Assert.AreEqual(8, getter.GetValue(2, 1));
            Assert.AreEqual(9, getter.GetValue(2, 2));
        }
    }
}