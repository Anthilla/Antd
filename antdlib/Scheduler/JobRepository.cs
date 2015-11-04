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
using System.Dynamic;
using System.Linq;

namespace antdlib.Scheduler {

    public class JobRepository {

        public static List<JobModel> GetAll() {
            var list = DeNSo.Session.New.Get<JobModel>().ToArray();
            if (list.Length == 0) {
                return new List<JobModel>() { };
            }
            else {
                return list.ToList();
            }
        }

        public static List<JobModel> GetEnabled() {
            var list = DeNSo.Session.New.Get<JobModel>(j => j.isEnabled == true).ToArray();
            if (list.Length == 0) {
                return new List<JobModel>() { };
            }
            else {
                return list.ToList();
            }
        }

        public static JobModel GetByGuid(string guid) {
            return DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
        }

        public static string GetResultByGuid(string guid) {
            var results = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid && j != null).Select(j => j.Results).FirstOrDefault();
            var kvp = results.OrderByDescending(r => r.Key).First();
            return kvp.Value.ToString();
        }

        public static JobModel SetTaskOneTimeOnly(string guid, string data) {
            JobModel task = new JobModel();
            task._Id = Guid.NewGuid().ToString();
            task.Guid = guid;
            task.Alias = guid;
            task.Data = data;
            task.isEnabled = false;
            task.Results = new ExpandoObject() as IDictionary<String, object>;
            task.TriggerPeriod = TriggerPeriod.IsOneTimeOnly;
            task.StartHour = DateTime.Now.Hour;
            task.StartMinute = DateTime.Now.Minute + 1;
            task.StartTime = new DateTime(2000, 1, 1, task.StartHour, task.StartMinute, 1, 1);
            task.CronExpression = "";
            DeNSo.Session.New.Set(task);
            return task;
        }

        public static JobModel SetTaskOneTimeOnly(string guid, string alias, string data) {
            JobModel task = new JobModel();
            task._Id = Guid.NewGuid().ToString();
            task.Guid = guid;
            task.Alias = alias;
            task.Data = data;
            task.isEnabled = false;
            task.Results = new ExpandoObject() as IDictionary<String, object>;
            task.TriggerPeriod = TriggerPeriod.IsOneTimeOnly;
            task.StartHour = DateTime.Now.Hour;
            task.StartMinute = DateTime.Now.Minute + 1;
            task.StartTime = new DateTime(2000, 1, 1, task.StartHour, task.StartMinute, 1, 1);
            task.CronExpression = "";
            DeNSo.Session.New.Set(task);
            return task;
        }

        public static JobModel SetTaskCron(string guid, string alias, string data, string _cron) {
            JobModel task = new JobModel();
            task._Id = Guid.NewGuid().ToString();
            task.Guid = guid;
            task.Alias = alias;
            task.Data = data;
            task.isEnabled = true;
            task.Results = new ExpandoObject() as IDictionary<String, object>;
            task.TriggerPeriod = TriggerPeriod.IsCron;
            task.StartHour = DateTime.Now.Hour;
            task.StartMinute = DateTime.Now.Minute + 1;
            task.StartTime = new DateTime(2000, 1, 1, task.StartHour, task.StartMinute, 1, 1);
            task.CronExpression = _cron;
            DeNSo.Session.New.Set(task);
            return task;
        }

        public static void Enable(string guid) {
            JobModel task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            if (task.TriggerPeriod == TriggerPeriod.IsCron) {
                task.isEnabled = true;
                DeNSo.Session.New.Set(task);
            }
        }

        public static void Disable(string guid) {
            JobModel task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            if (task.TriggerPeriod == TriggerPeriod.IsCron) {
                task.isEnabled = false;
                DeNSo.Session.New.Set(task);
            }
        }

        public static void Delete(string guid) {
            JobModel task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            DeNSo.Session.New.Delete(task);
        }

        public static void AddResult(string guid, string data) {
            JobModel task = DeNSo.Session.New.Get<JobModel>(j => j.Guid == guid).FirstOrDefault();
            var p = task.Results as IDictionary<String, object>;
            p[DateTime.Now.ToString("yyyyMMddHHmmssfff")] = data;
            task.Results = p;
            DeNSo.Session.New.Set(task);
        }
    }
}