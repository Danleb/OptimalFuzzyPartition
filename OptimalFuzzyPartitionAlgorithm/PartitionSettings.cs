using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace OptimalFuzzyPartitionAlgorithm
{
    public class PartitionSettings
    {
        /// <summary>
        /// Количество измерений пространства, в котором будет задано разбиение.
        /// </summary>
        public int DimensionsCount => MinCorner.Count;

        /// <summary>
        /// Левый нижний угол прямоугольной области, которая будет разбита.
        /// </summary>
        public Vector<double> MinCorner;

        /// <summary>
        /// Правый верхний угол прямоугольной области, которая будет разбита.
        /// </summary>
        public Vector<double> MaxCorner;

        /// <summary>
        /// Количество сегментов разбиений области определения, сетка. Например [10,20] - мы разбиваем область по oX на 10 отрезков, и на 20 отрезков по oY.
        /// </summary>
        public Vector<int> GridSize;

        /// <summary>
        /// Максимальное количество итераций. Алгоритм прекращается, если количество итераций превысило максимально допустимое, а заданная точность еще не достигнута.
        /// </summary>
        public int MaxIterationsCount;

        /// <summary>
        /// Эпсилон для центров (точек-генераторов). Одно из условий прекращения работы алгоритма - если расстояние между центрами на i и i+1 итерациях отличается меньше чем на эпсилон.
        /// </summary>
        public double CentersDeltaEpsilon;

        /// <summary>
        /// Количество центров (точек-генераторов).
        /// </summary>
        public int CentersCount;

        /// <summary>
        /// Это задача с размещением центров? Если задача с размещением центров, тогда будут выбраны оптимальные точки для расположения центров.
        /// </summary>
        public bool IsCenterPlacingTask;

        /// <summary>
        /// Список позиций центров. Если задача без размещения центров, тогда эти точки будут взяты как координаты центров.
        /// </summary>
        public List<Vector<double>> CenterPositions;

        /// <summary>
        /// Начальный шаг r-алгоритма.
        /// </summary>
        public double H0;

        /// <summary>
        /// Мультипликативные коэффициенты w[i]
        /// </summary>
        public List<double> MultiplicativeCoefficients;

        /// <summary>
        /// Аддитивные коэффициенты a[i].
        /// </summary>
        public List<double> AdditiveCoefficients;

        /// <summary>
        /// Параметр m - экспоненциальный вес. Это степень, в которую возводится функция принадлежности.
        /// </summary>
        public int ExponentialWeight;

        /// <summary>
        /// Функция расстояния между двумя точками. В описании алгоритма используется как c(x, тета).
        /// </summary>
        public Func<Vector<double>, Vector<double>, double> Distance;

        /// <summary>
        /// Функция плотности "ро" - ρ(x). Возвращает значение плотности пространства в его точке. Может интерпретироваться как мощность потребления в данной точке.
        /// </summary>
        public Func<Vector<double>, double> Density;
    }
}