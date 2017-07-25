using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.TaskScheduler;
using NightNight;
using System.Collections.Generic;

namespace NightNight_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var ts = new TaskService())
            {
                var td = ts.NewTask();
                td.RegistrationInfo.Description = "Task Description XXX";
                td.Triggers.Add(new DailyTrigger { DaysInterval = 2 });
                td.Actions.Add(new ExecAction("notepad.exe", "c:\\dev\\projects\\NightNight\\LICENSEX"));
                ts.GetFolder("\\NightNight").RegisterTaskDefinition(@"TEST", td);


            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            var s = new Settings()
            {
                SetApplicationMode = false,
                DayThemeFile = null,
                NightThemeFile = null
            };

            var h = new TaskSchedulerHelper();
            h.ScheduleThemeChanges(s);
        }


        [TestMethod]
        public void TestMethod3()
        {
            var s = new Settings()
            {
                SetApplicationMode = true,
                DayThemeFile = null,
                NightThemeFile = null
            };

            var h = new TaskSchedulerHelper();
            h.ScheduleThemeChanges(s);
        }

    }
}
