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
using antdlib.common.Helpers;
using antdlib.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public  static class MountManagement {

        private static readonly string[] DefaultWorkingDirectories = { Parameter.AntdCfg, Parameter.EtcSsh };

        private static readonly Bash Bash = new Bash();

        private static readonly Dictionary<string, string> DefaultWorkingDirectoriesWithOptions = new Dictionary<string, string> {
            {"/dev/shm", "-o remount,nodev,nosuid,mode=1777"},
            {"/hugepages", "-o mode=1770,gid=78 -t hugetlbfs hugetlbfs"},
            {"/sys/kernel/dlm", "-o default -t ocfs2_dlmfs dlm"}
        };

        public  static void WorkingDirectories() {
            foreach(var dir in DefaultWorkingDirectories) {
                var mntDir = MountHelper.SetDirsPath(dir);
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(mntDir);
                if(MountHelper.IsAlreadyMounted(dir))
                    continue;
                ConsoleLogger.Log($"mount {mntDir} -> {dir}");
                SetBind(mntDir, dir);
            }
            foreach(var kvp in DefaultWorkingDirectoriesWithOptions) {
                if(MountHelper.IsAlreadyMounted(kvp.Key) == false) {
                    Bash.Execute($"mount {kvp.Value} {kvp.Key}", false);
                }
            }
        }

        public  static List<MountModel> GetAll() {
            var list = new List<MountModel>();
            var directories = Directory.EnumerateDirectories(Parameter.RepoDirs, "DIR*", SearchOption.TopDirectoryOnly).ToArray();
            foreach(var directory in directories) {
                var mo = new MountModel {
                    SystemPath = MountHelper.GetDirsPath(Path.GetFileName(directory)),
                    RepoDirsPath = directory,
                    Context = MountContext.External,
                    Entity = MountEntity.Directory
                };
                list.Add(mo);
            }
            var files = Directory.EnumerateFiles(Parameter.RepoDirs, "FILE*", SearchOption.TopDirectoryOnly).ToArray();
            foreach(var file in files) {
                var mo = new MountModel {
                    SystemPath = MountHelper.GetFilesPath(Path.GetFileName(file)),
                    RepoDirsPath = file,
                    Context = MountContext.External,
                    Entity = MountEntity.File
                };
                list.Add(mo);

            }
            return list;
        }

        public  static void AllDirectories() {
            var list = new List<MountModel>();
            var directories = Directory.EnumerateDirectories(Parameter.RepoDirs, "DIR*", SearchOption.TopDirectoryOnly).ToArray();
            foreach(var directory in directories) {
                var mo = new MountModel {
                    SystemPath = MountHelper.GetDirsPath(Path.GetFileName(directory)),
                    RepoDirsPath = directory,
                    Context = MountContext.External,
                    Entity = MountEntity.Directory
                };
                list.Add(mo);
            }
            var files = Directory.EnumerateFiles(Parameter.RepoDirs, "FILE*", SearchOption.TopDirectoryOnly).ToArray();
            foreach(var file in files) {
                var mo = new MountModel {
                    SystemPath = MountHelper.GetFilesPath(Path.GetFileName(file)),
                    RepoDirsPath = file,
                    Context = MountContext.External,
                    Entity = MountEntity.File
                };
                list.Add(mo);

            }
            ConsoleLogger.Log("directories and files enumerated");

            var directoryMounts = list.Where(m => m.Entity == MountEntity.Directory).ToList();
            foreach(var directoryMount in directoryMounts) {
                try {
                    var dir = directoryMount.SystemPath.Replace("\\", "");
                    var mntDir = directoryMount.RepoDirsPath;
                    if(MountHelper.IsAlreadyMounted(dir) == false) {
                        Directory.CreateDirectory(dir);
                        Directory.CreateDirectory(mntDir);
                        SetBind(mntDir, dir);
                        ConsoleLogger.Log($"mount {mntDir} -> {dir}");
                    }
                }
                catch(Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
            ConsoleLogger.Log("directories mounted");

            var fileMounts = list.Where(m => m.Entity == MountEntity.File).ToList();
            foreach(var fileMount in fileMounts) {
                var file = fileMount.SystemPath.Replace("\\", "");
                var mntFile = fileMount.RepoDirsPath;
                if(System.IO.File.Exists(mntFile)) {
                    var path = Path.GetDirectoryName(file);
                    var mntPath = Path.GetDirectoryName(mntFile);
                    if(MountHelper.IsAlreadyMounted(file) == false) {
                        Bash.Execute($"mkdir -p {path}", false);
                        Bash.Execute($"mkdir -p {mntPath}", false);
                        if(!System.IO.File.Exists(file)) {
                            Bash.Execute($"cp {mntFile} {file}", false);
                        }
                        SetBind(mntFile, file);
                        ConsoleLogger.Log($"mount {mntFile} -> {file}");
                    }
                }
            }
            ConsoleLogger.Log("files mounted");
        }

        public  static void Dir(string directory) {
            var mntDir = MountHelper.SetDirsPath(directory);
            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(mntDir);
            SetBind(mntDir, directory);
        }

        public  static void File(string file) {
            var mntFile = MountHelper.SetFilesPath(file);
            SetBind(mntFile, file);
        }

        //private static readonly Dfp _dfp = new Dfp();

        //private static void CheckMount(string directory) {
        //    var isMntd = MountHelper.IsAlreadyMounted(directory);
        //    var mntDirectory = MountHelper.SetDirsPath(directory);
        //    var timestampNow = Timestamp.Now;
        //    _dfp.Set(mntDirectory, timestampNow);
        //    _dfp.Set(directory, timestampNow);
        //    var dirsTimestamp = _dfp.GetTimestamp(mntDirectory);
        //    var dirsDfp = dirsTimestamp != null;
        //    var directoryTimestamp = _dfp.GetTimestamp(directory);
        //    var directoryDfp = directoryTimestamp != null;
        //    if(isMntd && directoryTimestamp == "unauthorizedaccessexception" && dirsTimestamp == "unauthorizedaccessexception") {
        //        MountRepository.SetAsMountedReadOnly(directory);
        //    }
        //    else if(isMntd && dirsDfp && directoryDfp) {
        //        if(dirsTimestamp == directoryTimestamp) {
        //            MountRepository.SetAsMounted(directory, mntDirectory);
        //        }
        //        else {
        //            MountRepository.SetAsDifferentMounted(directory);
        //        }
        //    }
        //    else if(isMntd == false && dirsDfp && directoryDfp == false) {
        //        MountRepository.SetAsNotMounted(directory);
        //    }
        //    else if(isMntd && dirsDfp == false && directoryDfp) {
        //        MountRepository.SetAsTmpMounted(directory);
        //    }
        //    else if(isMntd == false && dirsDfp == false && directoryDfp == false) {
        //        MountRepository.SetAsError(directory);
        //    }
        //    else {
        //        MountRepository.SetAsError(directory);
        //    }
        //    _dfp.Delete(mntDirectory);
        //    _dfp.Delete(directory);
        //}

        private static void SetBind(string source, string destination) {
            if(MountHelper.IsAlreadyMounted(source, destination))
                return;
            Bash.Execute($"mount -o bind {source} {destination}", false);
        }
    }
}
