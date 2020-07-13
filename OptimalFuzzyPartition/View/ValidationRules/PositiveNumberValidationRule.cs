using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;

            var res = new NumberValidationRule().Validate(value, cultureInfo);

            if (!res.IsValid)
                return res;

            var v = double.Parse(s);

            if (v <= 0)
                return new ValidationResult(false, "Введене число не є додатнім. Введіть додатнє значення.");

            return ValidationResult.ValidResult;
        }
    }
}