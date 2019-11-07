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

namespace Antd.Sdp {
    class SdpSession : Session {
        public SdpSession(Origin o, NetworkInterface inter) {
            SessionDescription = new SessionDescription();
            SessionDescription.Origin = o;
            NetworkInterface = inter;
        }
        public SdpSession(SessionDescription sd, NetworkInterface inter) {
            SessionDescription = sd;
            NetworkInterface = inter;
        }
        public SdpSession(SessionAnnouncement announcement) {
            SessionDescription = announcement.SessionDescription;
            NetworkInterface = announcement.NetworkInterface;
        }

        public SessionDescription SessionDescription { get; private set; }
        public NetworkInterface NetworkInterface { get; private set; }

        public override int GetHashCode() {
            int h1 = SessionDescription.Origin.GetSessionHashCode();
            int h2 = GetInterfaceIndex(NetworkInterface).GetHashCode();
            int hash = 17;
            hash = hash * 31 + h1;
            hash = hash * 31 + h2;
            return hash;
        }

        public override bool Equals(object obj) {
            SdpSession session = obj as SdpSession;
            if (session == null) {
                return false;
            }
            return GetInterfaceIndex(NetworkInterface).Equals(GetInterfaceIndex(NetworkInterface)) && SessionDescription.Origin.IsSameSession(session.SessionDescription.Origin);
        }

        public override int CompareTo(Session o) {
            SdpSession other = o as SdpSession;
            if (GetInterfaceIndex(NetworkInterface) == GetInterfaceIndex(other.NetworkInterface)) {
                if (SessionDescription.Origin.SessionID == other.SessionDescription.Origin.SessionID) {
                    return string.Format("{0} {1} {2} {3}", SessionDescription.Origin.UserName, SessionDescription.Origin.NetworkType, SessionDescription.Origin.AddressType, SessionDescription.Origin.Address)
                        .CompareTo(string.Format("{0} {1} {2} {3}", other.SessionDescription.Origin.UserName, other.SessionDescription.Origin.NetworkType, other.SessionDescription.Origin.AddressType, other.SessionDescription.Origin.Address));
                }
                else {
                    return SessionDescription.Origin.SessionID.CompareTo(other.SessionDescription.Origin.SessionID);
                }
            }
            else {
                return GetInterfaceIndex(NetworkInterface).CompareTo(GetInterfaceIndex(other.NetworkInterface));
            }
        }

        private static int GetInterfaceIndex(NetworkInterface interf) {
            if (interf == null) {
                return 0;
            }
            else {
                return interf.Index;
            }
        }
    }
}
