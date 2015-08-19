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
using System.IO;
using System.Linq;

namespace antdlib.MountPoint {
    public class Mount {

        public static void WorkingDirectories() {
            var defaults = new string[] {
                Folder.Root,
                Folder.Config,
                Folder.Database,
                Folder.FileRepository,
                Folder.Networkd,
            };
            for (int i = 0; i < defaults.Length; i++) {
                var dir = defaults[i];
                var DIR = SetDIRSPath(dir);
                Directory.CreateDirectory(dir);
                DFP.Set(dir);
                Directory.CreateDirectory(DIR);
                DFP.Set(DIR);
                SetBind(DIR, dir);
            }
        }

        public static void AllDirectories() {
            var mounts = MountRepository.Get();
            if (mounts.Length < 1) {
                var defaults = new string[] {
                    Folder.Root,
                    Folder.Config,
                    Folder.Database,
                    Folder.FileRepository,
                    Folder.Networkd,
                };
                for (int i = 0; i < defaults.Length; i++) {
                    var m = MountRepository.Create(Guid.NewGuid().ToString().Substring(0, 8), Timestamp.Now, defaults[i]);
                    mounts.ToList().Add(m);
                }
            }
            //todo: controllo la DIRS e guardo cosa c'è già...??
            for (int i = 0; i < mounts.Length; i++) {
                var dir = mounts[i].Path;
                var DIR = SetDIRSPath(dir);
                Directory.CreateDirectory(dir);
                DFP.Set(dir);
                Directory.CreateDirectory(DIR);
                DFP.Set(DIR);
                SetBind(DIR, dir);
            }
            for (int i = 0; i < mounts.Length; i++) {
                var dir = mounts[i].Path;
                CheckMount(dir);
            }
        }

        public static void Check() {
            var mounts = MountRepository.Get();
            if (mounts.Length > 0) {
                for (int i = 0; i < mounts.Length; i++) {
                    var dir = mounts[i].Path;
                    CheckMount(dir);
                }
            }
        }

        public static void Dir(string directory) {
            MountRepository.Create(Guid.NewGuid().ToString().Substring(0, 8), Timestamp.Now, directory);
            var DIR = SetDIRSPath(directory);
            SetBind(DIR, directory);
            Check();
        }

        private static void CheckMount(string directory) {
            bool livecdDFP;
            var livecdPath = SetLiveCDPath(directory);
            var livecdTimestamp = DFP.GetTimestamp(livecdPath);
            livecdDFP = (livecdTimestamp == null) ? false : true;
            bool dirsDFP;
            var dirsPath = SetDIRSPath(directory);
            var dirsTimestamp = DFP.GetTimestamp(dirsPath);
            dirsDFP = (dirsTimestamp == null) ? false : true;
            bool directoryDFP;
            var directoryTimestamp = DFP.GetTimestamp(directory);
            directoryDFP = (directoryTimestamp == null) ? false : true;
            if (livecdDFP == false && dirsDFP == true && directoryDFP == true) {
                if (dirsTimestamp == directoryTimestamp) {
                    MountRepository.SetAsMounted(directory);
                }
                else {
                    MountRepository.SetAsDifferentMounted(directory);
                }
            }
            else if (livecdDFP == false && dirsDFP == true && directoryDFP == false) {
                MountRepository.SetAsNotMounted(directory);
            }
            else if (livecdDFP == false && dirsDFP == false && directoryDFP == true) {
                MountRepository.SetAsTMPMounted(directory);
            }
            else if (livecdDFP == false && dirsDFP == false && directoryDFP == false) {
                MountRepository.SetAsError(directory);
            }
            else {
                MountRepository.SetAsError(directory);
            }
        }

        private static void SetBind(string source, string destination) {
            Terminal.Execute($"mount -o bind {source} {destination}");
        }

        public static string SetDIRSPath(string source) {
            return Path.Combine(Folder.Dirs, $"DIR{source.Replace("/", "_")}").Replace("\\", "/");
        }

        private static string SetLiveCDPath(string source) {
            return Path.Combine(Folder.LiveCd, source).Replace("\\", "/");
        }
    }
}
