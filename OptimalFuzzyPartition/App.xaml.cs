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
            ReplaceDictionary("Theme.xaml", Settings.Default.Theme);
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
                var newDictionary = ReplaceDictionary("Resources/StringLocalization.", $"Resources/StringLocalization.{value.Name}.xaml");

                s_stringsDictionary = newDictionary;
                LanguageChanged?.Invoke(Current, new EventArgs());
            }
        }

        public static string Theme
        {
            get => Settings.Default.Theme;
            set
            {
                var newTheme = value;
                ReplaceDictionary("Theme.xaml", newTheme);
                Settings.Default.Theme = newTheme;
                Settings.Default.Save();
            }
        }

        public static string Localize(string key)
        {
            return (string)s_stringsDictionary[key];
        }

        public static ResourceDictionary ReplaceDictionary(string oldSubstring, string newDictionary)
        {
            var resDict = new ResourceDictionary
            {
                Source = new Uri(newDictionary, UriKind.Relative)
            };
            var oldDict = FindDictionary(oldSubstring);

            if (oldDict != null)
            {
                Current.Resources.MergedDictionaries.Remove(oldDict);
            }

            Current.Resources.MergedDictionaries.Add(resDict);
            return resDict;
        }

        private static ResourceDictionary FindDictionary(string substring)
        {
            return Current.Resources.MergedDictionaries
                                .Where(d => d?.Source?.OriginalString?.Contains(substring) ?? false)
                                .FirstOrDefault();
        }

        private static void App_LanguageChanged(object sender, EventArgs e)
        {
            Settings.Default.SavedLanguage = Language;
            Settings.Default.Save();
        }
    }
}
