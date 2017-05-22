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

using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using anthilla.core;

namespace Antd.Modules {
    public class AntdUpdateModule : NancyModule {

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        public AntdUpdateModule() {
            Get["/update"] = x => {
                var model = new PageUpdateModel();
                var updatecheck = CommandLauncher.Launch("mono-antdsh-update-check").ToList();
                var latestAntd = updatecheck.LastOrDefault(_ => _.Contains("update.antd"));
                var latestAntdUi = updatecheck.LastOrDefault(_ => _.Contains("update.antdui"));
                var latestAntdsh = updatecheck.LastOrDefault(_ => _.Contains("update.antdsh"));
                var latestSystem = updatecheck.LastOrDefault(_ => _.Contains("update.system"));
                var latestKernel = updatecheck.LastOrDefault(_ => _.Contains("update.kernel"));
                model.AntdLatestVersion = latestAntd;
                model.AntdUiLatestVersion = latestAntdUi;
                model.AntdshLatestVersion = latestAntdsh;
                model.SystemLatestVersion = latestSystem;
                model.KernelLatestVersion = latestKernel;
                const string antdActive = "/mnt/cdrom/Apps/Anthilla_Antd/active-version";
                const string antduiActive = "/mnt/cdrom/Apps/Anthilla_AntdUi/active-version";
                const string antdshActive = "/mnt/cdrom/Apps/Anthilla_antdsh/active-version";
                const string systemActive = "/mnt/cdrom/System/active-system";
                const string kernelActive = "/mnt/cdrom/Kernel/active-kernel";
                model.AntdVersion = GetVersionDateFromFile(Bash.Execute($"file {antdActive}"));
                model.AntdUiVersion = GetVersionDateFromFile(Bash.Execute($"file {antduiActive}"));
                model.AntdshVersion = GetVersionDateFromFile(Bash.Execute($"file {antdshActive}"));
                model.SystemVersion = GetVersionDateFromFile(Bash.Execute($"file {systemActive}"));
                model.KernelVersion = GetVersionDateFromFile(Bash.Execute($"file {kernelActive}"));
                return JsonConvert.SerializeObject(model);
            };

            Post["/update"] = x => {
                string context = Request.Form.Context;
                CommandLauncher.Launch("mono-antdsh-update", new Dictionary<string, string> { { "$context", context } });
                return HttpStatusCode.OK;
            };
        }
    }
}