
using antdlib;
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
using System;
using System.IO;
using System.Linq;
using static antdlib.Terminal;
using static antdsh.execute;
using static System.Console;

namespace antdsh {
    public class system {

        /// <summary>
        /// 01 - recupero il volume su cui è montato BootExt
        /// 02 - uso df {volume} per trovare lo spazio libero, espresso in byte
        /// </summary>
        public static bool ChechDiskSpace() {
            WriteLine("Checking Disk Space");
            var blkid = Execute("blkid | grep /dev/sda | grep BootExt");
            var volume = blkid.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()[0].Replace(":", "");
            var available = Execute($"df -k {volume} | sed -e 1d|head -3").
                Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()[3];
            var availableInt = Convert.ToInt32(available);
            var msg = (availableInt > 0) ? "There's enought disk space for the update" : "Not enough free disk space, try to remove something...";
            WriteLine($"{msg}");
            return (availableInt > 0) ? true : false;
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
        /// todo: cambiare il file name
        /// </summary>
        public static void DownloadNewFiles() {
            var firmwareTmp = $"{global.tmpDir}/firmare";
            DownloadFromUrl("/url/download/firmware", $"{firmwareTmp}");
            var initrdTmp = $"{global.tmpDir}/initrd";
            DownloadFromUrl("/url/download/initrd", $"{initrdTmp}");
            var kernelTmp = $"{global.tmpDir}/kernel";
            DownloadFromUrl("/url/download/kernel", $"{kernelTmp}");
            var modulesTmp = $"{global.tmpDir}/modules";
            DownloadFromUrl("/url/download/modules", $"{modulesTmp}");
            var systemTmp = $"{global.tmpDir}/system";
            DownloadFromUrl("/url/download/system", $"{systemTmp}");

            var firmware = $"{global.system.kernelDir}/firmare";
            var initrd = $"{global.system.kernelDir}/initrd";
            var kernel = $"{global.system.kernelDir}/kernel";
            var modules = $"{global.system.kernelDir}/modules";
            var system = $"{global.system.kernelDir}/system";

            Execute($"cp {firmwareTmp} {global.system.kernelDir}");
            Execute($"cp {initrdTmp} {global.system.kernelDir}");
            Execute($"cp {kernelTmp} {global.system.kernelDir}");
            Execute($"cp {modulesTmp} {global.system.kernelDir}");
            Execute($"cp {systemTmp} {global.system.systemDir}");

            Execute($"rm {global.system.kernelDir}/active-firmware");
            Execute($"rm {global.system.kernelDir}/active-initrd");
            Execute($"rm {global.system.kernelDir}/active-kernel");
            Execute($"rm {global.system.kernelDir}/active-modules");
            Execute($"rm {global.system.systemDir}/active-system");

            Execute($"ln -s {global.system.kernelDir}/firmware {global.system.kernelDir}/active-firmware");
            Execute($"ln -s {global.system.kernelDir}/initrd {global.system.kernelDir}/active-initrd");
            Execute($"ln -s {global.system.kernelDir}/kernel {global.system.kernelDir}/active-kernel");
            Execute($"ln -s {global.system.kernelDir}/modules {global.system.kernelDir}/active-modules");
            Execute($"ln -s {global.system.systemDir}/system {global.system.systemDir}/active-system");

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
            var versionInfo = Execute("uname -a").Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray()[2];
            WriteLine($"This aos version is: {versionInfo}");
        }

        /// <summary>
        /// 08 - se ve tutto bene antd si avvia in automatico al riavvio, la prima cosa che fa è create le units di sistema
        /// todo: quali sono le units?
        /// </summary>
        public static void CreateUnitsForSystem() {
            WriteLine("Setting system units");

            WriteLine("Reload daemon now...");
            Terminal.Execute("systemctl daemon-reload");
        }

        /// <summary>
        /// 09 - controlla la cartella /mnt/cdrom/DIRS e monta il suo contenuto
        /// </summary>
        public static void SetAndMountDirs() {
            WriteLine("Mounting directories and files: ");
            var directories = Directory.EnumerateDirectories(global.dir).Where(d => !d.Contains(".ori")).ToArray();
            for (int i = 0; i < directories.Length; i++) {
                var path = Path.GetFileName(directories[i]);
                var newPath = path.Replace("_", "/");
                WriteLine($"{directories[i]} mounted on {newPath}");
                Execute($"mount --bind {directories[i]} {newPath}");
            }

            var files = Directory.EnumerateFiles(global.dir).Where(f => !f.Contains(".ori")).ToArray();
            for (int i = 0; i < files.Length; i++) {
                var path = Path.GetFileName(files[i]);
                var newPath = path.Replace("_", "/");
                WriteLine($"{files[i]} mounted on {newPath}");
                Execute($"mount --bind {files[i]} {newPath}");
            }
        }

        /// <summary>
        /// 10 - controlla status di antd
        /// </summary>
        public static void CheckAntd() {
            WriteLine("Checking Antd Units:");
            WriteLine(Execute($"systemctl status {global.units.name.prepare}"));
            WriteLine("-------");
            WriteLine(Execute($"systemctl status {global.units.name.prepare}"));
            WriteLine("-------");
            WriteLine(Execute($"systemctl status {global.units.name.prepare}"));
        }

        /// <summary>
        /// 11 - controllo generico di systemctl
        /// </summary>
        public static void CheckSystemctl() {
            WriteLine("Checking Systemct services:");
            var l = Execute("systemctl -a | grep dead").Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray().Length;
            WriteLine($"{l} dead services");
            l = Execute("systemctl -a | grep inactive").Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray().Length;
            WriteLine($"{l} inactive services");
            l = Execute("systemctl -a | grep ' active'").Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray().Length;
            WriteLine($"{l} active services");
            WriteLine("-------");
            WriteLine("Checking Antd Applicative units:");
            var units = Directory.EnumerateFiles(global.unitsDir).ToArray();
            for (int i = 0; i < units.Length; i++) {
                WriteLine(Execute($"systemctl status {units[i]}"));
                WriteLine("-------");
            }
        }
    }
}
