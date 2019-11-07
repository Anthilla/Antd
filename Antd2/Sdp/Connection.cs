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
    public class Connection {
        public Connection() :
            this(IPAddress.Any, 1, 0) { }

        public Connection(IPAddress address, uint addressCount, uint ttl) {
            if (address == null) {
                throw new ArgumentException("address");
            }
            AddressCount = addressCount;
            IPAddress = address;
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                Ttl = ttl;
            }
            else {
                Ttl = 0;
            }
        }
        public Connection(string networkType, string addressType, string address, uint addressCount, uint ttl) {
            if (string.IsNullOrEmpty(networkType)) {
                throw new ArgumentException("networkType");
            }
            if (string.IsNullOrEmpty(addressType)) {
                throw new ArgumentException("addressType");
            }
            if (string.IsNullOrEmpty(address)) {
                throw new ArgumentException("address");
            }
            SetAddress(networkType, addressType, address);
            AddressCount = addressCount;
            Ttl = ttl;
        }

        private SessionDescription _sessionDescription;
        public SessionDescription SessionDescription {
            get {
                if (Media != null) {
                    return Media.SessionDescription;
                }
                else {
                    return _sessionDescription;
                }
            }
            internal set {
                _sessionDescription = value;
            }
        }
        public Media Media { get; internal set; }
        public bool IsReadOnly {
            get {
                if (Media != null && Media.IsReadOnly) {
                    return true;
                }
                if (SessionDescription != null && SessionDescription.IsReadOnly) {
                    return true;
                }
                return false;
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
            Grammar.ValidateAddress(address);
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
        private uint _addressCount;
        public uint AddressCount {
            get {
                return _addressCount;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _addressCount = value;
            }
        }
        private uint _ttl;
        public uint Ttl {
            get {
                return _ttl;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _ttl = value;
            }
        }
        public NetworkInterface NetworkInterface {
            get {
                SessionDescription sd = SessionDescription;
                if (sd != null) {
                    SessionAnnouncement sa = sd.Announcement;
                    if (sa != null) {
                        return sa.NetworkInterface;
                    }
                }
                return null;
            }
        }
    }
}
