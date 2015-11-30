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
using System.Linq;
using System.Collections.Generic;
using antdlib.Common;

namespace antdlib.MountPoint {
    public class MountLocal {
        public static IEnumerable<MountModel> Get() {
            var list = new List<MountModel>();
            var deny = new List<string>();
            var procmounts = FileSystem.ReadFile("/proc/mounts");
            if (procmounts.Length > 0) {
                var rows = procmounts.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                foreach (var row in rows) {
                    var cells = row.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (cells.Length <= 0)
                        continue;
                    var mm = new MountModel {
                        Device = cells[0].Trim(),
                        Path = cells[1].Trim(),
                        MountContext = MountContext.Other,
                        Type = cells[2].Trim(),
                        Options = cells[3].Trim()
                    };
                    if (mm.Options.Contains("rw")) {
                        mm.MountStatus = MountStatus.MountedReadWrite;
                    }
                    else if (mm.Options.Contains("ro")) {
                        mm.MountStatus = MountStatus.MountedReadOnly;
                    }
                    else {
                        mm.MountStatus = MountStatus.Mounted;
                    }

                    if (mm.Type.Contains("squash")) {
                        mm.MountedPath = GetSquashMount(mm.Device);
                    }
                    else {
                        var mntpt = GetBindMount(mm.Path);
                        if (mntpt == null) {
                            mm.MountedPath = "";
                        }
                        else {
                            mm.MountedPath = mntpt;
                            deny.Add(mntpt.Trim());
                        }
                    }
                    list.Add(mm);
                }
            }

            var preList = list.Where(m => !deny.Contains(m.Path)).OrderBy(m => m.Device).ThenBy(m => m.Path).ToList();

            foreach (var el in deny.Select(d => preList.FirstOrDefault(m => m.Path == d)).Where(el => el != null)) {
                preList.Remove(el);
            }

            return preList;
        }

        public static string GetSquashMount(string device) {
            var sq = Terminal.Terminal.Execute($"losetup | grep {device}");
            if (sq.Length <= 0)
                return "";
            var data = sq.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var src = "";
            if (data.Length > 1) {
                src = data[data.Length - 1];
            }
            return src;
        }

        public static string GetBindMount(string path) {
            var mnt = MountRepository.Get(path);
            return mnt.MountedPath.Length > 0 ? mnt.MountedPath : null;
        }
    }
}
