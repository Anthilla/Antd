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

namespace Antd.Sdp {
    class NetworkStreamReader {
        public Stream BaseStream { get; private set; }

        public NetworkStreamReader(Stream stream) {
            BaseStream = stream;
        }

        public byte ReadByte() {
            int i = BaseStream.ReadByte();
            if (i >= 0) {
                return (byte)i;
            }
            throw new Exception();
        }

        public UInt16 ReadUInt16() {
            byte b0 = ReadByte();
            byte b1 = ReadByte();
            return (UInt16)((b0 << 8) + b1);
        }

        public UInt32 ReadUInt32() {
            byte b0 = ReadByte();
            byte b1 = ReadByte();
            byte b2 = ReadByte();
            byte b3 = ReadByte();
            return (UInt32)((b0 << 24) + (b1 << 16) + (b2 << 8) + b3);
        }

        public void Skip(int length) {
            BaseStream.Seek(length, SeekOrigin.Current);
        }

        public byte[] ReadBytes(int length) {
            var buffer = new byte[length];
            int offset = 0;

            while (length > 0) {
                int bytesRead = BaseStream.Read(buffer, offset, length);
                if (bytesRead <= 0) {
                    throw new Exception();
                }
                length -= bytesRead;
                offset += bytesRead;
            }

            return buffer;
        }
    }
}
