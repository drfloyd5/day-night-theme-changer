using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NightNight
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = SettingsManager.LoadSettings();

            var cl = new CommandLineOptions(args);

            switch (cl.ApplicationMode)
            {
                case ApplicationModeSetting.GUI:

                    var f = new SettingsForm()
                    {
                        Settings = settings
                    };
                    Application.Run(f);
                    break;
                case ApplicationModeSetting.CLI:
                    var themeChanger = new ThemeChanger()
                    {
                        Settings = settings
                    };
                    switch (cl.ThemeMode)
                    {
                        case ThemeModes.Auto:
                            themeChanger.ApplyBasedOnTimeOfDay();
                            break;
                        case ThemeModes.NightTime:
                            themeChanger.ApplyNight();
                            break;
                        case ThemeModes.DayTime:
                            themeChanger.ApplyDay();
                            break;
                    }
                    break;
            }
        }


    }

    public class SettingsManager
    {
        public static Version MostRecentVersion { get { return new Version(1, 0, 0, 0); } }
        public Version Version { get; set; }

        public static Settings LoadSettings()
        {

            var settingsFilePath = SettingsFilePath();
            if (File.Exists(settingsFilePath))
            {
                var json = File.ReadAllText(settingsFilePath);
                try
                {
                    var p = JsonConvert.DeserializeObject<SettingsManager>(json);
                    if (p.Version == SettingsManager.MostRecentVersion)
                        return JsonConvert.DeserializeObject<Settings>(json);
                }
                catch (Exception) { /* empty */ }
            }
            return new Settings();
        }

        public virtual void SaveSettings() { }

        protected static string SettingsFilePath()
        {
#if DEBUG
            return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "settings.json");
#else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "The Grinning Wizards\\NiteNite", "settings.json");
#endif
        }
    }


    public class CommandLineOptions
    {
        public CommandLineOptions(string[] args)
        {
            if (args.Length == 0)
            {
                ApplicationMode = ApplicationModeSetting.GUI;
                return;
            }

            ApplicationMode = ApplicationModeSetting.CLI;
            e = args.GetEnumerator();
            string p;
            while ((p = Next()) != null)
            {
                switch (p.ToLower())
                {

                    case "-day":
                        ThemeMode = ThemeModes.DayTime;
                        break;
                    case "-night":
                        ThemeMode = ThemeModes.NightTime;
                        break;
                    case "-auto":
                        ThemeMode = ThemeModes.Auto;
                        break;
                    default:
                        throw new ArgumentException($"unrecognized argument \"{p}\".");
                }
            }
        }

        public ApplicationModeSetting ApplicationMode { get; private set; }
        public ThemeModes ThemeMode { get; private set; }
        private IEnumerator e;

        private string Next()
        {
            if (e == null) return null;
            if (e.MoveNext())
                return e.Current as string;
            return null;
        }
    }

    public class Settings_1_0_0_0 : SettingsManager
    {
        public string DayThemeFile { get; set; }
        public string NightThemeFile { get; set; }
        public TimeSpan DayTimeStart { get; set; }
        public TimeSpan NightTimeStart { get; set; }
        public Boolean SetApplicationMode { get; set; }

        public Settings_1_0_0_0()
        {
            DayThemeFile = "";
            NightThemeFile = "";
            DayTimeStart = new TimeSpan(7, 0, 0);
            NightTimeStart = new TimeSpan(20, 0, 0);
            SetApplicationMode = true;
            Version = new Version(1, 0, 0, 0);
        }
    }

    public class Settings : Settings_1_0_0_0
    {
        public Settings() : base() { }

        public override void SaveSettings()
        {
            base.SaveSettings();
            var settingsFilePath = SettingsFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
            var t = new TaskSchedulerHelper();
            t.ScheduleThemeChanges(this);
        }
    }

}
