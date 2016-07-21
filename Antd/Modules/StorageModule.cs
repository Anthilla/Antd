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
using antdlib.common;
using antdlib.Install;
using antdlib.Storage;
using Antd.Database;
using Antd.SystemdTimer;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class StorageModule : CoreModule {

        private readonly TimerRepository _timerRepository = new TimerRepository();

        public StorageModule() {
            this.RequiresAuthentication();

            Get["/storage/reload/volumes"] = x => {
                Volumes.PopulateBlocks();
                return HttpStatusCode.OK;
            };

            Post["/storage/install"] = x => {
                new InstallOperativeSystem((string)Request.Form.DiskName).SetDiskAndInstall();
                return HttpStatusCode.OK;
            };

            Get["/zfs/cron"] = x => {
                var list = _timerRepository.GetAll();
                return Response.AsJson(list);
            };

            Post["/zfs/snap"] = x => {
                var pool = (string)Request.Form.Pool;
                var hourInterval = (string)Request.Form.Interval;
                if (string.IsNullOrEmpty(pool) || string.IsNullOrEmpty(hourInterval)) {
                    return HttpStatusCode.InternalServerError;
                }
                var cron = $"0 0 0/{hourInterval} * * ?";
                Timers.Create(pool.ToLower() + "snap", cron, $"zfs snap -r {pool}@{DateTime.Now.ToString("yyyyMMdd-HHmmss")}");
                return HttpStatusCode.OK;
            };

            Post["/zfs/snap/disable"] = x => {
                string guid = Request.Form.Guid;
                var tt = _timerRepository.GetByGuid(guid);
                if (tt == null) return HttpStatusCode.InternalServerError;
                Timers.Disable(tt.Alias);
                return HttpStatusCode.OK;
            };

            Post["/parted/print"] = x => {
                var disk = (string)Request.Form.Disk;
                var result = Terminal.Execute($"parted /dev/{disk} print 2> /dev/null | grep 'Partition Table: '");
                return Response.AsText(result.Replace("Partition Table: ", ""));
            };

            Post["/parted/mklabel"] = x => {
                var disk = (string)Request.Form.Disk;
                var type = (string)Request.Form.Type;
                var yn = (string)Request.Form.Confirm;
                var result = Terminal.Execute($"parted -a optimal /dev/{disk} mklabel {type} {yn}");
                return Response.AsText(result);
            };
        }
    }
}