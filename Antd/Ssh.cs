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
using System.Xml.Schema;
using antdlib;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.ViewBinds;
using Antd.Database;
using Antd.MountPoint;
using Nancy.Responses;

namespace Antd {
    public class Ssh {

        private static IEnumerable<string> GetUsers() {
            var u = Directory.EnumerateDirectories(Parameter.Home);
            return u.Select(Path.GetDirectoryName);
        }

        public static IEnumerable<AuthorizedKeys> GetAuthorizedKeys() {
            var ak = new List<AuthorizedKeys>();
            foreach (var u in GetUsers()) {
                var auk = $"{Parameter.Home}/{u}/.ssh/authorized_keys";
                if (File.Exists(auk)) {
                    var a = new AuthorizedKeys {
                        User = u
                    };
                    var klist = new List<SshKey>();
                    foreach (var line in File.ReadAllLines(auk)) {
                        var split = line.Split(' ');
                        var k = new SshKey {
                            Type = split[0],
                            Value = split[1],
                            PublicUser = split[2]
                        };
                        klist.Add(k);
                    }
                    a.KeyList = klist;
                    ak.Add(a);
                }
            }
            return ak;
        }

        public class AuthorizedKeys {
            public string User { get; set; }
            public IEnumerable<SshKey> KeyList { get; set; } = new List<SshKey>();
        }

        public class SshKey {
            public string Type { get; set; }
            public string Value { get; set; }
            public string PublicUser { get; set; }
        }

        public class Keys {
            public static void SendKey(string host, string keyName, string user = "") {
                var at = (user.Length > 0 ? user + "@" : "") + $"{host}";
                Terminal.Execute($"scp {keyName} {at} /root/.ssh/authorized_keys");
            }

            private static string GenerateForUser(string userName) {
                var privateKeyPath = $"/home/{userName}/.ssh/{userName}-key";
                var publicKeyPath = $"/home/{userName}/.ssh/{userName}-key.pub";
                Terminal.Execute($"sudo -H -u {userName} bash -c 'echo y\n | ssh-keygen -b 2048 -t rsa -f {privateKeyPath} -q -N \"\"'");
                return publicKeyPath;
            }

            private static void ExportKeysToRemote(string user, string remoteMachine) {
                var userPubKeyPath = GenerateForUser(user);
                var keyContent = File.ReadAllText(userPubKeyPath).Replace(Environment.NewLine, " ");
                //todo comando per aggiungere la chiave al file di là
                Terminal.Execute($"scp {keyContent} {remoteMachine}/{Parameter.AuthKeys}");
            }

            public static void PropagateKeys(IEnumerable<string> users, IEnumerable<string> remoteMachines) {
                var enumerable = remoteMachines as string[] ?? remoteMachines.ToArray();
                foreach (var user in users) {
                    foreach (var remoteMachine in enumerable) {
                        ExportKeysToRemote(user, remoteMachine);
                    }
                }
            }
        }
    }
}
