//-------------------------------------------------------------------------------------
// Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// * Redistributions of source code must retain the above copyright
//   notice, this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer in the
//   documentation and/or other materials provided with the distribution.
// * Neither the name of the Anthilla S.r.l. nor the
//   names of its contributors may be used to endorse or promote products
//   derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// 20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.common.Tool;
using Antd.Database;
using Antd.MountPoint;

namespace Antd.Apps {
    public class AppsManagement {
        public class AppInfo {
            public string Name { get; set; }
            public string Repository { get; set; }
            public List<KeyValuePair<string, string>> Values { get; set; } = new List<KeyValuePair<string, string>>();
            public bool IsSetup { get; set; }
        }

        private static readonly ApplicationRepository ApplicationRepository = new ApplicationRepository();

        public IEnumerable<AppInfo> Detect() {
            var allDetected = Directory.EnumerateFiles(Parameter.RepoApps, "*.appinfo", SearchOption.AllDirectories).Select(MapFromFile).ToArray();
            var notInstalled = new List<AppInfo>();
            foreach(var app in allDetected) {
                var tryGet = ApplicationRepository.GetByName(app.Name);
                if(tryGet == null) {
                    notInstalled.Add(app);
                }
            }
            return notInstalled;
        }

        private readonly Bash _bash = new Bash();
        private readonly AppsUnits _appsUnits = new AppsUnits();
        private readonly Mount _mount = new Mount();

        public void Setup(string appName) {
            ConsoleLogger.Log("=========================================");
            ConsoleLogger.Log($"Installing {appName}");
            var apps = Detect();
            var appInfo = apps.FirstOrDefault(_ => _.Name == appName);
            if(appInfo != null) {
                ConsoleLogger.Log($"{appName} info found");
                var name = appInfo.Values.FirstOrDefault(_ => _.Key == "name").Value;
                var repoPath = appInfo.Repository;
                var timestamp = DateTime.Now.ToString("yyyyMMdd");
                var squashName = $"DIR_framework_{name.ToLower().Replace("/", "-")}-aosApps-{timestamp}-std-x86_64.squashfs.xz";
                ConsoleLogger.Log($"name => {name}");
                ConsoleLogger.Log($"repoPath => {repoPath}");
                ConsoleLogger.Log($"timestamp => {timestamp}");
                ConsoleLogger.Log($"squashName => {squashName}");
                if(File.Exists($"{repoPath}/{squashName}")) {
                    File.Delete($"{repoPath}/{squashName}");
                }
                ConsoleLogger.Log($">> mksquashfs {repoPath}/{name} {repoPath}/{squashName} -comp xz -Xbcj x86 -Xdict-size 75%");
                _bash.Execute($"mksquashfs {repoPath}/{name} {repoPath}/{squashName} -comp xz -Xbcj x86 -Xdict-size 75%", false);
                ConsoleLogger.Log("compressed fs for application created");
                var activeVersionPath = $"{repoPath}/active-version";
                ConsoleLogger.Log($"activeVersionPath => {activeVersionPath}");
                _bash.Execute($"ln -s {squashName} {activeVersionPath}", false);
                ConsoleLogger.Log("link created");
                var frameworkDir = $"/framework/{name.ToLower().Replace("/", "-")}";
                ConsoleLogger.Log($"frameworkDir => {frameworkDir}");
                Directory.CreateDirectory("/framework");
                Directory.CreateDirectory(frameworkDir);
                ConsoleLogger.Log("framework directories created");
                if(Mounts.IsAlreadyMounted(frameworkDir) == false) {
                    ConsoleLogger.Log($">> mount {activeVersionPath} {frameworkDir}");
                    _bash.Execute($"mount {activeVersionPath} {frameworkDir}", false);
                    ConsoleLogger.Log("application fs mounted");
                }
                var prepareUnitName = _appsUnits.CreatePrepareUnit(name, frameworkDir);
                var mountUnitName = _appsUnits.CreateMountUnit(name, activeVersionPath, frameworkDir);
                ConsoleLogger.Log($"prepareUnitName => {prepareUnitName}");
                ConsoleLogger.Log($"mountUnitName => {mountUnitName}");
                ConsoleLogger.Log("units created");

                var launcherUnitName = new List<string>();
                var exes = appInfo.Values.Where(_ => _.Key == "app_exe").Select(_ => _.Value).ToList();
                var frameworkDirContent = Directory.EnumerateFiles(frameworkDir, "*", SearchOption.AllDirectories).ToList();
                foreach(var exe in exes) {
                    ConsoleLogger.Log($"exe => {exe}");
                    var exePath = frameworkDirContent.FirstOrDefault(_ => _.EndsWith(exe));
                    ConsoleLogger.Log($"exePath? => {exePath}");
                    if(File.Exists(exePath)) {
                        var lun = _appsUnits.CreateLauncherUnit(name, exe, exePath);
                        ConsoleLogger.Log($"launcherUnitName => {lun}");
                        launcherUnitName.Add(lun);
                    }
                }
                ConsoleLogger.Log("launcher units created");

                var mounts = appInfo.Values.Where(_ => _.Key == "app_path").Select(_ => _.Value).ToList();
                foreach(var mount in mounts) {
                    ConsoleLogger.Log($"workingDir => {mount}");
                    _mount.Dir(mount);
                }
                ConsoleLogger.Log("working directories created and mounted");

                var tryGet = ApplicationRepository.GetByName(name);
                if(tryGet == null) {
                    var dict = new Dictionary<string, string> {
                        { "Name", name},
                        { "RepositoryName", repoPath},
                        { "Exes", exes.JoinToString()},
                        { "WorkingDirectories", mounts.JoinToString()},
                        { "UnitPrepare", prepareUnitName},
                        { "UnitMount", mountUnitName},
                        { "UnitLauncher", launcherUnitName.JoinToString()}
                        };
                    ApplicationRepository.Create(dict);
                    ConsoleLogger.Log("info recorded in db");
                }
            }
            ConsoleLogger.Log("=========================================");
        }

        private static AppInfo MapFromFile(string path) {
            if(!File.Exists(path)) {
                return null;
            }
            var rows = File.ReadAllLines(path);
            var dict = new List<KeyValuePair<string, string>>();
            foreach(var row in rows) {
                var v = row.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
                var kvp = new KeyValuePair<string, string>(v[0].Trim(), v[1].Trim());
                dict.Add(kvp);
            }
            var appinfo = new AppInfo { Values = dict };
            if(appinfo.Values.Any()) {
                appinfo.Name = appinfo.Values.FirstOrDefault(_ => _.Key == "name").Value.Trim();
                var repoName = appinfo.Values.FirstOrDefault(_ => _.Key == "repo_name").Value.Trim();
                var repoPath = $"{Parameter.RepoApps}/{repoName}";
                appinfo.Repository = repoPath;
            }
            return appinfo;
        }
    }
}
