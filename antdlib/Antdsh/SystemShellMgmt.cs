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
using System.IO;
using System.Linq;
using antdlib.Common;
using static antdlib.Terminal.Terminal;
using static antdlib.Antdsh.Execute;
using static System.Console;

namespace antdlib.Antdsh {
    public class SystemShellMgmt {

        /// <summary>
        /// 01 - recupero il volume su cui è montato BootExt
        /// 02 - uso df {volume} per trovare lo spazio libero, espresso in byte
        /// </summary>
        public static bool ChechDiskSpace() {
            WriteLine("Checking Disk Space");
            var blkid = Execute("blkid | grep /dev/sda | grep BootExt");
            var volume = blkid.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()[0].Replace(":", "");
            var available = Execute($"df -k {volume} | sed -e 1d|head -3").
                Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()[3];
            var availableInt = Convert.ToInt32(available);
            var msg = (availableInt > 0) ? "There's enought disk space for the update" : "Not enough free disk space, try to remove something...";
            WriteLine($"{msg}");
            return (availableInt > 0);
        }

        /// <summary>
        /// 03 - se c'è spazio libero scarica i file del Kernel
        ///     li scarica in una cartella tmp e poi li sposta in /mnt/cdrom/Kernel
        ///     - active-firmware
        ///     - active-initrd
        ///     - active-kernel
        ///     - active-modules
        /// 04 - e in /mnt/cdrom/System
        ///     - active-system
        /// </summary>
        public static void DownloadNewFiles() {
            var firmwareTmp = $"{Parameter.AntdTmpDir}/firmare";
            FileSystem.Download("/url/download/firmware", $"{firmwareTmp}");
            var initrdTmp = $"{Parameter.AntdTmpDir}/initrd";
            FileSystem.Download("/url/download/initrd", $"{initrdTmp}");
            var kernelTmp = $"{Parameter.AntdTmpDir}/kernel";
            FileSystem.Download("/url/download/kernel", $"{kernelTmp}");
            var modulesTmp = $"{Parameter.AntdTmpDir}/modules";
            FileSystem.Download("/url/download/modules", $"{modulesTmp}");
            var systemTmp = $"{Parameter.AntdTmpDir}/system";
            FileSystem.Download("/url/download/system", $"{systemTmp}");

            Execute($"cp {firmwareTmp} {Parameter.RepoKernel}");
            Execute($"cp {initrdTmp} {Parameter.RepoKernel}");
            Execute($"cp {kernelTmp} {Parameter.RepoKernel}");
            Execute($"cp {modulesTmp} {Parameter.RepoKernel}");
            Execute($"cp {systemTmp} {Parameter.RepoSystem}");

            Execute($"rm {Parameter.RepoKernel}/active-firmware");
            Execute($"rm {Parameter.RepoKernel}/active-initrd");
            Execute($"rm {Parameter.RepoKernel}/active-kernel");
            Execute($"rm {Parameter.RepoKernel}/active-modules");
            Execute($"rm {Parameter.RepoSystem}/active-system");

            Execute($"ln -s {Parameter.RepoKernel}/firmware {Parameter.RepoKernel}/active-firmware");
            Execute($"ln -s {Parameter.RepoKernel}/initrd {Parameter.RepoKernel}/active-initrd");
            Execute($"ln -s {Parameter.RepoKernel}/kernel {Parameter.RepoKernel}/active-kernel");
            Execute($"ln -s {Parameter.RepoKernel}/modules {Parameter.RepoKernel}/active-modules");
            Execute($"ln -s {Parameter.RepoSystem}/system {Parameter.RepoSystem}/active-system");

            CleanTmp();
        }

        /// <summary>
        /// 05 - copia il file di grub
        /// </summary>
        public static void CopyGrub() {
            WriteLine("Copying grub.conf file");
            Execute($"cp /mnt/cdrom/grub/grub.cfg /mnt/cdrom/DIRS/DIR_cfg_antd_config/grub{DateTime.Now.ToString("yyyyMMdd")}.conf");
        }

        /// <summary>
        /// 06 - riavvia il sistema
        /// </summary>
        public static void Reboot() {
            WriteLine("Rebooting system right now!");
            Execute("reboot");
        }

        /// <summary>
        /// 07 - il sistema è stato aggiornato (forse) e riavviato con i nuovi file: controlla lo status
        /// </summary>
        public static void VerifyUpdate() {
            var versionInfo = Execute("uname -a").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()[2];
            WriteLine($"This aos version is: {versionInfo}");
        }

        /// <summary>
        /// 08 - se ve tutto bene antd si avvia in automatico al riavvio, la prima cosa che fa è create le units di sistema
        /// </summary>
        public static void CreateUnitsForSystem() {
            WriteLine("Setting system units");

            WriteLine("Reload daemon now...");
            Execute("systemctl daemon-reload");
        }

        /// <summary>
        /// 09 - controlla la cartella /mnt/cdrom/DIRS e monta il suo contenuto
        /// todo: crea file o cartelle se non ci sono
        /// </summary>
        public static void SetAndMountDirs() {
            WriteLine("Mounting directories and files: ");
            var directories = Directory.EnumerateDirectories(Parameter.RepoDirs).Where(d => !d.Contains(".ori"));
            foreach (var t in directories) {
                var path = Path.GetFileName(t);
                if (path == null)
                    continue;
                var newPath = path.Replace("_", "/");
                WriteLine($"{t} mounted on {newPath}");
                Execute($"mount --bind {t} {newPath}");
            }

            var files = Directory.EnumerateFiles(Parameter.RepoDirs).Where(f => !f.Contains(".ori"));
            foreach (var t in files) {
                var path = Path.GetFileName(t);
                if (path == null)
                    continue;
                var newPath = path.Replace("_", "/");
                WriteLine($"{t} mounted on {newPath}");
                Execute($"mount --bind {t} {newPath}");
            }
        }
    }
}
