
using antdlib.Common;
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
using antdlib.MountPoint;
using antdlib.ViewBinds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.Svcs.Glusterfs {
    public class GlusterfsConfig {

        private static string serviceGuid = "691672A7-30F4-4EC5-8035-2F3DCF115068";

        private static string dir = "/etc/glusterfs";

        private static string DIR = Mount.SetDIRSPath(dir);

        private static string mainFile = "glusterd.vol";

        public static void SetReady() {
            Terminal.Execute($"cp {dir} {DIR}");
            FileSystem.CopyDirectory(dir, DIR);
            Mount.Dir(dir);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(dir);
            return (mount == null) ? false : true;
        }

        public static bool IsActive { get { return CheckIsActive(); } }

        /// <summary>
        /// todo prendere comando giusto
        /// </summary>
        public static void ReloadConfig() {
            //Terminal.Execute($"smbcontrol all reload-config");
        }

        private static List<string> GetServiceStructure() {
            var list = new List<string>() { };
            var files = Directory.EnumerateFiles(DIR, "*.vol", SearchOption.AllDirectories).ToArray();
            for (int i = 0; i < files.Length; i++) {
                list.Add(files[i].Replace("\\", "/"));
            }
            return list;
        }

        public static List<string> Structure { get { return GetServiceStructure(); } }

        public class MapRules {
            public static char CharComment { get { return '#'; } }

            public static char CharKevValueSeparator { get { return ' '; } }

            public static char CharEndOfLine { get { return '\n'; } }

            public class Statement {
                public static string StartVolume { get { return "volume management"; } }

                public static string EndVolume { get { return "end-volume"; } }

                public static string StartOption { get { return "option"; } }
            }
        }

        public class LineModel {
            public string FilePath { get; set; }

            public string Key { get; set; }

            public string Value { get; set; }

            public ServiceDataType Type { get; set; }

            public KeyValuePair<string, string> BooleanVerbs { get; set; }
        }

        public class VolumeModel {
            public string FilePath { get; set; } = $"{DIR}/{mainFile}";

            public string Name { get; set; }

            public string StringDefinition { get; set; } = "";

            public List<LineModel> Data { get; set; } = new List<LineModel>() { };
        }


        public class GlusterfsModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public string Timestamp { get; set; }

            public List<VolumeModel> Volumes { get; set; } = new List<VolumeModel>() { };
        }

        public class MapFile {

            private static string CleanLine(string line) {
                var removeTab = line.Replace("\t", " ");
                var clean = removeTab;
                if (removeTab.Contains(MapRules.CharComment) && !line.StartsWith(MapRules.CharComment.ToString())) {
                    var splitAtComment = removeTab.Split(MapRules.CharComment);
                    clean = splitAtComment[0].Trim();
                }
                return clean;
            }

            private static VolumeModel ReadFile(string path) {
                var input = FileSystem.ReadFile(path);
                var data = new List<LineModel>() { };

                var volume = new VolumeModel() {
                    FilePath = path,
                    Name = path, //prendi solo nome file?
                    Data = data
                };
                
                return volume;
            }

            public static void Render() {
                var path = $"{DIR}/{mainFile}";
                var volumes = new List<VolumeModel>() { };
                foreach (var file in Structure) {
                    volumes.Add(ReadFile(file));
                }
                var glusterfs = new GlusterfsModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Volumes = volumes
                };
                DeNSo.Session.New.Set(glusterfs);
            }

            public static GlusterfsModel Get() {
                var glusterfs = DeNSo.Session.New.Get<GlusterfsModel>(s => s.Guid == serviceGuid).FirstOrDefault();
                return glusterfs;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceGlusterfs parameter) {
                ServiceDataType type = Helper.ServiceData.SupposeDataType(parameter.DataValue);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(parameter.DataValue);
                var data = new LineModel() {
                    FilePath = parameter.DataFilePath,
                    Key = parameter.DataKey,
                    Value = parameter.DataValue,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                return data;
            }

            public static void SaveGlobalConfig(List<ServiceGlusterfs> newParameters) {
                var data = new List<LineModel>() { };
                foreach (var parameter in newParameters) {
                    data.Add(ConvertData(parameter));
                }
                var glusterfs = new GlusterfsModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                };
                DeNSo.Session.New.Set(glusterfs);
            }

            public static void DumpGlobalConfig() {
 
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void AppendLine(string path, string text) {
                File.AppendAllText(path, $"{text}{Environment.NewLine}");
            }
        }
    }
}
