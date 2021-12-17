using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OptimalFuzzyPartition
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static event EventHandler LanguageChanged;

        public static List<CultureInfo> Languages { get; }

        public static CultureInfo Language
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value == Thread.CurrentThread.CurrentUICulture)
                    return;

                Thread.CurrentThread.CurrentUICulture = value;

                var resDict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "en-US":
                        resDict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break;
                    default:
                        resDict.Source = new Uri(string.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                }

                var oldDict = Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang."))
                    .First();

                if (oldDict != null)
                {
                    var index = Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Current.Resources.MergedDictionaries.Remove(oldDict);
                    Current.Resources.MergedDictionaries.Insert(index, resDict);
                }
                else
                {
                    Current.Resources.MergedDictionaries.Add(resDict);
                }

                LanguageChanged?.Invoke(Current, new EventArgs());
            }
        }

        static App()
        {
            Languages = new List<CultureInfo>();
            Languages.Clear();
            var culturesCodes = new string[]
            {
                "en-US",
                "uk-UA",
                "ru-RU",
            };
            foreach (var code in culturesCodes)
            {
                Languages.Add(new CultureInfo(code));
            }
        }
    }
}
