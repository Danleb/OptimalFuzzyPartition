using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;

namespace OFP_AlgorithmTests.AlgorithmTests
{
    public class IntegralCalculatorTests
    {
        private const double epsilon = 1e-6;

        [Test]
        public void _0x2_0x2_1_Test()
        {
            var minCorner = VectorUtils.CreateVector(0d, 0);
            var maxCorner = VectorUtils.CreateVector(2d, 2);
            var grid = VectorUtils.CreateVector(2, 2);
            double IntegralFunction(Vector<double> x) => 1;

            var integralCalculator = new IntegralCalculator(minCorner, maxCorner, grid, IntegralFunction);

            var res = integralCalculator.CalculateIntegral();

            Assert.AreEqual(4, res, epsilon);
        }

        [Test]
        public void _10x20_20x300_XxX_Test()
        {
            var minCorner = VectorUtils.CreateVector(-10d, -20);
            var maxCorner = VectorUtils.CreateVector(20d, 300);
            var grid = VectorUtils.CreateVector(200, 200);
            double IntegralFunction(Vector<double> point) => point[0] * point[0];

            var integralCalculator = new IntegralCalculator(minCorner, maxCorner, grid, IntegralFunction);

            var res = integralCalculator.CalculateIntegral();

            Assert.AreEqual(960_000, res, 40);
        }
    }
}