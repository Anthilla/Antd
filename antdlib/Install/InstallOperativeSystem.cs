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

using System.IO;
using antdlib.common;

namespace antdlib.Install {
    public class InstallOperativeSystem {

        private static string _diskname;
        private static string _diskBiosBoot;
        private static string _diskEfi;
        private static string _diskData;
        private static string _tmpDataFolder = "/Data/newsystem";

        public InstallOperativeSystem(string disk) {
            ConsoleLogger.Log($"AOS INSTALLATION on: {_diskname}");
            ConsoleLogger.Log("setting parameters...");
            _diskname = $"{disk}";
            _diskBiosBoot = $"{disk}1";
            _diskEfi = $"{disk}2";
            _diskData = $"{disk}3";
            Directory.CreateDirectory(_tmpDataFolder);
            SetDiskAndInstall();
        }

        public void SetDiskAndInstall() {
            ConsoleLogger.Log($"creating partitions on: {_diskname}");
            const string n = "\n";
            var fdiskOptions1 = $"p{n}n{n}1{n}1M{n}{n}{n}t{n}1{n}4{n}{n}{n}w{n}";
            Terminal.Execute($"echo -e \"{fdiskOptions1}\" | fdisk {_diskname}");
            ConsoleLogger.Log($"{_diskBiosBoot} created");
            var fdiskOptions2 = $"p{n}n{n}2{n}-512M{n}{n}{n}t{n}2{n}1{n}{n}{n}w{n}";
            Terminal.Execute($"echo -e \"{fdiskOptions2}\" | fdisk {_diskname}");
            ConsoleLogger.Log($"{_diskEfi} created");
            var fdiskOptions3 = $"p{n}n{n}3{n}-32G{n}{n}{n}t{n}3{n}15{n}{n}{n}w{n}";
            Terminal.Execute($"echo -e \"{fdiskOptions3}\" | fdisk {_diskname}");
            ConsoleLogger.Log($"{_diskData} created");

            Terminal.Execute($"parted -a optimal -s {_diskname} name 2 \"EFI System Partition\"");
            Terminal.Execute($"parted -a optimal -s {_diskname} name 3 BootExt");

            Terminal.Execute($"mkfs.ext4 {_diskData} -L BootExt");
            Terminal.Execute($"mkfs.fat -n EFI {_diskEfi}");

            ConsoleLogger.Log("Copying files (this step will take a few minutes)");
            Terminal.Execute($"mount -o discard,noatime,rw {_diskData} {_tmpDataFolder}");
            Terminal.Execute($"rsync -aHAz /mnt/cdrom/ {_tmpDataFolder}");
            Terminal.Execute($"grub2-install {_diskname}");
            ConsoleLogger.Log("INSTALLATION DONE");
        }
    }
}
