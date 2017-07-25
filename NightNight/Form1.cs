using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NightNight
{


    public partial class SettingsForm : Form
    {
        private Settings _settings = null;
        public Settings Settings
        {
            get { return _settings; }
            set {
                _settings = value;

                if (_settings.SetApplicationMode) { radioButton1.Checked = true; } else { radioButton2.Checked = true; }
                var now = DateTime.Now;

                dayTimeStartPicker.Value = new DateTime(now.Year, now.Month, now.Day, _settings.DayTimeStart.Hours, _settings.DayTimeStart.Minutes, _settings.DayTimeStart.Seconds);

                nightTimeStartPicker.Value = new DateTime(now.Year, now.Month, now.Day, _settings.NightTimeStart.Hours, _settings.NightTimeStart.Minutes, _settings.NightTimeStart.Seconds);
                daytimeThemeTextBox.Text = _settings.DayThemeFile;
                nighttimeThemeTextBox.Text = _settings.NightThemeFile;

                _themeChanger.Settings = _settings;
            }
        }

        private ThemeChanger _themeChanger;

        public SettingsForm()
        {
            InitializeComponent();
            _themeChanger = new ThemeChanger
            {
                Settings = Settings
            };
        }

        private void previewLight_Click(object sender, EventArgs e)
        {
            _themeChanger.SetLight();
        }

        private void previewDark_Click(object sender, EventArgs e)
        {
            _themeChanger.SetDark();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void browseForDayThemeFile(object sender, EventArgs e)
        {

            var r = browseForTheme(Settings.DayThemeFile);
            if (r == DialogResult.OK)
            {
                daytimeThemeTextBox.Text = openFileDialog1.FileName;
                Settings.DayThemeFile = openFileDialog1.FileName;
            }
        }

        private DialogResult browseForTheme(string themeFile)
        {
            if (string.IsNullOrWhiteSpace(themeFile))
            {
                openFileDialog1.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Windows\\Themes");
            }
            else
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(themeFile);
                openFileDialog1.FileName = themeFile;
            }
            return openFileDialog1.ShowDialog();
        }

        private void browseForNightThemeFile(object sender, EventArgs e)
        {
            var r = browseForTheme(Settings.NightThemeFile);
            if (r == DialogResult.OK)
            {
                nighttimeThemeTextBox.Text = openFileDialog1.FileName;
                Settings.NightThemeFile = openFileDialog1.FileName;
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SetApplicationMode = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SetApplicationMode = false;
        }

        private void close_Click(object sender, EventArgs e)
        {
            Settings.SaveSettings();
            _themeChanger.SetAuto();
            Close();
        }

        private void apply_Click(object sender, EventArgs e)
        {
            _themeChanger.SetAuto();
        }

        private void lightTimePicker_ValueChanged(object sender, EventArgs e)
        {
            _settings.DayTimeStart = (sender as DateTimePicker).Value.TimeOfDay;
        }

        private void darkTimePicker_ValueChanged(object sender, EventArgs e)
        {
            _settings.NightTimeStart = (sender as DateTimePicker).Value.TimeOfDay;
        }

        private void daytimeThemeTextBox_TextChanged(object sender, EventArgs e)
        {
            _settings.DayThemeFile = (sender as TextBox).Text;
        }

        private void nighttimeThemeTextBox_TextChanged(object sender, EventArgs e)
        {
            _settings.NightThemeFile = (sender as TextBox).Text;
        }
    }
}
