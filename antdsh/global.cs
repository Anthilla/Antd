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

namespace antdsh {
    public class global {
        public static string dir { get { return "/mnt/cdrom/DIRS"; } }
        public static string versionsDir { get { return "/mnt/cdrom/DIRS/DIR_framework_antd"; } }
        public static string tmpDir { get { return "/mnt/cdrom/DIRS/DIR_framework_tmp"; } }
        public static string appsDir { get { return "/mnt/cdrom/Apps"; } }
        public static string unitsDir { get { return "/mnt/cdrom/Units/applicative.target.wants"; } }
        public const string configFile = "antdsh.config";
        public const string antdRunning = "running";
        public const string downloadName = "antdDownload.zip";
        public const string downloadFirstDir = "antdDownloadFirst";

        public const string zipStartsWith = "antd";
        public const string zipEndsWith = ".7z";
        public const string squashStartsWith = "DIR_framework_antd";
        public const string squashEndsWith = ".squashfs.xz";

        public const string dateFormat = "yyyyMMdd";

        public class system {
            public static string kernelDir { get { return "/mnt/cdrom/Kernel"; } }

            public static string systemDir { get { return "/mnt/cdrom/System"; } }


        }

        public class units {

            public class name {
                public static string prepare { get { return "anthillasp-prepare.service"; } }
                public static string mount { get { return "framework-anthillasp.mount"; } }
                public static string launch { get { return "anthillasp-launcher.service"; } }
            }

            public static string prepare { get { return $"{unitsDir}/{name.prepare}"; } }
            public static string mount { get { return $"{unitsDir}/{name.mount}"; } }
            public static string launch { get { return $"{unitsDir}/{name.launch}"; } }
        }
    }
}
