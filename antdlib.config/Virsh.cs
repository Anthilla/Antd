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

using antdlib.common;
using antdlib.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.config {
    public class Virsh {

        private readonly Bash _bash = new Bash();

        public IEnumerable<VirtualMachineInfo> GetVmList() {
            var vms = new List<VirtualMachineInfo>();
            var res = _bash.Execute("virsh list --all | sed '1,2d'");
            if(res.Length < 1) {
                return vms;
            }
            var virshVms = res.Split(new[] { Environment.NewLine }, 3, StringSplitOptions.RemoveEmptyEntries);
            foreach(var i in virshVms) {
                var info = i.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var vm = new VirtualMachineInfo {
                    Id = info[0],
                    Domain = info[1],
                    State = info[2],
                };
                var vnc = GetVmVncAddress(vm.Domain);
                vm.VncIp = vnc.Key;
                vm.VncPort = vnc.Value;
                vms.Add(vm);
            }
            return vms;
        }

        private KeyValuePair<string, string> GetVmVncAddress(string domain) {
            var res = _bash.Execute($"virsh dumpxml {domain}").SplitBash().Grep("graphics type='vnc'").First();
            if(res.Length < 1 || !res.Contains("port=") || !res.Contains("listen=")) {
                return new KeyValuePair<string, string>(null, null);
            }
            var port = new Regex("port='([\\d]*)'", RegexOptions.Multiline).Matches(res)[0].Value;
            var ip = new Regex("listen='([\\d. ]*)'", RegexOptions.Multiline).Matches(res)[0].Value;
            return new KeyValuePair<string, string>(ip, port);
        }
    }
}
