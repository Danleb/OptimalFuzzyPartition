using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.ViewModelUtils
{
    public static class Utils
    {
        public static bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
                   LogicalTreeHelper.GetChildren(obj)
                       .OfType<DependencyObject>()
                       .All(IsValid);
        }
    }
}