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
    public class Action {
        public static string Mount(string arguments, string source, string location) {
            CommandModel command = Command.Launch("mount", arguments + " " + source + " " + location);
            return command.output;
        }

        public static string Mkdir(string arguments, string dir) {
            CommandModel command = Command.Launch("mkdir", arguments + " " + dir);
            return command.output;
        }

        public static string Dhclient(string name) {
            CommandModel command = Command.Launch("dhclient", name);
            return command.output;
        }

        public static string SshKeygen(string arguments) {
            CommandModel command = Command.Launch("ssh-keygen", arguments);
            return command.output;
        }

        public static string Sshd(string arguments) {
            CommandModel command = Command.Launch("/usr/sbin/sshd", arguments);
            return command.output;
        }

        public static string Sleep(string arguments) {
            CommandModel command = Command.Launch("sleep", arguments);
            return command.output;
        }

        public static string QemuNbd(string arguments, string source) {
            CommandModel command = Command.Launch("qemu-nbd", arguments + " " + source);
            return command.output;
        }

        public static string Kpartx(string arguments, string source) {
            CommandModel command = Command.Launch("kpartx", arguments + " " + source);
            return command.output;
        }

        public static string Zpool(string arguments) {
            CommandModel command = Command.Launch("zpool", arguments);
            return command.output;
        }

        public static string Zfs(string arguments) {
            CommandModel command = Command.Launch("zfs", arguments);
            return command.output;
        }

        public static string Rsync(string arguments, string sourceA, string sourceB) {
            CommandModel command = Command.Launch("rsync", arguments + " " + sourceA + " " + sourceB);
            return command.output;
        }

        public static string Remove(string arguments, string source) {
            CommandModel command = Command.Launch("rm", arguments + " " + source);
            return command.output;
        }
    }
}
