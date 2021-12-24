using OptimalFuzzyPartition.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace OptimalFuzzyPartition
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ResourceDictionary s_stringsDictionary;

        static App()
        {
            LanguageChanged += App_LanguageChanged;

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

        App()
        {            
            InitializeComponent();
            Language = Settings.Default.SavedLanguage;
        }

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
                resDict.Source = new Uri($"Resources/StringLocalization.{value.Name}.xaml", UriKind.Relative);
                var oldDict = FindCurrentStringsDictionary();

                if (oldDict != null)
                {
                    Current.Resources.MergedDictionaries.Remove(oldDict);
                }

                s_stringsDictionary = resDict;
                Current.Resources.MergedDictionaries.Add(resDict);
                LanguageChanged?.Invoke(Current, new EventArgs());
            }
        }

        public static string Localize(string key)
        {
            return (string)s_stringsDictionary[key];
        }

        private static ResourceDictionary FindCurrentStringsDictionary()
        {
            return Current.Resources.MergedDictionaries
                                .Where(d => d?.Source?.OriginalString?.StartsWith("Resources/StringLocalization.") ?? false)
                                .FirstOrDefault();
        }

        private static void App_LanguageChanged(object sender, EventArgs e)
        {
            Settings.Default.SavedLanguage = Language;
            Settings.Default.Save();
        }
    }
}
