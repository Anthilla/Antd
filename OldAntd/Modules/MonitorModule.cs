using Antd.Info;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using anthilla.core;

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
    public class MonitorModule : NancyModule {

        private static int GetPercentage(int tot, int part) {
            if(tot == 0 || part == 0) {
                return 0;
            }
            var p = part * 100 / tot;
            return p <= 100 ? p : 0;
        }

        public MonitorModule() {

            Get["/monitor/resources"] = x => {
                var hostname = File.ReadAllText("/etc/hostname");
                var uptime = MachineInfo.GetUptime();
                var memoryUsage = 0;
                var memory = MachineInfo.GetFree().FirstOrDefault();
                if(memory != null) {
                    var tot = memory.Total;
                    var used = memory.Used;
                    int resultTot;
                    int.TryParse(new string(tot?.SkipWhile(_ => !char.IsDigit(_)).TakeWhile(char.IsDigit).ToArray()), out resultTot);
                    int resultPart;
                    int.TryParse(new string(used?.SkipWhile(_ => !char.IsDigit(_)).TakeWhile(char.IsDigit).ToArray()), out resultPart);
                    memoryUsage = GetPercentage(resultTot, resultPart);
                }
                var du = MachineInfo.GetDiskUsage().FirstOrDefault(_ => _.MountedOn == "/mnt/cdrom");
                var model = new PageMonitorModel {
                    Hostname = hostname,
                    Uptime = uptime.Uptime.SplitToList("up").Last().Trim(),
                    LoadAverage = uptime.LoadAverage.Replace(" load average:", "").Trim(),
                    MemoryUsage = memoryUsage.ToString(),
                    DiskUsage = du?.UsePercentage
                };
                return JsonConvert.SerializeObject(model);
            };

            Get["/configured"] = x => JsonConvert.SerializeObject(true);
        }
    }
}