using System.Windows;

namespace OptimalFuzzyPartition.View.ValidationRules
{
    public class CornerPositionWrapper : DependencyObject
    {
        public static readonly DependencyProperty AnotherCornerValueProperty = DependencyProperty.Register(
                nameof(AnotherCornerValue),
                typeof(double),
                typeof(CornerPositionWrapper),
                new FrameworkPropertyMetadata(double.MaxValue));

        public double AnotherCornerValue
        {
            get => (double)GetValue(AnotherCornerValueProperty);
            set => SetValue(AnotherCornerValueProperty, value);
        }
    }
}