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
using System.Reflection;
using System.Runtime.InteropServices;

namespace antdlib {

    public class Label {
        public static string Root { get { return "antd_root"; } }
        public static string Port { get { return "antd_port"; } }
        public static string Database { get { return "antd_database"; } }
        public static string Files { get { return "antd_files"; } }

        public class SMTP {
            public static string Url { get { return "smtp_url"; } }
            public static string Port { get { return "smtp_port"; } }
            public static string Account { get { return "smtp_account"; } }
            public static string Password { get { return "smtp_password"; } }
        }

        public class IMAP {
            public static string Url { get { return "imap_url"; } }
            public static string Port { get { return "imap_port"; } }
            public static string Account { get { return "imap_account"; } }
            public static string Password { get { return "imap_password"; } }
        }

        public class Auth {
            public static string IsEnabled { get { return "isenabled"; } }
        }
    }

    public class Folder {
        public static string Root { get { return "/cfg/antd"; } }
        public static string Database { get { return "/Data/antd"; } }
        public static string Networkd { get { return Folder.Root + "/networkd"; } }
        public static string AntdRepo { get { return "/mnt/cdrom"; } }
        public static string Apps { get { return "/mnt/cdrom/Apps"; } }
        public static string Dirs { get { return "/mnt/cdrom/DIRS"; } }
        public static string LiveCd { get { return "/mnt/livecd"; } }
        public static string AppsUnits { get { return $"{AntdRepo}/Units/applicative.target.wants"; } }
        public static string kernelDir { get { return $"{AntdRepo}/Kernel"; } }
        public static string systemDir { get { return $"{AntdRepo}/System"; } }
        public static string AntdVersionsDir { get { return $"{Apps}/Anthilla_Antd"; } }
        public static string AntdshVersionsDir { get { return $"{Apps}/Anthilla_antdsh"; } }
        public static string AntdTmpDir { get { return $"{Apps}/tmp"; } }
    }

    public class AntdFile {
        public const string NetworkConfig = "antd.boot.network.conf";
        public const string FirewallConfig = "antd.boot.firewall.conf";
        public const string zipStartsWith = "antd";
        public const string zipEndsWith = ".7z";
        public const string squashStartsWith = "DIR_framework_antd-";
        public const string squashEndsWith = ".squashfs.xz";
        public const string configFile = "antdsh.config";
        public const string antdRunning = "active-version";
        public const string downloadName = "antdDownload.zip";
        public const string downloadFirstDir = "antdDownloadFirst";
    }

    public class UID {
        public static string Guid { get { return System.Guid.NewGuid().ToString(); } }
        public static string ShortGuid { get { return System.Guid.NewGuid().ToString().Substring(0, 8); } }
    }

    public class Port {
        public static string Antd { get { return "7777"; } }
        public static string Websocket { get { return "8888"; } }
    }

    public class Uri {
        public static string Antd { get { return "http://+:" + Port.Antd + "/"; } }
    }

    public class Url {
        public static string Antd { get { return "http://localhost:" + Port.Antd + "/"; } }
    }

    public class AssemblyInfo {
        public const string dateFormat = "yyyyMMdd";

        private static string GetGuid() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return attribute.Value;
        }

        public static string Guid { get { return GetGuid(); } }

        public static OperatingSystem OS { get { return Environment.OSVersion; } }

        public static PlatformID Platform { get { return Environment.OSVersion.Platform; } }

        public static bool IsUnix { get { return (Platform == PlatformID.Unix); } }
    }

    public class Units {

        public class Name {
            public static string prepare { get { return "anthillasp-prepare.service"; } }
            public static string mount { get { return "framework-anthillasp.mount"; } }
            public static string launch { get { return "anthillasp-launcher.service"; } }
        }

        public static string prepare { get { return $"{Folder.AppsUnits}/{Name.prepare}"; } }
        public static string mount { get { return $"{Folder.AppsUnits}/{Name.mount}"; } }
        public static string launch { get { return $"{Folder.AppsUnits}/{Name.launch}"; } }
    }

    public class Update {
        public const string remoteRepo = "http://srv.anthilla.com:8081";
        public const string remoteAntdDir = "antd-update";
        public const string remoteAntdshDir = "antdsh-update";
        public const string remoteUpdateInfo = "update.txt";
    }
}
