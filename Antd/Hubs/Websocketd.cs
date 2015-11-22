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
using antdlib.Systemd;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using antdlib;
using antdlib.Log;
using antdlib.Terminal;

namespace Antd.Hubs {
    public class Websocketd {
        private static string _fileName = "websocketd";
        private static readonly string _filePath = $"{Parameter.AntdCfg}/websocketd";

        public static string GetFirstPort(long port = 31000) {
            var c = Terminal.Execute($"netstat -anp | grep :{port}");
            return (c.Length > 0) ? GetFirstPort(port + 1) : ((port > 49999) ? "30999" : port.ToString());
        }

        public static void SetUnit(string port, string command) {
            var unitName = $"ws{port}.service";
            var unitPath = $"{Parameter.WebsocketUnits}/{unitName}";
            if (!File.Exists(unitPath)) {
                using (var sw = File.CreateText(unitPath)) {
                    sw.WriteLine("[Unit]");
                    sw.WriteLine($"Description=External Volume Unit, Antd Websocketd Service @{port}");
                    sw.WriteLine("");
                    sw.WriteLine("[Service]");
                    sw.WriteLine($"ExecStart={_filePath} --port={port} {command}");
                    sw.WriteLine("");
                }
            }
            Systemctl.DaemonReload();
            Systemctl.Restart(unitName);
        }

        public static void SetCmd(string port, string command) {
            var cmd = $"{_filePath} --port={port} {command}";
            Terminal.Execute(cmd);
        }

        /// <summary></summary>
        ///     es: /usr/bin/vmstat -n 1
        ///     ->: /cfg/antd/websocketd/websocketd --port=30333 /usr/bin/vmstat -n 1
        /// <returns></returns>
        public static async Task SetWebsocket(string p = "") {
            var port = (p.Length > 0) ? p : GetFirstPort();
            var ws = new ClientWebSocket();
            var uri = new System.Uri($"ws://127.0.0.1:{port}/");
            await ws.ConnectAsync(uri, CancellationToken.None);
            var buffer = new byte[1024];
            while (true) {
                var segment = new ArraySegment<byte>(buffer);
                var result = await ws.ReceiveAsync(segment, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) {
                    await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "I don't do binary", CancellationToken.None);
                    ConsoleLogger.Warn("Connection closed: I don't do binary");
                }
                int count = result.Count;
                while (!result.EndOfMessage) {
                    if (count >= buffer.Length) {
                        await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                        ConsoleLogger.Warn("Connection closed: That's too long");
                    }
                    segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                    result = await ws.ReceiveAsync(segment, CancellationToken.None);
                    count += result.Count;
                }
                var message = Encoding.UTF8.GetString(buffer, 0, count);
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.All.getWebsocketd(message);
            }
        }

        public static async Task LaunchCommandToJournalctl(string command) {
            var port = GetFirstPort();
            SetUnit(port, command);
            var ws = new ClientWebSocket();
            var uri = new System.Uri($"ws://127.0.0.1:{port}/");
            await ws.ConnectAsync(uri, CancellationToken.None);
            var buffer = new byte[1024];
            while (true) {
                var segment = new ArraySegment<byte>(buffer);
                var result = await ws.ReceiveAsync(segment, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) {
                    await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "I don't do binary", CancellationToken.None);
                    ConsoleLogger.Warn("Connection closed: I don't do binary");
                }
                var count = result.Count;
                while (!result.EndOfMessage) {
                    if (count >= buffer.Length) {
                        await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                        ConsoleLogger.Warn("Connection closed: That's too long");
                    }
                    segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                    result = await ws.ReceiveAsync(segment, CancellationToken.None);
                    count += result.Count;
                }
                var message = Encoding.UTF8.GetString(buffer, 0, count);
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.All.getJournalctl(message);
            }
        }
    }
}
