using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class NumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Validate(value, cultureInfo, out _);
        }

        public ValidationResult Validate(object data, CultureInfo cultureInfo, out double value)
        {
            var s = (string)data;

            if (!double.TryParse(s, out value))
                return new ValidationResult(false, "Введено нечислове значення.");

            return ValidationResult.ValidResult;
        }
    }
}