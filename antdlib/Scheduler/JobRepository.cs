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
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Scheduler {

    public class JobRepository {

        public static List<JobModel> GetAll() {
            var list = DeNSo.Session.New.Get<JobModel>().ToArray();
            return list.Length == 0 ? new List<JobModel>() : list.ToList();
        }

        public static List<JobModel> GetEnabled() {
            var list = DeNSo.Session.New.Get<JobModel>(j => j.IsEnabled).ToArray();
            return list.Length == 0 ? new List<JobModel>() : list.ToList();
        }

        public static JobModel GetByGuid(string guid) {
            return DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
        }

        public static string GetResultByGuid(string guid) {
            var results = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).Select(j => j.Results).FirstOrDefault();
            var kvp = results?.OrderByDescending(r => r.Key).First();
            return kvp?.Value;
        }

        public static JobModel SetTaskOneTimeOnly(string guid, string alias, string data) {
            var task = new JobModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = guid,
                Alias = alias,
                Data = data,
                IsEnabled = false,
                TriggerPeriod = TriggerPeriod.IsOneTimeOnly,
                StartHour = DateTime.Now.Hour,
                StartMinute = DateTime.Now.Minute + 1
            };
            task.StartTime = new DateTime(2000, 1, 1, task.StartHour, task.StartMinute, 1, 1);
            task.CronExpression = "";
            DeNSo.Session.New.Set(task);
            return task;
        }

        public static JobModel SetTaskCron(string guid, string alias, string data, string cron) {
            var task = new JobModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = guid,
                Alias = alias,
                Data = data,
                IsEnabled = true,
                StartHour = DateTime.Now.Hour,
                StartMinute = DateTime.Now.Minute + 1
            };
            task.StartTime = new DateTime(2000, 1, 1, task.StartHour, task.StartMinute, 1, 1);
            task.CronExpression = cron;
            DeNSo.Session.New.Set(task);
            return task;
        }

        public static void Enable(string guid) {
            var task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            if (task == null) return;
            task.IsEnabled = true;
            DeNSo.Session.New.Set(task);
        }

        public static void Disable(string guid) {
            var task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            if (task == null) return;
            task.IsEnabled = false;
            DeNSo.Session.New.Set(task);
        }

        public static void Delete(string guid) {
            var task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            if (task != null) {
                DeNSo.Session.New.Delete(task);
            }
        }

        public static void AddResult(string guid, string data) {
            var task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            if (task == null) return;
            task.Results[DateTime.Now.ToString("yyyyMMddHHmmssfff")] = data;
            DeNSo.Session.New.Set(task);
        }
    }
}