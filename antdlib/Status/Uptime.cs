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

using antdlib.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.Common;

namespace antdlib.Status {

    public class Uptime {

        private static string GetUptime() {
            var output = JsonConvert.SerializeObject(Terminal.Terminal.Execute("uptime").ConvertCommandToModel().output);
            return MapUptimeJson(output).uptime;
        }

        public static string UpTime => GetUptime();

        private static string GetLoadAverage() {
            var output = JsonConvert.SerializeObject(Terminal.Terminal.Execute("uptime").ConvertCommandToModel().output);
            return MapUptimeJson(output).loadAverage;
        }

        public static string LoadAverage => GetLoadAverage();

        private static string[] GetLoadAverageValues() {
            var output = JsonConvert.SerializeObject(Terminal.Terminal.Execute("uptime").ConvertCommandToModel().output);
            return MapUptimeJson(output).loadAverageValues;
        }

        public static string[] LoadAverageValues => GetLoadAverageValues();

        private static UptimeModel MapUptimeJson(string inUptimeJson) {
            var uptimeJson = Regex.Replace(inUptimeJson, "\"", "").Replace("\\n", "\n");
            var uptimeJsonRow = uptimeJson.Split(new[] { "  " }, StringSplitOptions.None).ToArray();
            var model = new UptimeModel();
            if (uptimeJsonRow.Length == 3) {
                model.uptime = uptimeJsonRow[0];
                model.users = uptimeJsonRow[1];
                model.loadAverage = uptimeJsonRow[2];
                var values = uptimeJsonRow[2].Split(new[] { ", " }, StringSplitOptions.None).ToArray();
                model.loadAverageValues = values;
            }
            else {
                model.uptime = uptimeJson;
                model.users = uptimeJson;
                model.loadAverage = uptimeJson;
            }
            return model;
        }
    }
}