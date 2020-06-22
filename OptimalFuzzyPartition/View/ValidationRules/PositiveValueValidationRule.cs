using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class PositiveValueValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string) value;

            if(!double.TryParse(s, out var result))
                return new ValidationResult(false, "Введено нечислове значення. Введіть число більше нуля");


            return ValidationResult.ValidResult;
        }
    }
}