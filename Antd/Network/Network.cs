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

using Antd.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Antd.Network.Management {

    public class NetworkInfo {

    }

    public class NetworkInterface {

        private static List<string> GetInterfaceList() {
            var dirs = Directory.GetDirectories("/sys/class/net");
            var list = new List<string>() { };
            foreach (var dir in dirs) {
                list.Add(Path.GetFileName(dir));
            }
            return list;
        }

        private static List<NetworkInterfaceModel> MapInterface() {
            var list = new List<NetworkInterfaceModel>() { };
            foreach (var name in GetInterfaceList()) {
                var str = Terminal.Execute("ip addr show " + name);
                var rows = str.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var model = new NetworkInterfaceModel()/* { Data = str }*/;
                model.Number = Convert.ToInt32(rows[0].Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries).ToArray()[0]);
                if (rows.Length == 4) {
                    model.Physical = rows[0];
                    model.Datalink = rows[1];
                    model.Network = rows[2] + " " + rows[3];
                }
                else if (rows.Length == 3) {
                    model.Physical = rows[0];
                    model.Datalink = rows[1];
                    model.Network = rows[2];
                }
                else if (rows.Length == 2) {
                    model.Physical = rows[0];
                    model.Datalink = rows[1];
                }
                list.Add(model);
            }
            return list.OrderBy(m => m.Number).ToList();
        }

        public static List<NetworkInterfaceModel> All { get { return MapInterface(); } }
    }
}
