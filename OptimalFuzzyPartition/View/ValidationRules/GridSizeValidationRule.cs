using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class GridSizeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;

            var res = new IntegerValidationRule().Validate(value, cultureInfo);

            if (!res.IsValid)
                return res;

            var result = int.Parse(s);

            if (result <= 0)
                return new ValidationResult(false, "Розмір сітки повинен бути додатнім числом кратним 8.");

            if (result % 8 != 0)
                return new ValidationResult(false, "Через особливості реалізації, розмір сітки повинен бути кратним 8.");

            return ValidationResult.ValidResult;
        }
    }
}