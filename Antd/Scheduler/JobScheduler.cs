///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Common;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;

namespace Antd.Scheduler {

    public class JobScheduler {
        private static IScheduler __scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public static void Start(bool _recoverTasks) {
            if (_recoverTasks == false) {
                __scheduler.Start();
            }
            else {
                __scheduler.Start();
                List<JobModel> taskList = JobRepository.GetEnabled();
                ConsoleLogger.Log("{0} job(s) scheduled", taskList.ToArray().Length);
                if (taskList.ToArray().Length > 0) {
                    foreach (JobModel task in taskList) {
                        if (task != null) {
                            LaunchJob<JobList.CommandJob>(task.Guid);
                        }
                    }
                }
            }
        }

        public static void Stop() {
            __scheduler.Shutdown();
        }

        public static void Test() {
            var cron = "0/20 * * * * ?";
            IJobDetail task = JobBuilder.Create<JobList.HelloJob>()
                .WithIdentity("test", Guid.NewGuid().ToString())
                .UsingJobData("jobID", "yo")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("ciao", Guid.NewGuid().ToString())
                .StartAt(DateTime.Now.AddMinutes(1));
                //.WithSchedule(CronScheduleBuilder.CronSchedule(cron))
                //.WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever())
                //.WithCronSchedule(cron)
                //.Build();

            var i = trigger.WithSchedule(CronScheduleBuilder.CronSchedule(cron)).Build();

            __scheduler.ScheduleJob(task, i);
        }

        public static void LaunchJob<T>(string guid) where T : IJob {
            var _task = JobRepository.GetByGuid(guid);
            IJobDetail task = JobBuilder.Create<T>()
                .WithIdentity(_task.Alias, Guid.NewGuid().ToString())
                .UsingJobData("data", _task.Data)
                .UsingJobData("jobID", _task.Guid)
                .Build();

            ITrigger trigger;
            //if (_task.TriggerPeriod == null) {
            //    ConsoleLogger.Warn("----- Scheduler :-(");
            //    ConsoleLogger.Warn("Found a null value while defining a trigger");
            //    ConsoleLogger.Warn("Trigger identity: {0}", _identity);
            //    ConsoleLogger.Warn("Anyway, Antd can define a temporary trigger setting...");
            //    ConsoleLogger.Warn("...and your task will be scheduled in a minute.");
            //    trigger = DefineStaticTrigger(_identity);
            //    ConsoleLogger.Warn("But this error should not happen!");
            //    ConsoleLogger.Warn("----- Scheduler :-(");
            //    return trigger;
            //}
            switch (_task.TriggerPeriod) {
                case TriggerPeriod.IsOneTimeOnly:
                    trigger = DefineOneTimeOnlyTrigger(_task.Alias, _task.StartTime);
                    break;

                case TriggerPeriod.IsCron:
                    trigger = DefineCronTrigger(_task.Alias, _task.StartTime, _task.CronExpression);
                    break;

                default:
                    trigger = DefineStaticTrigger(_task.Alias);
                    break;
            }
            __scheduler.ScheduleJob(task, trigger);
        }

        private static ITrigger DefineTrigger(string _identity) {
            var cron = "";
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(DateTime.Now.AddMinutes(1))
                .WithSchedule(CronScheduleBuilder.CronSchedule(cron))
                .Build();
            return trigger;
        }

        private static ITrigger DefineStaticTrigger(string _identity) {
            ITrigger oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(DateTime.Now.AddMinutes(1))
                .Build();
            return oneTimeOnlyTrigger;
        }

        private static ITrigger DefineOneTimeOnlyTrigger(string _identity, DateTime _startTime) {
            ITrigger oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(_startTime)
                .Build();
            return oneTimeOnlyTrigger;
        }

        private static ITrigger DefineCronTrigger(string _identity, DateTime _startTime, string _cronEx) {
            ITrigger monthlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(_startTime)
                .WithSchedule(CronScheduleBuilder.CronSchedule(_cronEx))
                .Build();
            return monthlyTrigger;
        }

        //private static ITrigger DefineDailyTrigger(TriggerModel setting, string _identity) {
        //    ITrigger dailyTrigger = TriggerBuilder.Create()
        //        .WithIdentity(_identity, Guid.NewGuid().ToString())
        //        .StartAt(setting.StartTime)
        //        .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromDays(setting.TimeSpan)))
        //        .EndAt(setting.EndTime)
        //        .Build();
        //    return dailyTrigger;
        //}

        //private static ITrigger DefineWeeklyTrigger(TriggerModel setting, string _identity) {
        //    int _weeklyHour = setting.StartTime.Hour;
        //    int _weeklyMinute = setting.StartTime.Minute;
        //    ITrigger weeklyTrigger = TriggerBuilder.Create()
        //        .WithIdentity(_identity, Guid.NewGuid().ToString())
        //        .StartAt(setting.StartTime)
        //        .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(setting.DayOfTheWeek, _weeklyHour, _weeklyMinute))
        //        .EndAt(setting.EndTime)
        //        .Build();
        //    return weeklyTrigger;
        //}

        //private static ITrigger DefineMonthlyTrigger(TriggerModel setting, string _identity) {
        //    //string _cronExpression = "0 0/2 8-17 * * ?";
        //    ITrigger monthlyTrigger = TriggerBuilder.Create()
        //        .WithIdentity(_identity, Guid.NewGuid().ToString())
        //        .StartAt(setting.StartTime)
        //        .WithCronSchedule(setting.CronExpression)
        //        .EndAt(setting.EndTime)
        //        .Build();
        //    return monthlyTrigger;
        //}
    }
}