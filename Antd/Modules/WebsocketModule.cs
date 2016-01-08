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
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using antdlib;
using antdlib.Network;
using Nancy;
using WebSocket = antdlib.Websocket.Client.WebSocket;

namespace Antd.Modules {
    public class WebsocketModule : CoreModule {
        public WebsocketModule() {
            Get["/ws"] = x => {
                try {
                    WebSocket.Start(PortManagement.GetFirstAvailable(45000, 45999));
                    return Response.AsText("done");
                }
                catch (Exception ex) {
                    return Response.AsText(ex.Message);
                }
            };

            Get["/ws/{port}"] = x => {
                try {
                    WebSocket.Start((int)x.port);
                    return Response.AsText("done");
                }
                catch (Exception ex) {
                    return Response.AsText(ex.Message);
                }
            };

            Get["/ws/port"] = x => Response.AsJson(ApplicationSetting.WebsocketPort());

            Post["/ws/post", true] = async (x, ct) => {
                var port = PortManagement.GetFirstAvailable(45000, 45999);
                WebSocket.Start(port);
                Thread.Sleep(4);
                var webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri($"ws://localhost:{port}/cmd"), CancellationToken.None);
                string command = Request.Form.Command;
                await webSocket.SendAsync(new ArraySegment<byte>(new UTF8Encoding().GetBytes(command)), WebSocketMessageType.Text, true, CancellationToken.None);
                return Response.AsJson(port);
            };
        }

        private static async Task<string> Send(ClientWebSocket webSocket, string command) {
            var buffer = new UTF8Encoding().GetBytes(command);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            while (webSocket.State == WebSocketState.Open) {
                await Task.Delay(TimeSpan.FromMilliseconds(30000));
            }
            return "ok";
        }
    }
}