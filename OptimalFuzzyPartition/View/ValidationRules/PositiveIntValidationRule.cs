using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class PositiveIntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;

            var res = new IntegerValidationRule().Validate(value, cultureInfo);

            if (!res.IsValid)
                return res;

            var result = int.Parse(s);

            if(result <= 0)
                return new ValidationResult(false, "Введене число не більше нуля. Введіть ціле число більше нуля");

            return ValidationResult.ValidResult;
        }
    }
}