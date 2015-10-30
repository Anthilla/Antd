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

using System.Dynamic;
using antdlib.Scheduler;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class SchedulerModule : NancyModule {

        public SchedulerModule()
            : base("/scheduler") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.JobList = JobRepository.GetAll();
                return View["page-scheduler", vmod];
            };

            Post["/now"] = x => {
                var alias = (string)Request.Form.Alias;
                var command = (string)Request.Form.Command;
                Job.Schedule(alias, command);
                return Response.AsRedirect("/scheduler");
            };

            Post["/cron"] = x => {
                var alias = (string)Request.Form.Alias;
                var command = (string)Request.Form.Command;
                var cron = (string)Request.Form.CronResult;
                Job.Schedule(alias, command, cron);
                return Response.AsRedirect("/scheduler");
            };

            //Post["/other"] = x => {
            //    var _alias = (string)Request.Form.Alias;
            //    var _command = (string)Request.Form.Command;
            //    var _cron = (string)Request.Form.CronResult;
            //    Job.Schedule(_alias, _command, _cron);
            //    dynamic model = new ExpandoObject();
            //    return Response.AsRedirect("/scheduler");
            //};

            Get["/enable/{guid}"] = x => {
                string guid = x.guid;
                JobRepository.Enable(guid);
                return Response.AsJson(true);
            };

            Get["/disable/{guid}"] = x => {
                string guid = x.guid;
                JobRepository.Disable(guid);
                return Response.AsJson(true);
            };

            Get["/launch/{guid}"] = x => {
                string guid = x.guid;
                Job.ReSchedule(guid);
                return Response.AsJson(true);
            };

            Get["/delete/{guid}"] = x => {
                string guid = x.guid;
                JobRepository.Delete(guid);
                return Response.AsJson(true);
            };
        }
    }
}