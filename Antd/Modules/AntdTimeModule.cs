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

using antdlib.config;
using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Linq;
using anthilla.core;

namespace Antd.Modules {
    public class AntdTimeModule : NancyModule {

        public AntdTimeModule() {

            Get["/time/info"] = x => {
                const StringSplitOptions ssoree = StringSplitOptions.RemoveEmptyEntries;
                var timezones = Bash.Execute("timedatectl list-timezones --no-pager").Split();
                var timedatectl = CommandLauncher.Launch("timedatectl").ToList();
                var ntpd = CommandLauncher.Launch("cat-etc-ntp").ToArray();
                var model = new PageTimeModel {
                    Timezones = timezones,
                    LocalTime = timedatectl.First(_ => _.Contains("Local time:")).Split(new[] { ":" }, 2, ssoree)[1],
                    UnivTime = timedatectl.First(_ => _.Contains("Universal time:")).Split(new[] { ":" }, 2, ssoree)[1],
                    RtcTime = timedatectl.First(_ => _.Contains("RTC time:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Timezone = timedatectl.First(_ => _.Contains("Time zone:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Nettimeon = timedatectl.First(_ => _.Contains("Network time on:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Ntpsync = timedatectl.First(_ => _.Contains("NTP synchronized:")).Split(new[] { ":" }, 2, ssoree)[1],
                    Rtcintz = timedatectl.First(_ => _.Contains("RTC in local TZ:")).Split(new[] { ":" }, 2, ssoree)[1],
                    NtpServer = HostConfiguration.Host.NtpdateServer.StoredValues["$server"],
                    Ntpd = EnumerableExtensions.JoinToString(ntpd, "<br />"),
                    NtpdEdit = EnumerableExtensions.JoinToString(ntpd, Environment.NewLine)
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/host/timezone"] = x => {
                string timezone = Request.Form.Timezone;
                HostConfiguration.SetTimezone(timezone);
                HostConfiguration.ApplyTimezone();
                return HttpStatusCode.OK;
            };

            Post["/host/synctime"] = x => {
                HostConfiguration.SyncClock();
                return HttpStatusCode.OK;
            };

            Post["/host/ntpdate"] = x => {
                string ntpdate = Request.Form.Ntpdate;
                HostConfiguration.SetNtpdate(ntpdate);
                HostConfiguration.ApplyNtpdate();
                return HttpStatusCode.OK;
            };

            Post["/host/ntpd"] = x => HttpStatusCode.OK;
        }
    }
}