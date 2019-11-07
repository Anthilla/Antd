//Copyright (C) 2014  Tom Deseyn

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Antd.Sdp {
    class NetworkInterfaceHandler {
        public SapClient SapClient { get; private set; }
        public NetworkInterface NetworkInterface { get; private set; }
        public static readonly IPEndPoint IPv4EndPoint = new IPEndPoint(IPAddress.Parse("224.2.127.254"), 9875);

        public NetworkInterfaceHandler(SapClient sapClient, NetworkInterface networkInterface) {
            SapClient = sapClient;
            NetworkInterface = networkInterface;
            _index = NetworkInterface.Information.GetIPProperties().GetIPv4Properties().Index;
        }
        public bool IsEnabled { get; private set; }
        public void Enable() {
            if (IsEnabled) {
                return;
            }
            IsEnabled = true;

            lock (this) {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.HostToNetworkOrder(_index));
                _socket.Bind(new IPEndPoint(IPAddress.Any, IPv4EndPoint.Port));
                IPAddress ip = IPv4EndPoint.Address;
                _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, _index));
                _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);

                StartReceive();
            }
        }
        public void Disable() {
            if (!IsEnabled) {
                return;
            }

            lock (this) {
                IsEnabled = false;

                _socket.Dispose();
                _socket = null;

                SapClient.OnInterfaceDisable(this);
            }
        }

        private void StartReceive() {
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private void OnReceive(IAsyncResult ar) {
            lock (this) {
                int length;
                try {
                    if (_socket == null) {
                        return;
                    }
                    length = _socket.EndReceive(ar);
                }
                catch (Exception) {
                    return;
                }

                try {
                    var stream = new MemoryStream(_buffer, 0, length, false, true);
                    Announcement announcement = ReadAnnouncement(stream);

                    if (announcement.Type == Announcement.MessageType.Announcement) {
                        SapClient.OnSessionAnnounce(this, announcement);
                    }
                    else {
                        announcement.Decompress();
                        string origin = Encoding.UTF8.GetString(announcement.Payload.Array, announcement.Payload.Offset + 2, announcement.Payload.Count - 4);
                        SapClient.OnSessionDelete(this, Origin.Parse(origin));
                    }
                }
                catch (Exception e) {
                    SapClient.OnException(e);
                }

                StartReceive();
            }
        }

        private Announcement ReadAnnouncement(MemoryStream stream) {
            Announcement announcement = new Announcement();

            NetworkStreamReader sr = new NetworkStreamReader(stream);

            byte b = sr.ReadByte();

            announcement.Version = (byte)(b & 0xE0);
            bool IPv6 = (b & (1 << 4)) != 0;
            announcement.Type = (b & (1 << 2)) != 0 ? Announcement.MessageType.Deletion : Announcement.MessageType.Announcement;
            announcement.IsEncrypted = (b & (1 << 1)) != 0;
            announcement.IsCompressed = (b & (1 << 0)) != 0;

            byte authenticationLength = sr.ReadByte();
            announcement.AuthenticationData = new ArraySegment<byte>(stream.GetBuffer(), (int)stream.Position, (int)authenticationLength);

            announcement.Hash = sr.ReadUInt16();

            if (IPv6) {
                byte[] addressBytes = sr.ReadBytes(16);
                announcement.Source = new IPAddress(addressBytes);
            }
            else {
                byte[] addressBytes = sr.ReadBytes(4);
                announcement.Source = new IPAddress(addressBytes);
            }

            sr.Skip(authenticationLength);

            long position = sr.BaseStream.Position;

            StringBuilder sb = new StringBuilder();
            b = sr.ReadByte();
            while (b != 0) {
                sb.Append((char)b);
                if ((sb.Length == 2) && (sb.ToString() == "v=")) {
                    // SAPv1
                    sb.Clear();
                    sb.Append("application/sdp");
                    sr.BaseStream.Seek(position, SeekOrigin.Begin);
                    break;
                }
                b = sr.ReadByte();
            }

            announcement.PayloadType = sb.ToString();

            announcement.Payload = new ArraySegment<byte>(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));

            return announcement;
        }

        private Socket _socket;
        private readonly int _index;
        private readonly byte[] _buffer = new byte[9000];
    }
}
