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
using System.IO.Compression;
using System.Net;

namespace Antd.Sdp {
    class Announcement {
        public enum MessageType {
            Announcement,
            Deletion
        }
        public byte Version { get; set; }
        public MessageType Type { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsCompressed { get; set; }
        public ArraySegment<byte> AuthenticationData { get; set; }
        public ushort Hash { get; set; }
        public IPAddress Source { get; set; }
        public string PayloadType { get; set; }
        public ArraySegment<byte> Payload { get; set; }

        public void Decompress() {
            if (!IsCompressed) {
                return;
            }

            MemoryStream stream = new MemoryStream(Payload.Array, Payload.Offset + 2, Payload.Count - 2);
            DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
            stream = new MemoryStream();
            deflateStream.CopyTo(stream);
            IsCompressed = false;
            Payload = new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        }
    }
}
