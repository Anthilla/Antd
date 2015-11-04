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

namespace antdlib.Install {
    public class InstallCheck {
        private static bool IsOnUsb() {
            var cmdResult = Terminal.Execute("lsblk -npl | grep /mnt/cdrom");
            var deviceName = cmdResult.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (deviceName == null)
                throw new ArgumentNullException(nameof(deviceName));
            var diskName = deviceName.Substring(0, deviceName.Length - 1);
            var diskFile = $"/sys/class/block/{diskName.Replace("dev", "").Replace("/", "")}/removable";
            if (!File.Exists(diskFile))
                return false;
            var t = FileSystem.ReadFile(diskFile);
            switch (t.Trim()) {
                case "1":
                    return true;
                case "0":
                    return false;
            }
            return false;
        }

        public static bool IsOsRemovable => IsOnUsb();

        public static bool IsDiskEligibleForOs(string disk) {
            var diskSizeCmdResult = Terminal.Execute($"lsblk -npl --output NAME,SIZE | grep \"{disk} \"");
            var size = diskSizeCmdResult.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (size != null && !size.Contains("G")) {
                //var num = Convert.ToInt32(size.Replace("M", ""));
                var num = Convert.ToInt32(string.Join("", size.Where(char.IsDigit)));
                if (num < 32) {
                    return false;
                }
            }
            var isPartitionCmdResult = Terminal.Execute($"lsblk -npl --output NAME,TYPE | grep \"{disk} \"");
            if (isPartitionCmdResult.Contains("part") && !isPartitionCmdResult.Contains("disk")) {
                return false;
            }
            var hasPartitionCmdResult = Terminal.Execute($"lsblk -npl --output NAME,TYPE | grep {disk}");
            var results = hasPartitionCmdResult.Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
            return results <= 1;
        }
    }
}
