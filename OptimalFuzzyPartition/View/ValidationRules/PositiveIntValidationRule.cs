using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class PositiveIntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;

            if (!int.TryParse(s, out var result))
                return new ValidationResult(false, "Введено не ціле число. Введіть ціле число більше нуля.");

            if(result <= 0)
                return new ValidationResult(false, "Введене число не більше нуля. Введіть ціле число більше нуля");

            return ValidationResult.ValidResult;
        }
    }
}