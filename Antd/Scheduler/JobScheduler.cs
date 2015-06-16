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
                            LauchJob<JobList.CommandJob>(task.Guid);
                        }
                    }
                }
            }
        }

        public static void Stop() {
            __scheduler.Shutdown();
        }

        public static void LauchJob<T>(string guid) where T : IJob {
            var _job = JobRepository.GetByGuid(guid);
            IJobDetail job = DefineJob<T>(_job);
            ITrigger trigger = DefineTrigger(_job.Trigger, _job.Alias);
            __scheduler.ScheduleJob(job, trigger);
        }

        private static IJobDetail DefineJob<T>(JobModel _job) where T : IJob {
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(_job.Alias, Guid.NewGuid().ToString())
                .UsingJobData("data", _job.Data)
                .UsingJobData("jobID", _job.Guid)
                .Build();
            return job;
        }

        private static ITrigger DefineTrigger(TriggerModel _trigger, string _identity) {
            ITrigger trigger;
            if (_trigger == null) {
                ConsoleLogger.Error("----- Scheduler :-(");
                ConsoleLogger.Error("Found a null value while defining a trigger");
                ConsoleLogger.Error("Trigger identity: {0}", _identity);
                ConsoleLogger.Error("Anyway, Antd can define a temporary trigger setting...");
                ConsoleLogger.Error("...and your task will be scheduled in a minute.");
                trigger = DefineStaticTrigger(_identity);
                ConsoleLogger.Error("But this error should not happen!");
                return trigger;
            }
            switch (_trigger.TriggerSetting) {
                case TriggerModel.TriggerPeriod.IsOneTimeOnly:
                    trigger = DefineOneTimeOnlyTrigger(_trigger, _identity);
                    break;

                case TriggerModel.TriggerPeriod.IsCron:
                    trigger = DefineCronTrigger(_trigger, _identity);
                    break;

                default:
                    trigger = DefineOneTimeOnlyTrigger(_trigger, _identity);
                    break;
            }
            return trigger;
        }

        private static ITrigger DefineStaticTrigger(string _identity) {
            ITrigger oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(DateTime.Now.AddMinutes(1))
                .Build();
            return oneTimeOnlyTrigger;
        }

        private static ITrigger DefineOneTimeOnlyTrigger(TriggerModel setting, string _identity) {
            ITrigger oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(setting.StartTime)
                .Build();
            return oneTimeOnlyTrigger;
        }

        private static ITrigger DefineCronTrigger(TriggerModel setting, string _identity) {
            ITrigger monthlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartAt(setting.StartTime)
                .WithCronSchedule(setting.CronExpression)
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