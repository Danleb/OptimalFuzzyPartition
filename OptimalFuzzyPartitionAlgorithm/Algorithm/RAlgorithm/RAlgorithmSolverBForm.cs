using MathNet.Numerics.LinearAlgebra;
using System;

namespace OptimalFuzzyPartitionAlgorithm.Algorithm
{
    /// <summary>
    /// Реализация алгоритма субградиентного метода оптимизации с растяжением пространства.
    /// Алгоритм реализован в так называемой B-форме.
    /// </summary>
    public class RAlgorithmSolverBForm
    {
        /// <summary>
        /// Количество измерений пространства.
        /// </summary>
        public readonly int DimensionsCount;

        /// <summary>
        /// Количество выполненных итераций.
        /// </summary>
        public int PreformedIterationsCount { private set; get; } = 0;

        /// <summary>
        /// Текущее, на данный момент приближение точки минимума.
        /// </summary>
        public Vector<double> x { private set; get; }

        /// <summary>
        /// Матрица растяжения пространства B.
        /// </summary>
        public Matrix<double> B;

        /// <summary>
        /// Шаговый множитель.
        /// </summary>
        public double h;

        /// <summary>
        /// Значение вектора градиента функции от х на текущей итерации.
        /// </summary>
        public Vector<double> g;

        /// <summary>
        /// Значение вектора градиента функции от х на предыдущей итерации.
        /// </summary>
        //public Vector<double> gPrevious;

        public Func<Vector<double>, Vector<double>> FunctionGradient { get; set; }

        /// <summary>
        /// Коэффициент растяжения пространства.
        /// </summary>
        public double a;

        /// <summary>
        /// Конструктор для инициализации начала работы r-алгоритма.
        /// </summary>
        /// <param name="initialX"></param>
        /// <param name="functionGradient"></param>
        /// <param name="a">Коэффициент растяжения пространства. Обычно берется из промежутка [2,3] </param>
        public RAlgorithmSolverBForm(Vector<double> initialX, Func<Vector<double>, Vector<double>> functionGradient, double a = 2)
        {
            DimensionsCount = initialX.Count;
            x = initialX;
            FunctionGradient = functionGradient;
            this.a = a;

            B = Matrix<double>.Build.Sparse(DimensionsCount, DimensionsCount);
            //делаем единичную матрицу B0.
            ResetBMatrixToUnit();

            //считаем градиент функции в точке x для первой итерации
            g = functionGradient(x);
        }

        /// <summary>
        /// Сбросить текущие значения матрицы B, сделать её единичной.
        /// </summary>
        public void ResetBMatrixToUnit()
        {
            B.CoerceZero(double.PositiveInfinity);

            for (var i = 0; i < DimensionsCount; i++)
                B[i, i] = 1;
        }

        public void DoIteration()
        {
            var Bt = B.Transpose();//считаем транспонированную матрицу B
            var Ksi = CalculateKsi(Bt, g);//считаем ξ (кси), единичный вектор направления растяжения пространства
            var x1 = x - h * B * Ksi;//делаем основной шаг итерации
            var g1 = FunctionGradient(x1);


            var r = CalculateR(g, g1);
            var eta = CalculateEta(Bt, r);
            var beta = 1d / a;
            var operatorR = CalculateOperatorR(eta, beta);
            var B1 = B * operatorR;

            //
            x = x1;
            B = B1;

            PreformedIterationsCount++;
        }

        /// <summary>
        /// Рассчитать ξ - кси.
        /// </summary>
        /// <returns></returns>
        private static Vector<double> CalculateKsi(Matrix<double> Bt, Vector<double> g)
        {
            var Bt_g = Bt * g;//умножаем транспонированную матрицу на вектор градиента
            var Bt_g_norm = Bt_g.L2Norm();//расчет нормы можно вынести для произвольной меры пространства
            var ksi = Bt_g / Bt_g_norm;//делим вектор на его норму - получаем единичный вектор
            return ksi;
        }

        /// <summary>
        /// Рассчитать η - эта.
        /// </summary>
        private static Vector<double> CalculateEta(Matrix<double> Bt, Vector<double> r)
        {
            var Bt_r = Bt * r;
            var norm = Bt_r.L2Norm();
            var eta = Bt_r / norm;
            return eta;
        }

        private static Vector<double> CalculateR(Vector<double> gPrevious, Vector<double> gNext)
        {
            var r = gNext - gPrevious;
            return r;
        }

        /// <summary>
        /// Рассчитать матрицу-оператор R растяжения пространства по бета.
        /// </summary>
        /// <param name="eta"></param>
        /// <returns></returns>
        private static Matrix<double> CalculateOperatorR(Vector<double> Ksi, double coef)
        {
            var DimensionsCount = Ksi.Count;

            var I = Matrix<double>.Build.Sparse(DimensionsCount, DimensionsCount);
            for (var i = 0; i < DimensionsCount; i++)
                I[i, i] = 1;

            var ksiColumn = Matrix<double>.Build.Sparse(DimensionsCount, DimensionsCount);
            for (var rowIndex = 0; rowIndex < DimensionsCount; rowIndex++)
                ksiColumn[rowIndex, 0] = Ksi[rowIndex];

            var ksiRow = Matrix<double>.Build.Sparse(DimensionsCount, DimensionsCount);
            for (var columnIndex = 0; columnIndex < DimensionsCount; columnIndex++)
                ksiRow[0, columnIndex] = Ksi[columnIndex];

            var operatorR = I + (coef - 1) * (ksiColumn * ksiRow);

            return operatorR;
        }
    }
}