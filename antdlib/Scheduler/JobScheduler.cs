//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using Quartz;
using Quartz.Impl;
using System;
using System.Linq;
using antdlib.Log;

namespace antdlib.Scheduler {

    public class JobScheduler {
        private static readonly IScheduler Scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public static void Start(bool recoverTasks) {
            if (recoverTasks == false) {
                Scheduler.Start();
            }
            else {
                Scheduler.Start();
                var taskList = JobRepository.GetEnabled();
                ConsoleLogger.Log($"{taskList.ToArray().Length} job(s) scheduled");
                if (taskList.ToArray().Length <= 0) return;
                foreach (var task in taskList.Where(task => task != null)) {
                    LaunchJob<JobList.CommandJob>(task.Guid);
                }
            }
        }

        public static void Stop() {
            Scheduler.Shutdown();
        }

        public static void Test() {
            const string cron = "0/20 * * * * ?";
            var task = JobBuilder.Create<JobList.HelloJob>()
                .WithIdentity("test", Guid.NewGuid().ToString())
                .UsingJobData("jobID", "yo")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("ciao", Guid.NewGuid().ToString())
                .StartAt(DateTime.Now.AddMinutes(1));

            var i = trigger.WithSchedule(CronScheduleBuilder.CronSchedule(cron)).Build();

            Scheduler.ScheduleJob(task, i);
            Start(false);
        }

        public static void LaunchJob<T>(string guid) where T : IJob {
            var task = JobRepository.GetByGuid(guid);
            var dbtask = JobBuilder.Create<T>()
                .WithIdentity(task.Alias, Guid.NewGuid().ToString())
                .UsingJobData("data", task.Data)
                .UsingJobData("jobID", task.Guid)
                .Build();
            ITrigger trigger;
            switch (task.TriggerPeriod) {
                case TriggerPeriod.IsOneTimeOnly:
                    trigger = DefineOneTimeOnlyTrigger(task.Alias, task.StartTime);
                    break;
                case TriggerPeriod.IsCron:
                    trigger = DefineCronTrigger(task.Alias, task.StartTime, task.CronExpression);
                    break;
                case TriggerPeriod.WithInterval:
                    trigger = DefineIntervalTrigger(task.Alias, task.StartTime, task.IntervalType, task.IntervalSpan, task.Count);
                    break;
                case TriggerPeriod.Other:
                    trigger = DefineDefaultTrigger(task.Alias);
                    break;
                default:
                    trigger = DefineDefaultTrigger(task.Alias);
                    break;
            }
            Scheduler.ScheduleJob(dbtask, trigger);
        }

        private static ITrigger DefineDefaultTrigger(string identity) {
            var oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(identity, Guid.NewGuid().ToString())
                .StartAt(DateTime.Now.AddMinutes(1))
                .Build();
            return oneTimeOnlyTrigger;
        }

        private static ITrigger DefineOneTimeOnlyTrigger(string identity, DateTime startTime) {
            var oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(identity, Guid.NewGuid().ToString())
                .StartAt(startTime)
                .Build();
            return oneTimeOnlyTrigger;
        }

        private static ITrigger DefineCronTrigger(string identity, DateTime startTime, string cronEx) {
            var monthlyTrigger = TriggerBuilder.Create()
                .WithIdentity(identity, Guid.NewGuid().ToString())
                .StartAt(startTime)
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronEx))
                .Build();
            return monthlyTrigger;
        }

        private static ITrigger DefineIntervalTrigger(string identity, DateTime startTime, IntervalType intervalType, int span, int count) {
            var trigger = TriggerBuilder.Create()
                .WithIdentity(identity, Guid.NewGuid().ToString());
            trigger.StartAt(startTime);
            switch (intervalType) {
                case IntervalType.Hourly:
                    trigger.WithSchedule(count == 0
                        ? SimpleScheduleBuilder.RepeatHourlyForever(span)
                        : SimpleScheduleBuilder.RepeatHourlyForTotalCount(count, span));
                    break;
                case IntervalType.Minutely:
                    trigger.WithSchedule(count == 0
                        ? SimpleScheduleBuilder.RepeatMinutelyForever(span)
                        : SimpleScheduleBuilder.RepeatMinutelyForTotalCount(count, span));
                    break;
                case IntervalType.Secondly:
                    trigger.WithSchedule(count == 0
                        ? SimpleScheduleBuilder.RepeatSecondlyForever(span)
                        : SimpleScheduleBuilder.RepeatSecondlyForTotalCount(count, span));
                    break;
                case IntervalType.None:
                    break;
                default:
                    break;
            }
            return trigger.Build();
        }
    }
}