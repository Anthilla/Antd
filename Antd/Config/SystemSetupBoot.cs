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

namespace Antd {

    public class SystemSetupBoot {
        private static string cfgAlias = "antdSetupBoot";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdBoot");

        public static string[] baseScriptArgs = new string[] {
                "mount /mnt/cdrom -o remount,rw,noatime,discard",
                "mount /mnt/cdrom/DIRS/DIR_usr_lib64_mono.402git.squashfs.xz /usr/lib64/mono",
                "mkdir /cfg/base/repo",
                "mount /mnt/cdrom/DIRS/DIR_cfg_base_repo /cfg/base/repo",
                "mount /mnt/cdrom/DIRS/DIR_etc_portage.squashfs.xz /etc/portage",
                "mkdir /framework/antd",
                "mount /mnt/cdrom/DIRS/DIR_framework_antd /framework/antd",
                "mkdir /framework/anthilla",
                "mount /mnt/cdrom/DIRS/DIR_framework_anthilla /framework/anthilla",
                "mount /mnt/cdrom/DIRS/DIR_var /var",
                "mount /mnt/cdrom/DIRS/DIR_var_cache /var/cache",
                "mount /mnt/cdrom/DIRS/DIR_var_db /var/db",
                "mount /mnt/cdrom/DIRS/DIR_var_db_pkg.squashfs.xz /var/db/pkg",
                "mount /mnt/cdrom/DIRS/DIR_var_lib_portage /var/lib/portage",
                "mount /mnt/cdrom/DIRS/DIR_var_log /var/log",
                "zpool import Data01",
                "zpool import Data02",
                "zfs mount -a",
                "rsync -aHAz --delete-during /tmp/ /Data/Data01Vol01/tmp/",
                "rm -fR /tmp/* ",
                "mount -o bind /Data/Data01Vol01/tmp /tmp"
            };

        public static void Start() {
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var row = baseScriptArgs[i];
                var array = row.Split(new[] { ' ' }, 2);
                string file = array[0];
                string args = array[1];
                Command.Launch(file, args);
            }
        }

        public static void Write() {
            for (int i = 0; i < baseScriptArgs.Length; i++) {
                var n = i.ToString();
                if (config.CheckValue(n) == false) {
                    config.Write(n, baseScriptArgs[i]);
                }
            }
        }

        public static void StartAndWrite() {
            Start();
            Write();
        }
    }
}