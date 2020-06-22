using System;
using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class GridSizeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string) value;

            if(!int.TryParse(s, out var result))
                return new ValidationResult(false, "Введено нечислове значення. Введіть число.");

            if(result <= 0)
                return new ValidationResult(false, "Розмір сітки повинен бути цілим числом більше нуля.");

            //if(result % 8 != 0)
                //return new ValidationResult("Через особливості реалізації");

            return ValidationResult.ValidResult;
        }
    }
}