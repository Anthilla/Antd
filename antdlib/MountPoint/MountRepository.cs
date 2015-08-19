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
using System.Linq;
using System.Collections.Generic;

namespace antdlib.MountPoint {
    public class MountRepository {
        public static MountModel[] Get() {
            var list = new List<MountModel>() { };
            foreach (var mountItem in DeNSo.Session.New.Get<MountModel>().ToArray()) {
                list.Add(mountItem);
            }
            return list.ToArray();
        }

        public static MountModel Get(string path) {
            return DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
        }

        public static MountModel Create(string guid, string timestamp, string path) {
            var exMount = Get(path);
            if (exMount != null) {
                return exMount;
            }
            var mount = new MountModel() {
                _Id = Guid.NewGuid().ToString(),
                DFPGuid = guid,
                DFPTimestamp = timestamp,
                Path = path
            };
            DeNSo.Session.New.Set(mount);
            return mount;
        }

        public static void SetAsMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount != null) {
                mount.MountStatus = MountStatus.Mounted;
                DeNSo.Session.New.Set(mount);
            }
        }

        public static void SetAsNotMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount != null) {
                mount.MountStatus = MountStatus.Unmounted;
                DeNSo.Session.New.Set(mount);
            }
        }

        public static void SetAsTMPMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount != null) {
                mount.MountStatus = MountStatus.MountedTMP;
                DeNSo.Session.New.Set(mount);
            }
        }

        public static void SetAsDifferentMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount != null) {
                mount.MountStatus = MountStatus.DifferentMount;
                DeNSo.Session.New.Set(mount);
            }
        }

        public static void SetAsError(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount != null) {
                mount.MountStatus = MountStatus.Error;
                DeNSo.Session.New.Set(mount);
            }
        }
    }
}
