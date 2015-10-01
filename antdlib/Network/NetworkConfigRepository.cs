
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
namespace antdlib.Network {
    public class NetworkConfigRepository {
        public class Model {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public bool IsEnabled { get; set; } = true;

            public string CommandLine { get; set; }
        }

        public static List<Model> GetAll() {
            var get = DeNSo.Session.New.Get<Model>(m => m != null).ToList();
            return (get == null) ? new List<Model>() { } : get;
        }

        public static Model GetByGuid(string guid) {
            return GetAll().Where(m => m.Guid == guid).FirstOrDefault();
        }

        public static List<Model> GetByString(string q) {
            return GetAll().Where(m => m.CommandLine.Contains(q)).ToList();
        }

        public static void Create(string command) {
            var model = new Model() {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                CommandLine = command
            };
            DeNSo.Session.New.Set(model);
        }

        public static void Enable(string guid) {
            var model = GetByGuid(guid);
            model.IsEnabled = true;
            DeNSo.Session.New.Set(model);
        }

        public static void Disable(string guid) {
            var model = GetByGuid(guid);
            model.IsEnabled = false;
            DeNSo.Session.New.Set(model);
        }

        public static void Delete(string guid) {
            var model = GetByGuid(guid);
            DeNSo.Session.New.Delete(model);
        }

        public static void ExportToFile() {
            var commands = GetAll().Where(m => m.IsEnabled == true).ToArray();
            if (commands.Length > 0) {
                if (File.Exists(AntdFile.NetworkConfig)) {
                    File.Delete(AntdFile.NetworkConfig);
                }
                FileSystem.WriteFile(AntdFile.NetworkConfig, String.Join(Environment.NewLine, commands.Select(m => m.CommandLine)));
            }
        }
    }
}