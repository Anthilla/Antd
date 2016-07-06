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

using System;
using System.Linq;
using antdlib.common;
using Antd.Database;
using Quartz;
using Quartz.Impl;

namespace Antd.Scheduler {

    public class JobScheduler {
        private static readonly IScheduler Scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public static void Start(bool recoverTasks) {
            if (recoverTasks == false) {
                Scheduler.Start();
            }
            else {
                Scheduler.Start();
                var taskList = new JobRepository().GetAll().ToList();
                ConsoleLogger.Log($"{taskList.ToArray().Length} job(s) scheduled");
                if (taskList.ToArray().Length <= 0) return;
                foreach (var task in taskList.Where(task => task != null)) {
                    LaunchJob<Command>(task.Guid);
                }
            }
        }

        public static void Stop() {
            Scheduler.Shutdown();
        }

        public static void LaunchJob<T>(string guid) where T : IJob {
            var task = new JobRepository().GetByGuid(guid);
            var dbtask = JobBuilder.Create<T>()
                .WithIdentity(task.Alias, Guid.NewGuid().ToString())
                .UsingJobData("data", task.Data)
                .UsingJobData("jobID", task.Guid)
                .Build();
            var trigger = TriggerBuilder.Create()
                 .WithIdentity(task.Alias, Guid.NewGuid().ToString())
                 .StartAt(DateTime.Now.AddSeconds(30))
                 .WithSchedule(CronScheduleBuilder.CronSchedule(task.CronExpression))
                 .Build();
            Scheduler.ScheduleJob(dbtask, trigger);
        }

        public static void LaunchJob<T>(string guid, string identity, string command, string cron) where T : IJob {
            var dbtask = JobBuilder.Create<T>()
                .WithIdentity(identity, Guid.NewGuid().ToString())
                .UsingJobData("data", command)
                .UsingJobData("jobID", guid)
                .Build();
            var trigger = TriggerBuilder.Create()
                 .WithIdentity(identity, Guid.NewGuid().ToString())
                 .StartAt(DateTime.Now.AddSeconds(30))
                 .WithSchedule(CronScheduleBuilder.CronSchedule(cron))
                 .Build();
            Scheduler.ScheduleJob(dbtask, trigger);
        }

        public class Command : IJob {
            public void Execute(IJobExecutionContext context) {
                var dataMap = context.JobDetail.JobDataMap;
                var command = dataMap.GetString("data");
                if (command.StartsWith("*backup*")) {
                    command = command.Split(new[] { "*backup*" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (!string.IsNullOrEmpty(command)) {
                        var pool = dataMap.GetString("data");
                        command = $"zfs snap -r {pool}@{DateTime.Now.ToString("yyyyMMdd-HHmmss")}";
                        new Terminal().Execute(command);
                    }
                }
                else {
                    new Terminal().Execute(command.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }
    }
}