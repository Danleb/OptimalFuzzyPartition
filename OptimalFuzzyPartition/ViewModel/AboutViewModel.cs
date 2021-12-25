using System.Reflection;

namespace OptimalFuzzyPartition.ViewModel
{
    public class AboutViewModel
    {
        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
