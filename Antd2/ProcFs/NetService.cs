using System.Collections.Generic;
using System.Text;

namespace ProcFsCore
{
    public struct NetService
    {
        public NetServiceType Type { get; }
        public NetEndPoint LocalEndPoint { get; }
        public NetEndPoint RemoteEndPoint { get; }
        public string Path { get; }
        public NetServiceState State { get; }
        public int INode { get; }

        public NetService(NetServiceType type, in NetEndPoint localEndPoint, in NetEndPoint remoteEndPoint, string path, NetServiceState state, int iNode)
        {
            Type = type;
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
            Path = path;
            State = state;
            INode = iNode;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Type);
            if (Type == NetServiceType.Unix)
            {
                builder.Append(" ");
                builder.Append(Path);
            }
            else
            {
                if (!LocalEndPoint.IsEmpty)
                {
                    builder.Append(" ");
                    builder.Append(LocalEndPoint);
                }

                if (!RemoteEndPoint.IsEmpty)
                {
                    builder.Append(" ");
                    builder.Append(RemoteEndPoint);
                }
            }

            builder.Append($" {State}/{(int)State} {INode}");
            return builder.ToString();
        }

        private static readonly string[,] NetServiceFiles = 
        {
            { ProcFs.RootPath + "/net/tcp", ProcFs.RootPath + "/net/tcp6" },
            { ProcFs.RootPath + "/net/udp", ProcFs.RootPath + "/net/udp6" },
            { ProcFs.RootPath + "/net/raw", ProcFs.RootPath + "/net/raw6" },
            { ProcFs.RootPath + "/net/unix", null }
        };
        
        private static IEnumerable<NetService> Get(NetServiceType type, NetAddressVersion? addressVersion)
        {
            var serviceFile = NetServiceFiles[(int) type, (addressVersion ?? NetAddressVersion.IPv4) == NetAddressVersion.IPv4 ? 0 : 1];
            var statReader = new Utf8FileReader<X256>(serviceFile);
            try
            {
                statReader.SkipLine();
                while (!statReader.EndOfStream)
                {
                    var lineReader = new Utf8SpanReader(statReader.ReadLine());
                    lineReader.SkipWhiteSpaces();
                    lineReader.SkipWord();
                    if (type != NetServiceType.Unix)
                    {
                        var localEndPoint = NetEndPoint.Parse(ref lineReader);
                        var remoteEndPoint = NetEndPoint.Parse(ref lineReader);
                        var state = (NetServiceState)lineReader.ReadInt16('x');
                        
                        lineReader.SkipWord();
                        lineReader.SkipWord();
                        lineReader.SkipWord();
                        lineReader.SkipWord();
                        lineReader.SkipWord();

                        var iNode = lineReader.ReadInt32();
                        
                        lineReader.SkipLine();

                        yield return new NetService(type, localEndPoint, remoteEndPoint, null, state, iNode);
                    }
                    else
                    {
                        lineReader.SkipWord();
                        lineReader.SkipWord();
                        lineReader.SkipWord();
                        lineReader.SkipWord();
                        var state = (NetServiceState)lineReader.ReadInt16('x');
                        var iNode = lineReader.ReadInt32();
                        var path = lineReader.EndOfStream ? null : lineReader.ReadStringWord();

                        yield return new NetService(type, default, default, path, state, iNode);
                    }
                }
            }
            finally
            {
                statReader.Dispose();
            }
        }

        internal static IEnumerable<NetService> GetTcp(NetAddressVersion addressVersion) => Get(NetServiceType.Tcp, addressVersion);

        internal static IEnumerable<NetService> GetUdp(NetAddressVersion addressVersion) => Get(NetServiceType.Udp, addressVersion);

        internal static IEnumerable<NetService> GetRaw(NetAddressVersion addressVersion) => Get(NetServiceType.Raw, addressVersion);

        internal static IEnumerable<NetService> GetUnix() => Get(NetServiceType.Unix, null);
    }
}