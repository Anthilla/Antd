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

using System.Net.NetworkInformation;
using System.Threading;
using NetworkInterfaceInformation = System.Net.NetworkInformation.NetworkInterface;

namespace Antd.Sdp {
    public class NetworkInterface {
        public string Name { get { return Information.Name; } }
        public string Description { get { return Information.Description; } }
        public string Id { get { return Information.Id; } }
        public int IPv4Index { get; private set; }
        public int IPv6Index { get; private set; }

        public override bool Equals(object obj) {
            NetworkInterface networkInterface = obj as NetworkInterface;
            if (networkInterface == null) {
                return false;
            }
            return Index.Equals(networkInterface.Index);
        }

        public override int GetHashCode() {
            return Index.GetHashCode();
        }

        internal NetworkInterface(NetworkInterfaceInformation info) {
            Information = info;
            IPv4Index = -1;
            IPv6Index = -1;
            if (info.Supports(NetworkInterfaceComponent.IPv4)) {
                IPv4Index = info.GetIPProperties().GetIPv4Properties().Index;
            }
            if (info.Supports(NetworkInterfaceComponent.IPv6)) {
                IPv6Index = info.GetIPProperties().GetIPv6Properties().Index;
            }
            Index = Interlocked.Increment(ref NextIndex);
        }

        internal int Index { get; private set; }
        internal NetworkInterfaceInformation Information { get; private set; }

        static private int NextIndex = 1;
    }
}
