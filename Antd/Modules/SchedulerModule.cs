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

using Antd.Scheduler;
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;
using System;
using System.Dynamic;

namespace Antd {

    public class SchedulerModule : NancyModule {

        public SchedulerModule()
            : base("/jobs") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return View["page-scheduler"];
            };

            Get["/quartz"] = x => {
                string[] data = new string[] { 
                    "primo valore" + Guid.NewGuid().ToString().Substring(0,4),
                    "secondo valore" + Guid.NewGuid().ToString().Substring(0,4)
                };
                int i = new Random().Next(1, 10);
                string guid = Guid.NewGuid().ToString();
                string dataJson = JsonConvert.SerializeObject(data);
                JobRepository.Create(guid, dataJson, i);
                JobScheduler.LauchJob<AntdJob.Command>(
                    guid,
                    dataJson,
                    i
                );
                dynamic model = new ExpandoObject();
                model.Message = "Job created and executed.";
                return View["page-scheduler", model];
            };
        }
    }
}