using System.Net;

namespace antdlib.ProcFs {
    public struct NetEndPoint {
        public NetAddress Address { get; }
        public int Port { get; }
        public bool IsEmpty => Address.IsEmpty && Port == 0;

        public NetEndPoint(in NetAddress address, int port) {
            Address = address;
            Port = port;
        }

        public static NetEndPoint Parse(ref Utf8SpanReader statReader) => new NetEndPoint(NetAddress.Parse(statReader.ReadFragment(':'), NetAddressFormat.Hex), statReader.ReadInt32('x'));

        public override string ToString() => ((IPEndPoint)this)?.ToString();

        public static implicit operator IPEndPoint(in NetEndPoint endPoint) => endPoint.IsEmpty ? null : new IPEndPoint(endPoint.Address, endPoint.Port);
    }
}