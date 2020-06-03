using System;

namespace OptimalFuzzyPartitionAlgorithm.Validators
{
    /// <summary>
    /// Вспомогательный класс для проверки условий разбиения на корректность.
    /// </summary>
    public class PartitionSettingsValidator
    {
        private PartitionSettings Settings { get; }

        public PartitionSettingsValidator(PartitionSettings settings)
        {
            Settings = settings;
        }

        public void Check()
        {
            if (Settings.H0 <= 0)
                throw new ArgumentException($"Начальный шаг H0 должен быть положительным. H0={Settings.H0}.");

            if (Settings.CentersCount <= 0)
                throw new ArgumentException("Количество центров должно быть положительным числом.");

            if (Settings.CenterPositions.Count != Settings.CentersCount)
                throw new ArgumentException($"Заданное количество координат центров {Settings.CenterPositions.Count} не равняется заданному количеству центров {Settings.CentersCount}.");


        }
    }
}