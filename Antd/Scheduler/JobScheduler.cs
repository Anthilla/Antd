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
                List<JobModel> taskList = JobRepository.GetAll();
                foreach (JobModel task in taskList) {
                    LauchJob<AntdJob.CommandJob>(
                        task.Guid,
                        task.Data,
                        task.Interval
                    );
                }
            }
        }

        public static void Stop() {
            __scheduler.Shutdown();
        }

        public static void LauchJob<T>(string _identity, string _data, int _interval) where T : IJob {
            IJobDetail job = DefineJob<T>(_identity, _data);
            ITrigger trigger = DefineTrigger(_identity, _interval);
            __scheduler.ScheduleJob(job, trigger);
        }

        private static IJobDetail DefineJob<T>(string _identity, string _data) where T : IJob {
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .UsingJobData("data", _data)
                .UsingJobData("jobID", _identity)
                .Build();
            return job;
        }

        private static ITrigger DefineTrigger(string _identity, int _interval) {
            //todo
            //customize triggers' timers
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(_interval)
                    .RepeatForever())
                .Build();
            return trigger;
        }

        private static ITrigger DefineTrigger() {
            //parametri per definire l'identita del trigger
            string _identity = "";
            string _group = Guid.NewGuid().ToString();

            //parametri di inizio e fine della schedulazione, validi per tutti
            DateTime _startTime = DateTime.Now; //lo startTime definisce anche l'ora a cui far scattare il trigger
            DateTime _endTime = DateTime.Now.AddDays(7);

            //paramtro per il dailyTrigger -> ogni quanti giorni?
            int _daySpan = 1;

            //parametri per il weeklyTrigger -> quale giorno della settimana?
            //int _dayOfWeek = 1;
            DayOfWeek _dayOfWeek = DayOfWeek.Sunday; //todo, definire il giorno in un altro modo... -> es: from int
            int _weeklyHour = _startTime.Hour;
            int _weeklyMinute = _startTime.Minute;

            ITrigger oneTimeOnlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, _group)
                .StartAt(_startTime)
                .Build();

            ITrigger dailyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, _group)
                .StartAt(_startTime)
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromDays(_daySpan)))
                .EndAt(_endTime)
                .Build();

            ITrigger weeklyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, _group)
                .StartAt(_startTime)
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(_dayOfWeek, _weeklyHour, _weeklyMinute))
                .EndAt(_endTime)
                .Build();

            ITrigger monthlyTrigger = TriggerBuilder.Create()
                .WithIdentity(_identity, _group)
                .StartAt(_startTime)
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(_dayOfWeek, _weeklyHour, _weeklyMinute))
                .EndAt(_endTime)
                .Build();

            //ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc//
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString())
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
                .Build();
            return trigger;
        }
    }
}
