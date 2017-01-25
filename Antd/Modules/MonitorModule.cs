using System;
using System.IO;
using System.Linq;
using antdlib.common;
using Antd.Info;
using Antd.Storage;
using Nancy;

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

namespace Antd.Modules {
    public class MonitorModule : CoreModule {

        private static string GetResourcesHtmlDiv(string value, string iconName = "") {
            var ico = string.IsNullOrEmpty(iconName) ? "" : $"<i class=\"icon-{iconName} fg-anthilla-blu on-left-more\" style=\"line-height: 5px;\"></i>";
            return $"<a class=\"element nav-button no-overlay bg-darker\" href=\"#\">{ico}<span>{value}</span></a><div class=\"element-divider\"></div>";
        }

        private static int GetPercentage(int tot, int part) {
            if(tot == 0 || part == 0) {
                return 0;
            }
            var p = part * 100 / tot;
            return p <= 100 ? p : 0;
        }

        public MonitorModule() {

            Get["/monitor/resources/html"] = x => {
                try {
                    var hostname = File.ReadAllText("/etc/hostname");
                    var hostnameHtml = GetResourcesHtmlDiv(hostname);
                    var machineInfo = new MachineInfo();
                    var uptime = machineInfo.GetUptime();
                    var up = uptime.Uptime.SplitToList("up").Last().Trim();
                    var upHtml = GetResourcesHtmlDiv($"Up: {up}");
                    var la = uptime.LoadAverage.Replace(" load average:", "").Trim();
                    var laHtml = GetResourcesHtmlDiv(la);
                    var memory = machineInfo.GetFree().FirstOrDefault();
                    var tot = memory?.Total;
                    var used = memory?.Used;
                    int resultTot;
                    int.TryParse(new string(tot?.SkipWhile(_ => !char.IsDigit(_)).TakeWhile(char.IsDigit).ToArray()), out resultTot);
                    int resultPart;
                    int.TryParse(new string(used?.SkipWhile(_ => !char.IsDigit(_)).TakeWhile(char.IsDigit).ToArray()), out resultPart);
                    var perc = GetPercentage(resultTot, resultPart);
                    var memHtml = GetResourcesHtmlDiv($"Memory Used: {perc}%");
                    var diskUsage = new DiskUsage();
                    var du = diskUsage.GetInfo().FirstOrDefault(_ => _.MountedOn == "/mnt/cdrom");
                    var duHtml = GetResourcesHtmlDiv($"Disk Used: {du?.UsePercentage}");
                    var response = $"{hostnameHtml}{upHtml}{laHtml}{memHtml}{duHtml}";
                    return Response.AsText(response);
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return Response.AsText(GetResourcesHtmlDiv("Unable to obtain data"));
                }
            };

            Get["/machineuuid"] = x => {
                var machineUuid = Machine.MachineId.Get;
                return machineUuid;
            };
        }
    }
}