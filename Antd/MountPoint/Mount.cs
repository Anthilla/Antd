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
using antdlib.views;

namespace Antd.MountPoint {
    public class Mount {

        private static readonly Database.MountRepository MountRepository = new Database.MountRepository();

        private static readonly string[] DefaultWorkingDirectories = { Parameter.AntdCfg, Parameter.EtcSsh };

        public static void WorkingDirectories() {
            foreach (var dir in DefaultWorkingDirectories) {
                var mntDir = Mounts.SetDirsPath(dir);
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(mntDir);
                if (Mounts.IsAlreadyMounted(dir) == false) {
                    ConsoleLogger.Log($"mount {mntDir} -> {dir}");
                    SetBind(mntDir, dir);
                }
            }
        }

        public static void AllDirectories() {
            var all = MountRepository.GetAll().ToList();
            if (!all.Any()) {
                foreach (var path in DefaultWorkingDirectories.Where(path => MountRepository.GetByPath(path) == null)) {
                    MountRepository.Create(new Dictionary<string, string> {
                        {"Path", path},
                        {"MountContext", MountContext.Core.ToString()},
                        {"MountEntity", MountEntity.Directory.ToString()}
                    });
                }
            }
            ConsoleLogger.Log("stored mount info checked");

            RemoveDuplicates();
            CheckCurrentStatus();
            ConsoleLogger.Log("current mount status checked");

            var directoryMounts = all.Where(m => m.MountEntity == MountEntity.Directory.ToString()).ToArray();
            foreach (var t in directoryMounts) {
                try {
                    var dir = t.Path.Replace("\\", "");
                    var mntDir = Mounts.SetDirsPath(dir);
                    if (Mounts.IsAlreadyMounted(dir) == false) {
                        Directory.CreateDirectory(dir);
                        Directory.CreateDirectory(mntDir);
                        ConsoleLogger.Log($"mount {mntDir} -> {dir}");
                        SetBind(mntDir, dir);
                    }
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
            ConsoleLogger.Log("directories mounted");

            var fileMounts = all.Where(m => m.MountEntity == MountEntity.File.ToString()).ToArray();
            foreach (var t in fileMounts) {
                var file = t.Path.Replace("\\", "");
                var mntFile = Mounts.SetFilesPath(file);
                if (System.IO.File.Exists(mntFile)) {
                    var path = Path.GetDirectoryName(file);
                    var mntPath = Path.GetDirectoryName(mntFile);
                    if (Mounts.IsAlreadyMounted(file) == false) {
                        Terminal.Execute($"mkdir -p {path}");
                        Terminal.Execute($"mkdir -p {mntPath}");
                        if (!System.IO.File.Exists(file)) {
                            Terminal.Execute($"cp {mntFile} {file}");
                        }
                        ConsoleLogger.Log($"mount {mntFile} -> {file}");
                        SetBind(mntFile, file);
                    }
                }
            }
            ConsoleLogger.Log("files mounted");

            foreach (var t in directoryMounts) {
                CheckMount(t.Path);
            }
            ConsoleLogger.Log("detected directories status checked");

            foreach (var srvc in from t in directoryMounts select t.AssociatedUnits into service where service.Any() from srvc in service select srvc) {
                Terminal.Execute($"systemctl restart {srvc}");
            }
            ConsoleLogger.Log("services restarted");
        }

        private static void RemoveDuplicates() {
            var all = MountRepository.GetAll().ToList();
            var sorted = all.GroupBy(a => a.Path);
            foreach (var group in sorted) {
                if (group.Count() > 1) {
                    var old = group.OrderByDescending(_ => _.Timestamp).Skip(1);
                    foreach (var g in old) {
                        MountRepository.Delete(g.Id);
                    }
                }
            }
        }

        public static void CheckCurrentStatus() {
            var directories = Directory.EnumerateDirectories(Parameter.RepoDirs, "DIR*", SearchOption.TopDirectoryOnly).ToArray();
            foreach (var t in directories) {
                var realPath = Mounts.GetDirsPath(t);
                var mount = MountRepository.GetByPath(realPath);
                if (mount == null) {
                    MountRepository.Create(new Dictionary<string, string> {
                        {"Path", realPath},
                        {"MountContext", MountContext.External.ToString()},
                        {"MountEntity", MountEntity.Directory.ToString()}
                    });
                }
                try {
                    Terminal.Execute($"mkdir -p {t}");
                    Terminal.Execute($"mkdir -p {realPath}");
                    Terminal.Execute($"cp {t} {realPath}");
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
            ConsoleLogger.Log("current directories status checked");

            var files = Directory.EnumerateFiles(Parameter.RepoDirs, "FILE*", SearchOption.TopDirectoryOnly).ToArray();
            foreach (var t in files) {
                var realPath = Mounts.GetFilesPath(t);
                var mount = MountRepository.GetByPath(realPath);
                if (mount == null) {
                    MountRepository.Create(new Dictionary<string, string> {
                        {"Path", realPath},
                        {"MountContext", MountContext.External.ToString()},
                        {"MountEntity", MountEntity.File.ToString()}
                    });
                }
                try {
                    var path = t.GetAllStringsButLast('/');
                    var mntPath = realPath.GetAllStringsButLast('/');
                    Terminal.Execute($"mkdir -p {path}");
                    Terminal.Execute($"mkdir -p {mntPath}");
                    Terminal.Execute($"cp {t} {realPath}");
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
            ConsoleLogger.Log("current files status checked");
        }

        public static void Check() {
            var mounts = MountRepository.GetAll().ToList();
            if (!mounts.Any())
                return;
            foreach (var t in mounts) {
                ConsoleLogger.Log($"{t.Path}:");
                CheckMount(t.Path);
            }
        }

        public static void Dir(string directory) {
            var tryget = MountRepository.GetByPath(directory);
            if (tryget == null) {
                MountRepository.Create(new Dictionary<string, string> {
                        {"Path", directory},
                        {"MountContext", MountContext.External.ToString()},
                        {"MountEntity", MountEntity.Directory.ToString()}
                    });
                var mntDir = Mounts.SetDirsPath(directory);
                Directory.CreateDirectory(directory);
                Directory.CreateDirectory(mntDir);
                SetBind(mntDir, directory);
            }
        }

        public static void File(string file) {
            var tryget = MountRepository.GetByPath(file);
            if (tryget == null) {
                MountRepository.Create(new Dictionary<string, string> {
                        {"Path", file},
                        {"MountContext", MountContext.External.ToString()},
                        {"MountEntity", MountEntity.File.ToString()}
                    });
                var mntFile = Mounts.SetFilesPath(file);
                SetBind(mntFile, file);
            }
        }

        private static void CheckMount(string directory) {
            var isMntd = Mounts.IsAlreadyMounted(directory);
            var mntDirectory = Mounts.SetDirsPath(directory);
            var timestampNow = Timestamp.Now;
            Dfp.Set(mntDirectory, timestampNow);
            Dfp.Set(directory, timestampNow);
            var dirsTimestamp = Dfp.GetTimestamp(mntDirectory);
            var dirsDfp = dirsTimestamp != null;
            var directoryTimestamp = Dfp.GetTimestamp(directory);
            var directoryDfp = directoryTimestamp != null;
            if (isMntd && directoryTimestamp == "unauthorizedaccessexception" && dirsTimestamp == "unauthorizedaccessexception") {
                MountRepository.SetAsMountedReadOnly(directory);
            }
            else if (isMntd && dirsDfp && directoryDfp) {
                if (dirsTimestamp == directoryTimestamp) {
                    MountRepository.SetAsMounted(directory, mntDirectory);
                }
                else {
                    MountRepository.SetAsDifferentMounted(directory);
                }
            }
            else if (isMntd == false && dirsDfp && directoryDfp == false) {
                MountRepository.SetAsNotMounted(directory);
            }
            else if (isMntd && dirsDfp == false && directoryDfp) {
                MountRepository.SetAsTmpMounted(directory);
            }
            else if (isMntd == false && dirsDfp == false && directoryDfp == false) {
                MountRepository.SetAsError(directory);
            }
            else {
                MountRepository.SetAsError(directory);
            }
            Dfp.Delete(mntDirectory);
            Dfp.Delete(directory);
        }

        private static void SetBind(string source, string destination) {
            if (Mounts.IsAlreadyMounted(source, destination))
                return;
            Terminal.Execute($"mount -o bind {source} {destination}");
        }
    }
}
