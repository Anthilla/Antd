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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using Antd.MountPoint;

namespace Antd.Apps {
    public class AppsManagement {
        public class AppInfo {
            public string Name { get; set; }
            public string Repository { get; set; }
            public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
            public bool IsSetup { get; set; }
        }

        /// <summary>
        /// find apps not yet installed
        /// </summary>
        /// <returns></returns>
        public static AppInfo[] Detect() {
            var appinfoFiles = Directory.EnumerateFiles(Parameter.RepoApps, "*.appinfo", SearchOption.AllDirectories).Select(MapFromFile).ToArray();
            return appinfoFiles;
        }

        public static void Setup(string appName) {
            var apps = Detect();
            var appInfo = apps.FirstOrDefault(_ => _.Name == appName);
            if (appInfo != null) {
                var name = appInfo.Values["name"];
                var repoPath = appInfo.Repository;

                var timestamp = DateTime.Now.ToString("yyyyMMdd");
                var squashName = $"DIR_framework_{name.ToLower().Replace("/", "-")}-{timestamp}.squashfs.xz";
                Terminal.Execute($"mksquashfs {repoPath}/{name} {repoPath}/{squashName} -comp xz -Xbcj x86 -Xdict-size 75%");
                var activeVersionPath = $"{repoPath}/active-version";
                Terminal.Execute($"ln -s {squashName} {activeVersionPath}");
                var frameworkDir = $"/framework/{name.ToLower().Replace("/", "-")}";
                Directory.CreateDirectory("/framework");
                Directory.CreateDirectory(frameworkDir);
                if (Mounts.IsAlreadyMounted(activeVersionPath, frameworkDir) == false) {
                    Terminal.Execute($"mount -o bind {activeVersionPath} {frameworkDir}");
                }
                AppsUnits.CreatePrepareUnit(name, frameworkDir);
                AppsUnits.CreateMountUnit(name, activeVersionPath, frameworkDir);

                var exes = appInfo.Values.Where(_ => _.Key == "app_exe").Select(_ => _.Value);
                foreach (var exe in exes) {
                    var exePath = Directory.EnumerateFiles(frameworkDir).FirstOrDefault(_ => _.Contains(exe));
                    if (File.Exists(exePath)) {
                        AppsUnits.CreateLauncherUnit(name, exe, exePath);
                    }
                }

                var mounts = appInfo.Values.Where(_ => _.Key == "app_path").Select(_ => _.Value);
                foreach (var mount in mounts) {
                    Mount.Dir(mount);
                }

                //todo save entry in db
            }
        }

        private static AppInfo MapFromFile(string path) {
            if (!File.Exists(path)) {
                return null;
            }
            var rows = File.ReadAllLines(path);
            var dict = new Dictionary<string, string>();
            foreach (var row in rows) {
                var kvp = row.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(kvp[0], kvp[1]);
            }
            var appinfo = new AppInfo { Values = dict };
            if (appinfo.Values.Any()) {
                appinfo.Name = appinfo.Values["name"];
                var repoName = appinfo.Values["repo_name"];
                var repoPath = $"{Parameter.RepoApps}/{repoName}";
                appinfo.Repository = repoPath;
            }
            return appinfo;
        }
    }
}
