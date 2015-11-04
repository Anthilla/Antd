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
using System.Reflection;
using System.Runtime.InteropServices;

namespace antdlib {

    public class Label {
        public static string Root => "antd_root";
        public static string Port => "antd_port";
        public static string Database => "antd_database";
        public static string Files => "antd_files";

        public class SMTP {
            public static string Url => "smtp_url";
            public static string Port => "smtp_port";
            public static string Account => "smtp_account";
            public static string Password => "smtp_password";
        }

        public class IMAP {
            public static string Url => "imap_url";
            public static string Port => "imap_port";
            public static string Account => "imap_account";
            public static string Password => "imap_password";
        }

        public class Auth {
            public static string IsEnabled => "isenabled";
        }
    }

    public class Folder {
        public static string Root => "/cfg/antd";
        public static string Database => Root + "/database";
        public static string Networkd => Root + "/networkd";
        //todo: as soon as there's systemd managed websocketd.target change this value v
        public static string Websocketd => AppsUnits;
        public static string AntdRepo => "/mnt/cdrom";
        public static string Apps => "/mnt/cdrom/Apps";
        public static string Dirs => "/mnt/cdrom/DIRS";
        public static string Config => "/mnt/cdrom/Config";
        public static string LiveCd => "/mnt/livecd";
        public static string AppsUnits => $"{AntdRepo}/Units/applicative.target.wants";
        public static string kernelDir => $"{AntdRepo}/Kernel";
        public static string systemDir => $"{AntdRepo}/System";
        public static string AntdVersionsDir => $"{Apps}/Anthilla_Antd";
        public static string AntdshVersionsDir => $"{Apps}/Anthilla_antdsh";
        public static string AntdTmpDir => $"{Apps}/tmp";
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
        public static string Guid => System.Guid.NewGuid().ToString();
        public static string ShortGuid => System.Guid.NewGuid().ToString().Substring(0, 8);
    }

    public class Port {
        public static string Antd => "7777";
        public static string Websocket => "8888";
    }

    public class Uri {
        public static string Antd => "http://+:" + Port.Antd + "/";
    }

    public class Url {
        public static string Antd => "http://localhost:" + Port.Antd + "/";
    }

    public class AssemblyInfo {
        public const string dateFormat = "yyyyMMdd";

        private static string GetGuid() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return attribute.Value;
        }

        public static string Guid => GetGuid();

        public static OperatingSystem OS => Environment.OSVersion;

        public static PlatformID Platform => Environment.OSVersion.Platform;

        public static bool IsUnix => (Platform == PlatformID.Unix);
    }

    public class Units {

        public class Name {
            public static string prepare => "app-antd-01-prepare.service";
            public static string mount => "app-antd-02-mount.service";
            public static string launch => "app-antd-03-launcher.service";
        }

        public static string prepare = $"{Folder.AppsUnits}/{Name.prepare}";
        public static string mount = $"{Folder.AppsUnits}/{Name.mount}";
        public static string launch = $"{Folder.AppsUnits}/{Name.launch}";
    }

    public class Update {
        public const string remoteRepo = "http://srv.anthilla.com:8081";
        public const string remoteAntdDir = "antd-update";
        public const string remoteAntdshDir = "antdsh-update";
        public const string remoteUpdateInfo = "update.txt";
    }
}
