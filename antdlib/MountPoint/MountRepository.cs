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
using antdlib.Log;

namespace antdlib.MountPoint {
    public class MountRepository {
        public static IEnumerable<MountModel> Get() {
            try {
                var list = new List<MountModel>();
                var dbGet = DeNSo.Session.New.Get<MountModel>().ToArray();
                foreach (var mount in dbGet) {
                    mount.DirsPath = Mount.SetDirsPath(mount.Path);
                    mount.HtmlStatusIcon = AssignHtmlStatusIcon(mount.MountStatus);
                    list.Add(mount);
                }
                return list.OrderBy(m => m.MountContext)
                    .Where(_ => !_.Path.Contains("livecd") || !_.DirsPath.Contains("livecd") || !_.MountedPath.Contains("livecd"));
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
                return new MountModel[] { };
            }
        }

        private static string AssignHtmlStatusIcon(MountStatus status) {
            switch (status) {
                case MountStatus.Mounted:
                    return "icon-record fg-green";
                case MountStatus.Unmounted:
                    return "icon-record fg-red";
                case MountStatus.MountedTMP:
                    return "icon-record fg-orange";
                case MountStatus.DifferentMount:
                    return "icon-stop-2 fg-orange";
                case MountStatus.MountedReadOnly:
                    return "icon-stop-2 fg-red";
                case MountStatus.MountedReadWrite:
                    return "icon-stop-2 fg-red";
                case MountStatus.Error:
                    return "icon-stop-2 fg-red";
                default:
                    return "icon-stop-2 fg-red";
            }
        }

        public static MountModel Get(string path) {
            return DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
        }

        public static IEnumerable<MountModel> GetByUnit(string unit) {
            return DeNSo.Session.New.Get<MountModel>(m => m.AssociatedUnits.Contains(unit));
        }

        public static string[] GetListByUnit(string unit) {
            return DeNSo.Session.New.Get<MountModel>(m => m.AssociatedUnits.Contains(unit)).Select(mnt => mnt.Path).ToArray();
        }


        public static MountModel Create(string path, MountContext context, MountEntity entity) {
            var get = Get(path);
            var mntContext = get?.MountContext ?? context;
            var exMount = Get(path);
            if (exMount != null) {
                return exMount;
            }
            var mount = new MountModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                DFPTimestamp = Timestamp.Now,
                MountContext = mntContext,
                MountEntity = entity,
                Path = path
            };
            DeNSo.Session.New.Set(mount);
            return mount;
        }

        public static void SetAsMounted(string path, string mounted) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount == null)
                return;
            mount.MountStatus = MountStatus.Mounted;
            mount.MountedPath = mounted;
            DeNSo.Session.New.Set(mount);
        }

        public static void SetAsNotMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount == null)
                return;
            mount.MountStatus = MountStatus.Unmounted;
            DeNSo.Session.New.Set(mount);
        }

        public static void SetAsTmpMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount == null)
                return;
            mount.MountStatus = MountStatus.MountedTMP;
            DeNSo.Session.New.Set(mount);
        }

        public static void SetAsMountedReadOnly(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount == null)
                return;
            mount.MountStatus = MountStatus.MountedReadOnly;
            DeNSo.Session.New.Set(mount);
        }

        public static void SetAsDifferentMounted(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount == null)
                return;
            mount.MountStatus = MountStatus.DifferentMount;
            DeNSo.Session.New.Set(mount);
        }

        public static void SetAsError(string path) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Path == path).FirstOrDefault();
            if (mount == null)
                return;
            mount.MountStatus = MountStatus.Error;
            DeNSo.Session.New.Set(mount);
        }

        public static void AddUnit(string guid, string unit) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Guid == guid).FirstOrDefault();
            if (mount == null)
                return;
            mount.AssociatedUnits.Add(unit);
            DeNSo.Session.New.Set(mount);
        }

        public static void AddUnit(string guid, IEnumerable<string> unit) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Guid == guid).FirstOrDefault();
            foreach (var u in unit) {
                mount?.AssociatedUnits.Add(u);
            }
            DeNSo.Session.New.Set(mount);
        }

        public static void RemoveUnit(string guid, string unit) {
            var mount = DeNSo.Session.New.Get<MountModel>(m => m.Guid == guid).FirstOrDefault();
            if (mount == null)
                return;
            mount.AssociatedUnits.Remove(unit);
            DeNSo.Session.New.Set(mount);
        }
    }
}
