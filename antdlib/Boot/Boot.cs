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

using System.Text.RegularExpressions;
using antdlib.Log;

namespace antdlib.Boot {
    public class RepositoryCheck {
        public static void CheckIfGlobalRepositoryIsWriteable() {
            var bootExtData = Terminal.Terminal.Execute("blkid | grep BootExt");
            if (bootExtData.Length <= 0) return;
            var bootExtDevice = new Regex(".*:").Matches(bootExtData)[0].Value.Replace(":", "").Trim();
            var bootExtUid = new Regex("[\\s]UUID=\"[\\d\\w\\-]+\"").Matches(bootExtData)[0].Value.Replace("UUID=", "").Replace("\"", "").Trim();
            ConsoleLogger.Log($"global repository -> checking");
            var mountResult = Terminal.Terminal.Execute($"cat /proc/mounts | grep '{bootExtDevice} /mnt/cdrom '");
            if (mountResult.Length > 0) {
                if (mountResult.Contains("ro") && !mountResult.Contains("rw")) {
                    ConsoleLogger.Log($"is RO -> remounting");
                    Terminal.Terminal.Execute("Mount -o remount,rw,discard,noatime /mnt/cdrom");
                }
                else if (mountResult.Contains("rw") && !mountResult.Contains("ro")) {
                    ConsoleLogger.Log($"is RW -> ok!");
                }
            }
            else {
                ConsoleLogger.Log("is not mounted -> IMPOSSIBLE");
            }
            ConsoleLogger.Log($"global repository -> {bootExtDevice} - {bootExtUid}");
            ConsoleLogger.Log("global repository -> checked");
        }
    }
}
