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

using Antd.Storage;
using Antd.SystemdTimer;
using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Modules {
    public class AntdZfsModule : NancyModule {

        private readonly CommandLauncher _launcher = new CommandLauncher();

        public AntdZfsModule() {
            Get["/zfs"] = x => {
                var zfsSnap = new ZfsSnap();
                var zfs = new Zfs();
                var disks = new Disks();

                var model = new PageZfsModel {
                    ZpoolList = Zpool.List(),
                    ZfsList = zfs.List(),
                    ZfsSnap = zfsSnap.List(),
                    ZpoolHistory = Zpool.History(),
                    DisksList = disks.GetList().Where(_ => _.Type == "disk" && string.IsNullOrEmpty(_.Mountpoint))

                };
                return JsonConvert.SerializeObject(model);
            };

            //todo
            //Get["/zfs/cron"] = x => {
            //    var list = _timerRepository.GetAll();
            //    return Response.AsJson(list);
            //};

            Post["/zfs/snap"] = x => {
                string pool = Request.Form.Pool;
                string interval = Request.Form.Interval;
                if(string.IsNullOrEmpty(pool) || string.IsNullOrEmpty(interval)) {
                    return HttpStatusCode.InternalServerError;
                }
                Timers.Create(pool + "snap", interval, $"/sbin/zfs snap -r {pool}@${{TTDATE}}");
                return HttpStatusCode.OK;
            };

            //Post["/zfs/snap/disable"] = x => {
            //    string guid = Request.Form.Guid;
            //    var tt = _timerRepository.GetByGuid(guid);
            //    if(tt == null)
            //        return HttpStatusCode.InternalServerError;
            //    Timers.Disable(tt.Alias);
            //    return HttpStatusCode.OK;
            //};

            Post["/zpool/create"] = x => {
                string altroot = Request.Form.Altroot;
                string poolname = Request.Form.Name;
                string pooltype = Request.Form.Type;
                string disk = Request.Form.Disk;
                if(string.IsNullOrEmpty(altroot) || string.IsNullOrEmpty(poolname) || string.IsNullOrEmpty(pooltype) || string.IsNullOrEmpty(disk)) {
                    return HttpStatusCode.BadRequest;
                }
                ConsoleLogger.Log($"[zpool] create => altroot:{altroot} poolname:{poolname} pooltype:{pooltype} disk:{disk} ");
                _launcher.Launch("zpool-create", new Dictionary<string, string> {
                    { "$pool_altroot", altroot },
                    { "$pool_name", poolname },
                    { "$pool_type", poolname },
                    { "$disk", disk }
                });
                return HttpStatusCode.OK;
            };

            Post["/zfs/create"] = x => {
                string altroot = Request.Form.Altroot;
                string poolname = Request.Form.Name;
                string datasetname = Request.Form.Dataset;
                if(string.IsNullOrEmpty(altroot) || string.IsNullOrEmpty(poolname) || string.IsNullOrEmpty(datasetname)) {
                    return HttpStatusCode.BadRequest;
                }
                _launcher.Launch("zfs-create", new Dictionary<string, string> {
                    { "$pool_altroot", altroot },
                    { "$pool_name", poolname },
                    { "$dataset_name", datasetname }
                });
                return HttpStatusCode.OK;
            };
        }
    }
}