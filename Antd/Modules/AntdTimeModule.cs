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
using antd.commands;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AntdTimeModule : NancyModule {

        public AntdTimeModule() {

            Get["/time/info"] = x => {
                const StringSplitOptions ssoree = StringSplitOptions.RemoveEmptyEntries;
                var bash = new Bash();
                var launcher = new CommandLauncher();
                var hostConfiguration = new HostConfiguration();
                var timezones = bash.Execute("timedatectl list-timezones --no-pager").SplitBash();
                var timedatectl = launcher.Launch("timedatectl").ToList();
                var ntpd = launcher.Launch("cat-etc-ntp").ToArray();
                var model = new PageTimeModel {
                    Timezones = timezones,
                    LocalTime = timedatectl.First(_ => _.Contains("Local time:")).Split(new[] { ":" }, 2, ssoree)[1],
                    UnivTime = timedatectl.First(_ => _.Contains("Universal time:")).Split(new[] { ":" }, 2, ssoree)[1],
                    RtcTime = timedatectl.First(_ => _.Contains("RTC time:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Timezone = timedatectl.First(_ => _.Contains("Time zone:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Nettimeon = timedatectl.First(_ => _.Contains("Network time on:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Ntpsync = timedatectl.First(_ => _.Contains("NTP synchronized:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Rtcintz = timedatectl.First(_ => _.Contains("RTC in local TZ:")).Split(new[] { ":" }, 2, ssoree)[1],
                    NtpServer = hostConfiguration.Host.NtpdateServer.StoredValues["$server"],
                    Ntpd = ntpd.JoinToString("<br />"),
                    NtpdEdit = ntpd.JoinToString(Environment.NewLine)
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/host/timezone"] = x => {
                string timezone = Request.Form.Timezone;
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetTimezone(timezone);
                hostconfiguration.ApplyTimezone();
                return HttpStatusCode.OK;
            };

            Post["/host/synctime"] = x => {
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SyncClock();
                return HttpStatusCode.OK;
            };

            Post["/host/ntpdate"] = x => {
                string ntpdate = Request.Form.Ntpdate;
                var hostconfiguration = new HostConfiguration();
                hostconfiguration.SetNtpdate(ntpdate);
                hostconfiguration.ApplyNtpdate();
                return HttpStatusCode.OK;
            };

            Post["/host/ntpd"] = x => {
                string ntpd = Request.Form.Ntpd;
                var hostConfiguration = new HostConfiguration();
                hostConfiguration.SetNtpd(ntpd.Contains("\n")
                  ? ntpd.SplitToList("\n").ToArray()
                  : ntpd.SplitToList(Environment.NewLine).ToArray());
                hostConfiguration.ApplyNtpd();
                return HttpStatusCode.OK;
            };
        }
    }
}