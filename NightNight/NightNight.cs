using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightNight
{

    public enum ThemeModes
    {
        Auto, DayTime, NightTime
    }

    public enum ApplicationModeSetting
    {
        GUI, CLI
    }

    public class ThemeChanger
    {
        public Settings Settings { get; set; }

        public ThemeChanger() { }

        public void ApplyBasedOnTimeOfDay()
        {
            var time = DateTime.Now.TimeOfDay;
            if (time >= Settings.DayTimeStart && time <= Settings.NightTimeStart)
                ApplyDay();
            else
                ApplyNight();
        }

        public void ApplyDay()
        {
            SetTheme(Settings.DayThemeFile, ThemeModes.DayTime);
        }

        public void ApplyNight()
        {
            SetTheme(Settings.NightThemeFile, ThemeModes.NightTime);
        }

        public void SetTheme(string themeFile, ThemeModes lightMode)
        {
            if (Settings.SetApplicationMode)
                Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", lightMode == ThemeModes.DayTime ? 1 : 0, RegistryValueKind.DWord);
            if (!string.IsNullOrEmpty(themeFile))
            {
                var si = new ProcessStartInfo("ThemeSwitcher.exe", $"\"{themeFile}\"");
                Process.Start(si);
            }
        }
    }
}
