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
using antdlib.common;
using antdlib.models;
using Antd.Storage;
using Nancy;
using Newtonsoft.Json;

namespace Antd.ServerModules {
    public class AntdStorageModule : NancyModule {

        private readonly Bash _bash = new Bash();

        public AntdStorageModule() {
            Get["/storage"] = x => {
                var disks = new Disks();
                var model = new PageStorageModel {
                    DisksList = disks.GetList()
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/storage/print"] = x => {
                string disk = Request.Form.Disk;
                var result = _bash.Execute($"parted /dev/{disk} print 2> /dev/null").SplitBash().Grep("'Partition Table: '").First();
                return Response.AsText(result.Replace("Partition Table: ", ""));
            };

            Post["/storage/mklabel"] = x => {
                string disk = Request.Form.Disk;
                string type = Request.Form.Type;
                string yn = Request.Form.Confirm;
                var result = _bash.Execute($"parted -a optimal /dev/{disk} mklabel {type} {yn}");
                return Response.AsText(result);
            };
        }
    }
}