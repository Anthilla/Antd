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

using System.Linq;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdSchedulerModule : NancyModule {

        public AntdSchedulerModule() {
            Get["/scheduler"] = x => {
                var schedulerConfiguration = new TimerConfiguration();
                var scheduledJobs = schedulerConfiguration.Get().Timers;
                var model = new PageSchedulerModel {
                    Jobs = scheduledJobs?.ToList().OrderBy(_ => _.Alias)
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/scheduler/set"] = x => {
                var schedulerConfiguration = new TimerConfiguration();
                schedulerConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/scheduler/enable"] = x => {
                var dhcpdConfiguration = new TimerConfiguration();
                dhcpdConfiguration.Enable();
                return HttpStatusCode.OK;
            };

            Post["/scheduler/disable"] = x => {
                var dhcpdConfiguration = new TimerConfiguration();
                dhcpdConfiguration.Disable();
                return HttpStatusCode.OK;
            };

            Post["/scheduler/timer"] = x => {
                string alias = Request.Form.Alias;
                string time = Request.Form.Time;
                string command = Request.Form.Command;
                var model = new TimerModel{
                    Alias = alias,
                    Time = time,
                    Command = command,
                    IsEnabled = true
                };
                var schedulerConfiguration = new TimerConfiguration();
                schedulerConfiguration.AddTimer(model);
                return HttpStatusCode.OK;
            };

            Post["/scheduler/timer/del"] = x => {
                string guid = Request.Form.Guid;
                var schedulerConfiguration = new TimerConfiguration();
                schedulerConfiguration.RemoveTimer(guid);
                return HttpStatusCode.OK;
            };
        }
    }
}