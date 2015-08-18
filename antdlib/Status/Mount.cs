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

using antdlib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.Status {

    public class Mount {

        //private static List<MountModel> GetAllMounts() {
        //    string text = FileSystem.ReadFile("/proc", "mounts");
        //    var output = JsonConvert.SerializeObject(text);
        //    List<MountModel> mounts = MapMountJson(output);
        //    return mounts;
        //}

        //public static List<MountModel> Running { get { return GetAllMounts(); } }

        //private static List<MountModel> ReadMountCustomFile() {
        //    string text = FileSystem.ReadFile(Folder.Config, "antd.mounts");
        //    var output = JsonConvert.SerializeObject(text);
        //    List<MountModel> mounts = MapMountJson(output);
        //    return mounts;
        //}

        //public static List<MountModel> Antd { get { return ReadMountCustomFile(); } }

        //private static List<MountModel> MapMountJson(string _mountJson) {
        //    string mountJson2 = _mountJson;
        //    mountJson2 = Regex.Replace(_mountJson, @"\s{2,}", " ").Replace("\"", "").Replace("\\n", "\n");
        //    string mountJson = mountJson2;
        //    mountJson = Regex.Replace(mountJson2, @"\\t", " ");
        //    string[] rowDivider = new String[] { "\n" };
        //    string[] mountJsonRow = new string[] { };
        //    mountJsonRow = mountJson.Split(rowDivider, StringSplitOptions.None).ToArray();
        //    List<MountModel> mounts = new List<MountModel>() { };
        //    foreach (string rowJson in mountJsonRow) {
        //        if (rowJson != null && rowJson != "") {
        //            var fCh = rowJson.ToArray()[0];
        //            if (fCh != '#') {
        //                string[] mountJsonCell = new string[] { };
        //                string[] cellDivider = new String[] { " " };
        //                mountJsonCell = rowJson.Split(cellDivider, StringSplitOptions.None).ToArray();
        //                MountModel mount = MapMount(mountJsonCell);
        //                mounts.Add(mount);
        //            }
        //        }
        //    }
        //    return mounts;
        //}

        //private static MountModel MapMount(string[] _mountJsonCell) {
        //    string[] mountJsonCell = _mountJsonCell;
        //    MountModel mount = new MountModel();
        //    if (mountJsonCell.Length > 1) {
        //        mount.device = mountJsonCell[0];
        //        mount.mountpoint = mountJsonCell[1];
        //        mount.fstype = mountJsonCell[2];
        //        mount.rorw = mountJsonCell[3];
        //        mount.dv1 = mountJsonCell[4];
        //        mount.dv2 = mountJsonCell[5];
        //    }
        //    return mount;
        //}

        //public static void WriteConfig() {
        //    var parameters = Running;
        //    string path = Path.Combine(Folder.Config, "antd.mounts");
        //    if (File.Exists(path)) {
        //        File.Delete(path);
        //    }
        //    using (StreamWriter sw = File.CreateText(path)) {
        //        sw.WriteLine("# " + path);
        //        sw.WriteLine("# Custom Configuration for Antd");
        //        foreach (MountModel p in parameters) {
        //            sw.WriteLine(p.device + " " +
        //                        p.mountpoint + " " +
        //                        p.fstype + " " +
        //                        p.rorw + " " +
        //                        p.dv1 + " " +
        //                        p.dv2);
        //        }
        //        sw.WriteLine("");
        //    }
        //}
    }
}