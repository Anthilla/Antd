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
using Newtonsoft.Json;

namespace antdlib.Network.Management {
    public class NetworkConfiguration {

        private static string fileName = "antd.boot.network.type";

        private static string filePath = $"{Folder.Root}/{fileName}";

        public static void WriteFileMethod(NetworkBootType netBootType) {
            if (!File.Exists(filePath)) {
                FileSystem.WriteFile(filePath, JsonConvert.SerializeObject(netBootType));
            }
            else {
                ConsoleLogger.Warn($"{filePath} already exists, overwriting existing file!");
                File.Delete(filePath);
                FileSystem.WriteFile(filePath, JsonConvert.SerializeObject(netBootType));
            }
        }

        private static NetworkBootType ReadFileMethod() {
            NetworkBootType type = NetworkBootType.Default;
            if (File.Exists(filePath)) {
                var t = FileSystem.ReadFile(filePath);
                if (t.Length > 0) {
                    try {
                        type = JsonConvert.DeserializeObject<NetworkBootType>(t);
                    }
                    catch (Exception ex) {
                        ConsoleLogger.Warn($"Exception while Deserializing an object: {ex.Message}");
                    }
                }
            }
            return type;
        }

        public static NetworkBootType BootType { get { return ReadFileMethod(); } }

        public static void LoadExistingConfiguration() {
            if (File.Exists(NetworkFile.Name) && File.Exists(FirewallFile.Name)) {
                Terminal.Execute($"chmod 777 {NetworkFile.Name}");
                Terminal.Execute($"./{NetworkFile.Name}");
                Terminal.Execute($"nft -f {FirewallFile.Name}");
            }
        }

        public class NetworkFile {
            public static string Name { get { return $"{Folder.Root}/antd.boot.network"; } }

            public static string Content {
                get {
                    return (File.Exists(Name)) ? FileSystem.ReadFile(Name) : "null";
                }
            }

            public static void Edit(string newText) {
                if (File.Exists(Name)) {
                    File.Delete(Name);
                }
                FileSystem.WriteFile(Name, newText);
            }
        }

        public class FirewallFile {
            public static string Name { get { return $"{Folder.Root}/antd.boot.firewall"; } }

            public static string Content {
                get {
                    return (File.Exists(Name)) ? FileSystem.ReadFile(Name) : "null";
                }
            }

            public static void Edit(string newText) {
                if (File.Exists(Name)) {
                    File.Delete(Name);
                }
                FileSystem.WriteFile(Name, newText);
            }
        }
    }
}
