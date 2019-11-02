using System.Collections.Generic;

namespace Antd.ProcFs {
    public static partial class ProcFs {
        public static class Net {
            public static IEnumerable<NetStatistics> Statistics() => NetStatistics.GetAll();

            public static class Services {
                public static IEnumerable<NetService> Tcp(NetAddressVersion addressVersion) => NetService.GetTcp(addressVersion);
                public static IEnumerable<NetService> Udp(NetAddressVersion addressVersion) => NetService.GetUdp(addressVersion);
                public static IEnumerable<NetService> Raw(NetAddressVersion addressVersion) => NetService.GetRaw(addressVersion);
                public static IEnumerable<NetService> Unix() => NetService.GetUnix();
            }

            public static IEnumerable<NetArpEntry> Arp() => NetArpEntry.Get();
        }
    }
}