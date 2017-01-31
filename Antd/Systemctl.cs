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

using antdlib.common;

namespace Antd {

    public class Systemctl {

        private static readonly Bash Bash = new Bash();

        public static void DaemonReload() {
            Bash.Execute("systemctl daemon-reload", false);
        }

        public static void Start(string unit) {
            Bash.Execute("systemctl start " + unit, false);
        }

        public static void Stop(string unit) {
            Bash.Execute("systemctl stop " + unit, false);
        }

        public static void Restart(string unit) {
            Bash.Execute("systemctl restart " + unit, false);
        }

        public static void Reload(string unit) {
            Bash.Execute("systemctl reload " + unit, false);
        }

        public static string Status(string unit) {
            return Bash.Execute("systemctl status " + unit);
        }

        public static bool IsActive(string unit) {
            var r = Bash.Execute("systemctl is-active " + unit);
            return r != "inactive";
        }

        public static bool IsEnabled(string unit) {
            var r = Bash.Execute("systemctl is-enabled " + unit);
            return r != "disabled";
        }

        public static void Enable(string unit) {
            Bash.Execute("systemctl enable " + unit, false);
        }

        public static void Disable(string unit) {
            Bash.Execute("systemctl disable " + unit, false);
        }
    }
}
