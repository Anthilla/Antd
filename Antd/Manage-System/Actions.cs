///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Scheduler;
using Newtonsoft.Json;
using System;

namespace Antd {

    public class Action {

        public static void Schedule(string _alias, string _command, string _args) {
            int startH = DateTime.Now.Hour;
            int startM = DateTime.Now.Minute + 1;
            int endH = startH + 1;
            int endM = startM;
            string[] data = new string[] {
                    _command,
                    _args
                };
            string guid = Guid.NewGuid().ToString();
            string dataJson = JsonConvert.SerializeObject(data);
            JobModel task = JobRepository.Create(guid, _alias, dataJson);
            JobRepository.AssignTrigger(guid, TriggerModel.TriggerPeriod.IsOneTimeOnly, startH, startM, endH, endM, "");
            JobScheduler.LauchJob<AntdJob.CommandJob>(task);
        }

        public static void Mount(string arguments, string source, string location) {
            Schedule("_mount_", "mount", arguments + " " + source + " " + location);
        }

        public static void Mkdir(string arguments, string dir) {
            Schedule("_mkdir_", "mkdir", arguments + " " + dir);
        }

        public static void Dhclient(string name) {
            Schedule("_dhclient_", "dhclient", name);
        }

        public static void SshKeygen(string arguments) {
            Schedule("_ssh-keygen_", "ssh-keygen", arguments);
        }

        public static void Sshd(string arguments) {
            Schedule("_/usr/sbin/sshd_", "/usr/sbin/sshd", arguments);
        }

        public static void Sleep(string arguments) {
            Schedule("_sleep_", "sleep", arguments);
        }

        public static void QemuNbd(string arguments, string source) {
            Schedule("_qemu-nbd_", "qemu-nbd", arguments + " " + source);
        }

        public static void Kpartx(string arguments, string source) {
            Schedule("_kpartx_", "kpartx", arguments + " " + source);
        }

        public static void Zpool(string arguments) {
            Schedule("_zpool_", "zpool", arguments);
        }

        public static void Zfs(string arguments) {
            Schedule("_zfs_", "zfs", arguments);
        }

        public static void Rsync(string arguments, string sourceA, string sourceB) {
            Schedule("_rsync_", "rsync", arguments + " " + sourceA + " " + sourceB);
        }

        public static void Remove(string arguments, string source) {
            Schedule("_rm_", "rm", arguments + " " + source);
        }
    }
}