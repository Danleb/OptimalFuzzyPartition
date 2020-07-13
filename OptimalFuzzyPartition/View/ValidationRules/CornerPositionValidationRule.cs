using System.Globalization;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class CornerPositionValidationRule : ValidationRule
    {
        public CornerType CornerType { get; set; }

        public CornerPositionWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object data, CultureInfo cultureInfo)
        {
            var res = new NumberValidationRule().Validate(data, cultureInfo, out var number);

            if (!res.IsValid)
                return res;

            switch (CornerType)
            {
                case CornerType.MinCorner when number > Wrapper.AnotherCornerValue:
                    return new ValidationResult(false, $"Ліва границя повинна бути меншою за праву. Ліва = {number}, права = {Wrapper.AnotherCornerValue}");
                case CornerType.MaxCorner when number < Wrapper.AnotherCornerValue:
                    return new ValidationResult(false, $"Права границя повинна бути більшою за ліву. Ліва = {Wrapper.AnotherCornerValue}, права = {number}");
                default:
                    return ValidationResult.ValidResult;
            }
        }
    }
}