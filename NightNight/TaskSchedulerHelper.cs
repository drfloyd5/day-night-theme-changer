using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.TaskScheduler;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace NightNight
{
    public class TaskSchedulerHelper
    {

        public TaskSchedulerHelper()
        {
            Assembly currentAssem = Assembly.GetExecutingAssembly();


            _applicationExe = currentAssem.Location;
            _applicationDirectory = Path.GetDirectoryName(currentAssem.Location);
        }

        private string _applicationExe = null;
        private string _applicationDirectory = null;

        private Settings _settings = null;
        private TaskFolder _folder = null;
        public TaskFolder Folder
        {
            get
            {
                return _folder = _folder ?? TaskService.Instance.GetFolder(FolderPath);
            }
            set { _folder = value; }
        }

        string FolderName
        {
            get
            {
#if DEBUG
                return "NightNight_Debug";
#else
                return "NightNight";
#endif
            }
        }
        string FolderPath { get { return "\\" + FolderName; } }
        string DayTimeTaskName { get { return "NightNight Set Daytime Theme"; } }
        string NightTimeTaskName { get { return "NightNight Set Nighttime Theme"; } }
        string AutoTimeTaskName { get { return "NightNight Auto Set Theme"; } }

        public void ScheduleThemeChanges(Settings settings)
        {
            try
            {
                _settings = settings;
                var somethingMade = false;
                if (String.IsNullOrWhiteSpace(_settings.DayThemeFile) && _settings.SetApplicationMode == false)
                {
                    //Remove DayTime Setting
                    Folder?.DeleteTask(DayTimeTaskName, false);
                }
                else
                {
                    somethingMade = true;
                    MakeFolder();
                    Folder.RegisterTaskDefinition(DayTimeTaskName, DaytimeTask());
                }

                if (String.IsNullOrWhiteSpace(_settings.NightThemeFile) && _settings.SetApplicationMode == false)
                {
                    //Remove NightTime Setting
                    Folder?.DeleteTask(NightTimeTaskName, false);
                }
                else
                {
                    somethingMade = true;
                    MakeFolder();
                    Folder.RegisterTaskDefinition(NightTimeTaskName, NighttimeTask());
                }

                if (!somethingMade)
                {
                    Folder?.DeleteTask(AutoTimeTaskName, false);
                    TaskService.Instance.RootFolder.DeleteFolder(FolderPath, false);
                }
                else
                {
                    MakeFolder();
                    Folder.RegisterTaskDefinition(AutoTimeTaskName, AutoTimeTask());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void MakeFolder()
        {
            if (Folder == null)
            {
                TaskService.Instance.RootFolder.CreateFolder(FolderName);
            }
        }

        private TaskDefinition DaytimeTask()
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, now.Day, _settings.DayTimeStart.Hours, _settings.DayTimeStart.Minutes, _settings.DayTimeStart.Seconds);
            if (start < now)
                start.AddDays(1);

            var td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "Set Daytime Theme";
            td.Triggers.Add(new DailyTrigger()
            {
                DaysInterval = 1,
                StartBoundary = start
            });

            td.Actions.Add(_applicationExe, "-day", _applicationDirectory);
            return td;
        }

        private TaskDefinition NighttimeTask()
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, now.Day, _settings.NightTimeStart.Hours, _settings.NightTimeStart.Minutes, _settings.NightTimeStart.Seconds);
            if (start < now)
                start.AddDays(1);

            var td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "Set Nighttime Theme";
            td.Triggers.Add(new DailyTrigger()
            {
                DaysInterval = 1,
                StartBoundary = start
            });

            td.Actions.Add(_applicationExe, "-night", _applicationDirectory);
            return td;
        }


        private TaskDefinition AutoTimeTask()
        {
            var td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "Set Theme based on time of day";
            td.Triggers.Add(new LogonTrigger()
            {
                UserId = Environment.UserName
            });
            td.Triggers.Add(new SessionStateChangeTrigger(TaskSessionStateChangeType.SessionUnlock)
            {
                UserId = Environment.UserName
            });
            td.Actions.Add(_applicationExe, "-auto", _applicationDirectory);
            return td;
        }
    }
}
