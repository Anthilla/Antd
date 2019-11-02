using System;
using System.Buffers.Text;
using System.Net;
using System.Runtime.InteropServices;

namespace Antd.ProcFs {
    public unsafe struct NetAddress {
        private const int MaxAddressLength = 16;
#pragma warning disable 649
        private fixed byte _data[MaxAddressLength];
#pragma warning restore 649
        private readonly int _length;

        private Span<byte> Data => MemoryMarshal.CreateSpan(ref _data[0], _length);

        public NetAddressVersion Version => _length == 4 ? NetAddressVersion.IPv4 : NetAddressVersion.IPv6;

        public bool IsEmpty {
            get {
                foreach (var part in Data)
                    if (part != 0)
                        return false;
                return true;
            }
        }

        public NetAddress(ReadOnlySpan<byte> address) {
            _length = address.Length;
            address.CopyTo(Data);
        }

        public static NetAddress Parse(ReadOnlySpan<byte> addressString, NetAddressFormat format) {
            switch (format) {
                case NetAddressFormat.Hex: {
                    Span<uint> address = stackalloc uint[MaxAddressLength >> 2];
                    var addressLength = addressString.Length >> 3;
                    for (var i = 0; i < addressLength; ++i) {
                        var hexPart = addressString.Slice(i << 3, 8);
                        Utf8Parser.TryParse(hexPart, out uint addressPart, out _, 'x');
                        address[i] = addressPart;
                    }

                    return new NetAddress(MemoryMarshal.Cast<uint, byte>(address.Slice(0, addressLength)));
                }
                case NetAddressFormat.Human: {
                    Span<char> addressStr = stackalloc char[64];
                    Utf8Extensions.Encoding.GetChars(addressString, addressStr);
                    var frameworkAddress = IPAddress.Parse(addressStr.Slice(0, addressString.Length));
                    Span<byte> addressBytes = stackalloc byte[MaxAddressLength];
                    frameworkAddress.TryWriteBytes(addressBytes, out var addressLength);
                    return new NetAddress(addressBytes.Slice(0, addressLength));
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public override string ToString() => ((IPAddress)this).ToString();

        public static implicit operator IPAddress(in NetAddress address) => new IPAddress(address.Data);
    }
}