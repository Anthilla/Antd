using System.Diagnostics;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Antd.Systemd {
    /// <summary>Represents a Unix Domain Socket endpoint as a path.</summary>
    internal sealed class UnixDomainSocketEndPoint : EndPoint {
        private const AddressFamily EndPointAddressFamily = AddressFamily.Unix;

        private static readonly Encoding s_pathEncoding = Encoding.UTF8;
        private const int s_nativePathOffset = 2;

        private readonly string _path;
        private readonly byte[] _encodedPath;

        public UnixDomainSocketEndPoint(string path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            _path = path;
            _encodedPath = s_pathEncoding.GetBytes(_path);

            if (path.Length == 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(path), path,
                    string.Format("The path '{0}' is of an invalid length for use with domain sockets on this platform. The length must be at least 1 characters.", path));
            }
        }

        internal UnixDomainSocketEndPoint(SocketAddress socketAddress) {
            throw new NotImplementedException();
        }

        public override SocketAddress Serialize() {
            var result = new SocketAddress(AddressFamily.Unix, _encodedPath.Length + s_nativePathOffset);

            for (int index = 0; index < _encodedPath.Length; index++) {
                result[s_nativePathOffset + index] = _encodedPath[index];
            }

            return result;
        }

        public override EndPoint Create(SocketAddress socketAddress) => new UnixDomainSocketEndPoint(socketAddress);

        public override AddressFamily AddressFamily => EndPointAddressFamily;

        public override string ToString() => _path;
    }
}