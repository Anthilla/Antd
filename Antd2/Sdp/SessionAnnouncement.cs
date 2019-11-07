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

namespace Antd.Sdp {
    public class SessionAnnouncement {
        public SessionAnnouncement(SessionDescription sessionDescription, NetworkInterface inter) {
            if (sessionDescription.Announcement != null) {
                throw new ArgumentException("sessionDescription");
            }
            SessionDescription = sessionDescription;
            SessionDescription.Announcement = this;
            NetworkInterface = inter;
        }

        public SessionDescription SessionDescription { get; private set; }
        public NetworkInterface NetworkInterface { get; private set; }
        private Session _session;
        public Session Session {
            get {
                if (_session == null) {
                    _session = new SdpSession(this);
                }
                return _session;
            }
        }

        public override string ToString() {
            return SessionDescription.Name + " (" + NetworkInterface.Name + ")";
        }
    }
}
