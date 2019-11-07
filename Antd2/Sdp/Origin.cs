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
using System.Net;
using System.Net.Sockets;

namespace Antd.Sdp {
    public class Origin {
        public Origin() :
            this(0, 0, IPAddress.Loopback) { }
        public Origin(ulong sessionID, ulong sessionVersion, IPAddress address) {
            if (address == null) {
                throw new ArgumentException("address");
            }
            UserName = "-";
            SessionID = sessionID;
            SessionVersion = sessionVersion;
            IPAddress = address;
        }
        public Origin(string username, ulong sessionID, ulong sessionVersion, string networkType, string addressType, string address) {
            if (string.IsNullOrEmpty(username)) {
                throw new ArgumentException("username");
            }
            if (string.IsNullOrEmpty(networkType)) {
                throw new ArgumentException("networkType");
            }
            if (string.IsNullOrEmpty(addressType)) {
                throw new ArgumentException("addressType");
            }
            if (string.IsNullOrEmpty(address)) {
                throw new ArgumentException("unicastAddress");
            }
            UserName = username;
            SessionID = sessionID;
            SessionVersion = sessionVersion;
            SetAddress(networkType, addressType, address);
        }
        public SessionDescription SessionDescription { get; internal set; }
        public bool IsReadOnly {
            get {
                if (SessionDescription != null) {
                    return SessionDescription.IsReadOnly;
                }
                else {
                    return false;
                }
            }
        }
        private string _userName;
        public string UserName {
            get {
                return _userName;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("value");
                }
                Grammar.ValidateNonWsString(value);
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _userName = value;
            }
        }
        private ulong _sessionID;
        public ulong SessionID {
            get {
                return _sessionID;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _sessionID = value;
            }
        }
        private ulong _sessionVersion;
        public ulong SessionVersion {
            get {
                return _sessionVersion;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _sessionVersion = value;
            }
        }
        private string _networkType;
        public string NetworkType {
            get {
                return _networkType;
            }
            set {
                SetAddress(value, _networkType, _address);
            }
        }
        private string _addressType;
        public string AddressType {
            get {
                return _addressType;
            }
            set {
                SetAddress(_networkType, value, _address);
            }
        }
        private string _address;
        public string Address {
            get {
                return _address;
            }
            set {
                SetAddress(_networkType, _addressType, value);
            }
        }
        public void SetAddress(string networkType, string addressType, string address) {
            if (string.IsNullOrEmpty(networkType)) {
                throw new ArgumentException("networkType");
            }
            if (string.IsNullOrEmpty(addressType)) {
                throw new ArgumentException("addressType");
            }
            if (string.IsNullOrEmpty(address)) {
                throw new ArgumentException("address");
            }
            Grammar.ValidateToken(networkType);
            Grammar.ValidateToken(addressType);
            Grammar.ValidateUnicastAddress(address);
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _networkType = networkType;
            _addressType = addressType;
            _address = address;
        }
        public IPAddress IPAddress {
            get {
                IPAddress ipAddress;
                IPAddress.TryParse(_address, out ipAddress);
                return ipAddress;
            }
            set {
                if (value == null) {
                    throw new ArgumentException("value");
                }
                SetAddress("IN", value.AddressFamily == AddressFamily.InterNetwork ? "IP4" : "IP6", value.ToString());
            }
        }

        public bool IsSameSession(Origin o) {
            return ((SessionID.Equals(o.SessionID))
                && (UserName.Equals(o.UserName))
                && (NetworkType.Equals(o.NetworkType))
                && (AddressType.Equals(o.AddressType))
                && (Address.Equals(o.Address)));
        }
        public bool IsUpdateOf(Origin o) {
            if (IsSameSession(o)) {
                return SessionVersion > o.SessionVersion;
            }
            return false;
        }
        public bool IsUpdateOrEqual(Origin o) {
            if (IsSameSession(o)) {
                return SessionVersion >= o.SessionVersion;
            }
            return false;
        }
        public static bool operator ==(Origin lhs, Origin rhs) {
            if (Object.ReferenceEquals(lhs, rhs)) {
                return true;
            }
            if (Object.ReferenceEquals(lhs, null) || Object.ReferenceEquals(rhs, null)) {
                return false;
            }
            return lhs.IsSameSession(rhs) && (lhs.SessionVersion == rhs.SessionVersion);
        }
        public static bool operator !=(Origin lhs, Origin rhs) {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj) {
            Origin o = obj as Origin;
            if (o == null) {
                return false;
            }
            else {
                return this == o;
            }
        }
        public int GetSessionHashCode() {
            int h1 = UserName != null ? UserName.GetHashCode() : 0;
            int h2 = Address != null ? Address.GetHashCode() : 0;
            int h3 = SessionID.GetHashCode();
            int hash = 17;
            hash = hash * 31 + h1;
            hash = hash * 31 + h2;
            hash = hash * 31 + h3;
            return hash;
        }
        public override int GetHashCode() {
            int h4 = SessionVersion.GetHashCode();
            int hash = GetSessionHashCode();
            hash = hash * 31 + h4;
            return hash;
        }

        internal static Origin Parse(string value) {
            Origin origin = new Origin();
            string[] parts = value.Split(' ');
            if (parts.Length != 6) {
                throw new SdpException("origin");
            }
            origin.UserName = parts[0];
            ulong sessionID = 0;
            Grammar.ValidateDigits(parts[1], false);
            if (!ulong.TryParse(parts[1], out sessionID)) {
                throw new SdpException("origin");
            }
            origin.SessionID = sessionID;
            ulong sessionVersion = 0;
            Grammar.ValidateDigits(parts[2], false);
            if (!ulong.TryParse(parts[2], out sessionVersion)) {
                throw new SdpException("origin");
            }
            origin.SessionVersion = sessionVersion;
            origin.SetAddress(parts[3], parts[4], parts[5]);
            return origin;
        }
    }
}
