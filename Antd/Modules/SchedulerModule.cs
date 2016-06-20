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
using System.Threading;
using Antd.Database;
using Antd.Scheduler;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class SchedulerModule : CoreModule {

        private readonly JobRepository _jobRepositoryRepo = new JobRepository();

        public SchedulerModule() {
            this.RequiresAuthentication();

            Post["/cron"] = x => {
                var alias = (string)Request.Form.Alias;
                var command = (string)Request.Form.Command;
                var cron = (string)Request.Form.CronExValue;
                var guid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(alias.Trim() + command.Trim())) {
                    alias = guid;
                }
                _jobRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Guid", guid },
                    { "Alias", alias },
                    { "Data", command },
                    { "IntervalSpan", cron },
                    { "CronExpression", cron }
                });
                Thread.Sleep(20);
                JobScheduler.LaunchJob<JobScheduler.Command>(guid, alias, command, cron);
                return Response.AsRedirect("/");
            };

            Post["/scheduler/cron"] = x => {
                var alias = (string)Request.Form.Alias;
                var command = (string)Request.Form.Command;
                var cron = (string)Request.Form.CronResult;
                var guid = Guid.NewGuid().ToString();
                _jobRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Guid", guid },
                    { "Alias", alias },
                    { "Data", command },
                    { "IntervalSpan", "1" },
                    { "CronExpression", cron }
                });
                Thread.Sleep(20);
                JobScheduler.LaunchJob<JobScheduler.Command>(guid, alias, command, cron);
                return Response.AsRedirect("/");
            };

            Post["/scheduler/enable"] = x => {
                string guid = Request.Form.Guid;
                _jobRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "IsEnabled", "true" }
                });
                return HttpStatusCode.OK;
            };

            Post["/scheduler/disable"] = x => {
                string guid = Request.Form.Guid;
                _jobRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "IsEnabled", "false" }
                });
                return HttpStatusCode.OK;
            };

            Post["/scheduler/delete"] = x => {
                string guid = Request.Form.Guid;
                _jobRepositoryRepo.Delete(guid);
                return HttpStatusCode.OK;
            };

            Post["/scheduler/edit"] = x => {
                var id = (string)Request.Form.Guid;
                var command = (string)Request.Form.Command;
                _jobRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", id },
                    { "Data", command }
                });
                return HttpStatusCode.OK;
            };
        }
    }
}