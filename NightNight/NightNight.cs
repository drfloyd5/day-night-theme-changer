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
        Auto, Light, Dark
    }

    public enum ApplicationModeSetting
    {
        GUI, CLI
    }

    public class ThemeChanger
    {
        public Settings Settings { get; set; }

        public ThemeChanger() { }

        public void SetAuto()
        {
            var time = DateTime.Now.TimeOfDay;
            if (time >= Settings.DayTimeStart && time <= Settings.NightTimeStart)
                SetLight();
            else
                SetDark();
        }

        public void SetLight()
        {
            SetTheme(Settings.DayThemeFile, ThemeModes.Light);
        }

        public void SetDark()
        {
            SetTheme(Settings.NightThemeFile, ThemeModes.Dark);
        }

        public void SetTheme(string themeFile, ThemeModes lightMode)
        {
            if (Settings.SetApplicationMode)
                Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", lightMode == ThemeModes.Light ? 1 : 0, RegistryValueKind.DWord);
            if (!string.IsNullOrEmpty(themeFile))
            {
                var si = new ProcessStartInfo("ThemeSwitcher.exe", $"\"{themeFile}\"");
                Process.Start(si);
            }
        }
    }
}
