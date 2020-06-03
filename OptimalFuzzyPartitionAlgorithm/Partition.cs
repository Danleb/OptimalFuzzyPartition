using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OptimalFuzzyPartitionAlgorithm
{
    public class Partition
    {
        public PartitionSettings Settings { get; }

        public List<IterationData> IterationDatas { get; private set; }

        public event Action<IterationData> NextIterationCalculated;

        public int CurrentIterationNumber { get; private set; }

        private int NextIterationNumber => CurrentIterationNumber++;

        //private Vector<double> ZeroCentersVector

        /// <summary>
        /// Список центров (точек-генераторов).
        /// </summary>
        public List<Vector<double>> Centers { get; private set; }

        public Partition(PartitionSettings partitionSettings)
        {
            Settings = partitionSettings;
        }

        public void CreatePartition()
        {
            InitializeStartData();
            DoIterations();
        }

        private void InitializeStartData()
        {
            CurrentIterationNumber = 0;
            IterationDatas = new List<IterationData>();

            if (Settings.IsCenterPlacingTask)
            {
                //задаем начальные значения центров - левый нижний угол
                Centers = new List<Vector<double>>(Settings.CentersCount);

                for (int i = 0; i < Settings.CentersCount; i++)
                {
                    Centers[i] = Settings.MinCorner;
                }
            }
        }

        private void DoIterations()
        {
            var zeroIterationData = new IterationData
            {
                IterationNumber = NextIterationNumber,
                Centers = Centers.Select(v => Vector<double>.Build.SparseOfVector(v)).ToList()
            };

            while (true)
            {
                var iterationData = CalculateNextStep();

                IterationDatas.Add(iterationData);

                NextIterationCalculated?.Invoke(iterationData);

                if (IsStopConditionsSatisfied(iterationData, IterationDatas[CurrentIterationNumber - 1]))
                    break;
            }
        }

        private IterationData CalculateNextStep()
        {
            var data = new IterationData();


            return data;
        }

        /// <summary>
        /// Удовлетворены ли условия выхода из итерационного цикла (условия окончания работы алгоритма).
        /// </summary>
        /// <returns></returns>
        private bool IsStopConditionsSatisfied(IterationData iterationData, IterationData previousIterationData)
        {
            var centerDeltas = CreateVector.Dense<double>(Settings.CentersCount);

            for (int i = 0; i < Settings.CentersCount; i++)
            {
                var previousCenter = previousIterationData.Centers[i];
                var currentCenter = iterationData.Centers[i];

                var distance = Settings.Distance(previousCenter, currentCenter);

                centerDeltas.Add(distance);
            }

            var delta = Settings.Distance(centerDeltas, Vector<double>.Build.Sparse(Settings.CentersCount));

            Trace.WriteLine($"Teta iterations delta = {delta}");

            return
                CurrentIterationNumber > Settings.MaxIterationsCount ||
                delta <= Settings.CentersDeltaEpsilon
                ;
        }
    }
}
