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

                    //"mount -o bind /mnt/cdrom /System",
                    //"mount -o bind /mnt/cdrom /boot",
                    //"mount -t tmpfs none /Data",
                    //"mount -t tmpfs none /lib64/modules",
                    //"mkdir -p /lib64/modules/3.16.7-aufs",
                    //"mount /System/DIR_lib64_modules_3.16.7-aufs.squash.xz /lib64/modules/3.16.7-aufs",
                    //"mount /System/DIR_lib64_firmware.squash.xz /lib64/firmware",
                    //"qemu-nbd --connect=/dev/nbd0 /System/cfg.qed",
                    //"kpartx -a /dev/nbd0",
                    //"sleep 1",
                    //"mount -o rw,noatime,discard /dev/mapper/nbd0p1 /cfg",
                    //"qemu-nbd --connect=/dev/nbd1 /System/SystemDataRepo.qed",
                    //"kpartx -a /dev/nbd1",
                    //"sleep 1",
                    //"mkdir -p  /Data/SystemDataRepo",
                    //"mount -o rw,noatime,discard /dev/mapper/nbd1p1 /Data/SystemDataRepo/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_framework/ /framework/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_home/ /home/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_home_SYS/ /home/SYS/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_account/ /var/account/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_amavis/ /var/amavis/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_bind/ /var/bind/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_cache/ /var/cache/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_db_pkg/ /var/db/pkg/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_db_sudo/ /var/db/sudo/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_db_iscsi/ /var/db/iscsi/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_heimdal/ /var/heimdal/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_imap/ /var/imap/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_spool/ /var/spool/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_target/ /var/target/",
                    //"mount -o bind /Data/SystemDataRepo/DIR_var_www/ /var/www/",
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