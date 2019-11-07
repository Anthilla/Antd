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
using System.Collections.Generic;

namespace Antd.Sdp {
    public class Media {
        public const string TypeAudio = "audio";
        public const string TypeVideo = "video";
        public const string TypeText = "text";
        public const string TypeApplication = "application";
        public const string TypeMessage = "message";
        public static readonly string[] MediaTypes = null;

        public const string ProtocolUdp = "udp";
        public const string ProtocolRtpAvp = "RTP/AVP";
        public const string ProtocolRtpSavp = "RTP/SAVP";
        public static readonly string[] MediaProtocols = null;

        public Media(string type, uint port, uint portCount, string protocol, IEnumerable<string> formats) {
            if (string.IsNullOrEmpty(type)) {
                throw new ArgumentException("type");
            }
            if (string.IsNullOrEmpty(protocol)) {
                throw new ArgumentException("protocol");
            }
            if (formats == null) {
                throw new ArgumentNullException("formats");
            }

            Type = type;
            Port = port;
            PortCount = portCount;
            Protocol = protocol;
            _formats = new StringCollection(StringCollection.Type.Format, this, formats);
        }

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

        private string _type;
        public string Type {
            get {
                return _type;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("value");
                }
                Grammar.ValidateToken(value);
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _type = value;
            }
        }
        private uint _port;
        public uint Port {
            get {
                return _port;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _port = value;
            }
        }
        private uint _portCount;
        public uint PortCount {
            get {
                return _portCount;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _portCount = value;
            }
        }
        private string _protocol;
        public string Protocol {
            get {
                return _protocol;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("value");
                }
                Grammar.ValidateProtocol(value);
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _protocol = value;
            }
        }
        private StringCollection _formats;
        public IList<string> Formats {
            get {
                return _formats;
            }
        }
        private string _information;
        public string Information {
            get {
                return _information;
            }
            set {
                if (value == string.Empty) {
                    throw new ArgumentException("value");
                }
                Grammar.ValidateText(value);
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _information = value;
            }
        }
        public SessionDescription SessionDescription { get; internal set; }
        public bool HasConnections {
            get {
                return ((_connections != null) && (_connections.Count != 0));
            }
        }
        private ConnectionCollection _connections;
        public IList<Connection> Connections {
            get {
                if (_connections == null) {
                    _connections = new ConnectionCollection(this);
                }
                return _connections;
            }
        }
        public bool HasBandwidths {
            get {
                return ((_bandwidths != null) && (_bandwidths.Count != 0));
            }
        }
        private BandwidthCollection _bandwidths;
        public IList<Bandwidth> Bandwidths {
            get {
                if (_bandwidths == null) {
                    _bandwidths = new BandwidthCollection(this);
                }
                return _bandwidths;
            }
        }
        public bool HasAttributes {
            get {
                return ((_attributes != null) && (_attributes.Count != 0));
            }
        }
        private AttributeDictionary _attributes;
        public AttributeDictionary Attributes {
            get {
                if (_attributes == null) {
                    _attributes = new AttributeDictionary(this);
                }
                return _attributes;
            }
        }

        static Media() {
            MediaTypes = new[] { TypeAudio, TypeVideo, TypeText, TypeApplication, TypeMessage };
            MediaProtocols = new[] { ProtocolUdp, ProtocolRtpAvp, ProtocolRtpSavp };
        }
    }
}
