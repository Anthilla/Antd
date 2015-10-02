
using Antd.Hubs;
using antdlib;
using antdlib.Collectd;
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
using antdlib.Log;
using Nancy;
//using Nancy.Security;
using System;
using System.Dynamic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Antd {

    public class LogModule : NancyModule {
        public LogModule()
            : base("/log") {
            //this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.LOGS = Logger.GetAllMethods();
                return View["_page-log", vmod];
            };

            Get["/collectd"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-collectd", vmod];
            };

            Get["/websocket2"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-websocket", vmod];
            };

            Get["/websocket", true] = async (x, ct) => {
                dynamic vmod = new ExpandoObject();
                ConsoleLogger.Info("init websock");
                //Terminal.Execute("/root/test/web-vmstats-master/websocketd --port=30333 /usr/bin/journalctl");
                ConsoleLogger.Point("ws server active?");
                ClientWebSocket ws = new ClientWebSocket();
                var uri = new System.Uri("ws://127.0.0.1:30333/");
                await ws.ConnectAsync(uri, CancellationToken.None);
                ConsoleLogger.Point("connected?");
                var buffer = new byte[1024];
                while (true) {
                    var segment = new ArraySegment<byte>(buffer);
                    var result = await ws.ReceiveAsync(segment, CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) {
                        await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "I don't do binary", CancellationToken.None);
                        ConsoleLogger.Warn("I don't do binary");
                        return View["_page-log-websocket", vmod];
                    }
                    int count = result.Count;
                    while (!result.EndOfMessage) {
                        if (count >= buffer.Length) {
                            await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                            ConsoleLogger.Warn("That's too long");
                            return View["_page-log-websocket", vmod];
                        }
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await ws.ReceiveAsync(segment, CancellationToken.None);
                        count += result.Count;
                    }
                    var message = Encoding.UTF8.GetString(buffer, 0, count);
                    ConsoleLogger.Warn(message);
                    return View["_page-log-websocket", vmod];
                }
            };
        }
    }
}