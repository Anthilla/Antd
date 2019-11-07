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
using System.IO;
using System.Linq;
using System.Text;

namespace Antd.Sdp {
    public class SessionDescription {
        public SessionDescription() :
            this(true) { }
        private SessionDescription(bool initialize) {
            if (initialize) {
                Version = 0;
                Origin = new Origin();
                Name = " ";
                Times.Add(new Time(Time.Zero, Time.Zero));
            }
            else {
                _version = -1;
                _origin = null;
                _name = null;
            }
        }

        public bool IsReadOnly { get; private set; }
        public void SetReadOnly() {
            IsReadOnly = true;
        }

        private int _version;
        public int Version {
            get {
                return _version;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                if (value < 0) {
                    throw new ArgumentException("Version must be zero or higher", "value");
                }
                _version = value;
            }
        }
        internal bool HasOrigin {
            get {
                return _origin != null;
            }
        }
        private Origin _origin;
        public Origin Origin {
            get {
                return _origin;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                if (value.SessionDescription != null) {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                if (_origin != null) {
                    _origin.SessionDescription = null;
                }
                _session = null;
                _origin = value;
                _origin.SessionDescription = this;
            }
        }

        private Session _session;
        public Session Session {
            get {
                if (_session == null) {
                    _session = new SdpSession(this, null);
                }
                return _session;
            }
        }

        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentException("value");
                }
                Grammar.ValidateText(value);
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _name = value;
            }
        }
        public string _information;
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
        private Uri _uri;
        public Uri Uri {
            get {
                return _uri;
            }
            set {
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _uri = value;
            }
        }
        public bool HasEMails {
            get {
                return ((_emails != null) && (_emails.Count != 0));
            }
        }
        private StringCollection _emails;
        public IList<string> EMails {
            get {
                if (_emails == null) {
                    _emails = new StringCollection(StringCollection.Type.EMail, this);
                }
                return _emails;
            }
        }
        public bool HasPhoneNumbers {
            get {
                return ((_phoneNumbers != null) && (_phoneNumbers.Count != 0));
            }
        }
        private StringCollection _phoneNumbers;
        public IList<string> PhoneNumbers {
            get {
                if (_phoneNumbers == null) {
                    _phoneNumbers = new StringCollection(StringCollection.Type.Phone, this);
                }
                return _phoneNumbers;
            }
        }
        public bool HasConnection {
            get {
                return _connection != null;
            }
        }
        private Connection _connection;
        public Connection Connection {
            get {
                return _connection;
            }
            set {
                if (value.SessionDescription != null || value.Media != null) {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly) {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                if (_connection != null) {
                    _connection.SessionDescription = null;
                }
                _connection = value;
                _connection.SessionDescription = this;
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
        internal bool HasTimes {
            get {
                return ((_times != null) && (_times.Count != 0));
            }
        }
        private TimeCollection _times;
        public IList<Time> Times {
            get {
                if (_times == null) {
                    _times = new TimeCollection(this);
                }
                return _times;
            }
        }
        public Time Time {
            get { return Times[0]; }
            set { Times[0] = value; }
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
        public bool HasMedias {
            get {
                return ((_medias != null) && (_medias.Count != 0));
            }
        }
        private MediaCollection _medias;
        public IList<Media> Medias {
            get {
                if (_medias == null) {
                    _medias = new MediaCollection(this);
                }
                return _medias;
            }
        }
        public bool IsSameSession(SessionDescription sd) {
            if (sd == null) {
                return false;
            }
            return Origin.IsSameSession(sd.Origin);
        }
        public bool IsUpdateOf(SessionDescription sd) {
            if (sd == null) {
                return false;
            }
            return Origin.IsUpdateOf(sd.Origin);
        }
        public static bool operator ==(SessionDescription lhs, SessionDescription rhs) {
            if (Object.ReferenceEquals(lhs, rhs)) {
                return true;
            }
            if (Object.ReferenceEquals(lhs, null) || Object.ReferenceEquals(rhs, null)) {
                return false;
            }
            return lhs.Origin == rhs.Origin;
        }
        public static bool operator !=(SessionDescription lhs, SessionDescription rhs) {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj) {
            SessionDescription sd = obj as SessionDescription;
            return this == sd;
        }
        public override int GetHashCode() {
            return Origin.GetHashCode();
        }
        public void Save(TextWriter textWriter) {
            textWriter.Write(ToString());
        }
        public void Save(Stream stream) {
            Save(new StreamWriter(stream));
        }
        public static SessionDescription Parse(string text) {
            return Parse(text, LoadOptions.Default);
        }
        public static SessionDescription Parse(string text, LoadOptions options) {
            return Load(new StringReader(text), options);
        }
        public static SessionDescription Load(Stream stream) {
            return Load(stream, LoadOptions.Default);
        }
        public static SessionDescription Load(Stream stream, LoadOptions loadOptions) {
            return Load(new StreamReader(stream), loadOptions);
        }
        public static SessionDescription Load(TextReader reader) {
            return Load(reader, LoadOptions.Default);
        }
        public static SessionDescription Load(TextReader reader, LoadOptions loadOptions) {
            SessionDescription sd = new SessionDescription(false);

            Media media = null;
            string line = reader.ReadLine();
            while (line != null) {
                if ((line.Length == 0) && ((loadOptions & LoadOptions.IgnoreEmptyLines) != 0)) {
                    goto nextline;
                }
                if (line.Length < 3) {
                    goto invalidline;
                }
                if (line[1] != '=') {
                    goto invalidline;
                }
                string value = line.Substring(2);
                string[] parts = null;
                int sep = -1;
                try {
                    switch (line[0]) {
                        case 'v':
                            if (media != null) {
                                goto invalidline;
                            }
                            int version = -1;
                            Grammar.ValidateDigits(value, false);
                            if (!int.TryParse(value, out version)) {
                                goto invalidline;
                            }
                            if ((loadOptions & LoadOptions.IgnoreUnsupportedVersion) == 0) {
                                if (version != 0) {
                                    goto notsupported;
                                }
                            }
                            sd.Version = version;
                            break;
                        case 'o':
                            if (media != null) {
                                goto invalidline;
                            }
                            sd.Origin = Origin.Parse(value);
                            break;
                        case 's':
                            if (media != null) {
                                goto invalidline;
                            }
                            sd.Name = value;
                            break;
                        case 'i':
                            if (media == null) {
                                sd.Information = value;
                            }
                            else {
                                media.Information = value;
                            }
                            break;
                        case 'u':
                            if (media != null) {
                                goto invalidline;
                            }
                            sd.Uri = new Uri(value);
                            break;
                        case 'e':
                            if (media != null) {
                                goto invalidline;
                            }
                            sd.EMails.Add(value);
                            break;
                        case 'p':
                            if (media != null) {
                                goto invalidline;
                            }
                            sd.PhoneNumbers.Add(value);
                            break;
                        case 'c':
                            if (media == null) {
                                if (sd.HasConnection) {
                                    goto invalidline;
                                }
                            }
                            Connection conn = new Connection();
                            parts = value.Split(' ');
                            if (parts.Length != 3) {
                                goto invalidline;
                            }
                            string networkType = parts[0];
                            string addressType = parts[1];
                            parts = parts[2].Split('/');
                            if (parts.Length > 3) {
                                goto invalidline;
                            }
                            conn.SetAddress(networkType, addressType, parts[0]);
                            conn.Address = parts[0];
                            conn.AddressCount = 1;
                            if (parts.Length >= 2) {
                                uint ttl = 0;
                                if (!uint.TryParse(parts[1], out ttl)) {
                                    goto invalidline;
                                }
                                conn.Ttl = ttl;
                                if (parts.Length == 3) {
                                    uint addressCount = 0;
                                    if (!uint.TryParse(parts[2], out addressCount)) {
                                        goto invalidline;
                                    }
                                    conn.AddressCount = addressCount;
                                }
                                if (conn.AddressType == "IP6") {
                                    if (parts.Length == 3) {
                                        goto invalidline;
                                    }
                                    conn.AddressCount = conn.Ttl;
                                    conn.Ttl = 0;
                                }
                            }

                            if (media != null) {
                                media.Connections.Add(conn);
                            }
                            else {
                                sd.Connection = conn;
                            }
                            break;
                        case 'b':
                            sep = value.IndexOf(':');
                            if (sep == -1) {
                                goto invalidline;
                            }
                            string type = value.Substring(0, sep);
                            string bw = value.Substring(sep + 1);
                            Grammar.ValidateDigits(bw, false);
                            uint bwValue = 0;
                            if (!uint.TryParse(bw, out bwValue)) {
                                goto invalidline;
                            }
                            if (media != null) {
                                media.Bandwidths.Add(new Bandwidth(type, bwValue));
                            }
                            else {
                                sd.Bandwidths.Add(new Bandwidth(type, bwValue));
                            }
                            break;
                        case 'a':
                            sep = value.IndexOf(':');
                            string attrName;
                            string attrValue;
                            if (sep != -1) {
                                attrName = value.Substring(0, sep);
                                attrValue = value.Substring(sep + 1);
                            }
                            else {
                                attrName = value;
                                attrValue = null;
                            }
                            if (media != null) {
                                media.Attributes.Add(attrName, attrValue);
                            }
                            else {
                                sd.Attributes.Add(attrName, attrValue);
                            }
                            break;
                        case 't':
                            if (media != null) {
                                goto invalidline;
                            }
                            parts = value.Split(' ');
                            if (parts.Length != 2) {
                                goto invalidline;
                            }
                            Grammar.ValidateTime(parts[0]);
                            double startTime = 0;
                            if (!double.TryParse(parts[0], out startTime)) {
                                goto invalidline;
                            }
                            Grammar.ValidateTime(parts[1]);
                            double stopTime = 0;
                            if (!double.TryParse(parts[1], out stopTime)) {
                                goto invalidline;
                            }
                            DateTime startDateTime = Time.Zero;
                            DateTime stopDateTime = Time.Zero;
                            if (startTime != 0) {
                                startDateTime = Time.Zero + TimeSpan.FromSeconds(startTime);
                            }
                            if (stopTime != 0) {
                                stopDateTime = Time.Zero + TimeSpan.FromSeconds(stopTime);
                            }
                            sd.Times.Add(new Time(startDateTime, stopDateTime));
                            break;
                        case 'm':
                            parts = value.Split(' ');
                            if (parts.Length < 4) {
                                goto invalidline;
                            }
                            string mediaType = parts[0];
                            string protocol = parts[2];
                            var formats = parts.Skip(3).ToList();
                            parts = parts[1].Split('/');
                            if (parts.Length > 2) {
                                goto invalidline;
                            }
                            uint port = 0;
                            uint portCount = 1;
                            Grammar.ValidateDigits(parts[0], false);
                            if (!uint.TryParse(parts[0], out port)) {
                                goto invalidline;
                            }
                            if (parts.Length == 2) {
                                Grammar.ValidateDigits(parts[1], true);
                                if (!uint.TryParse(parts[1], out portCount)) {
                                    goto invalidline;
                                }
                            }
                            media = new Media(mediaType, port, portCount, protocol, formats);
                            sd.Medias.Add(media);
                            break;
                        case 'z':
                        case 'k':
                        case 'r':
                            if ((loadOptions & LoadOptions.IgnoreUnsupportedLines) == 0) {
                                goto notsupported;
                            }
                            break;
                        default:
                            if ((loadOptions & LoadOptions.IgnoreUnknownLines) == 0) {
                                goto invalidline;
                            }
                            break;
                    }
                }
                catch {
                    goto invalidline;
                }
            nextline:
                line = reader.ReadLine();
                continue;
            invalidline:
                throw new SdpException(string.Format("Invalid Line {0}", line));
            notsupported:
                throw new SdpException(string.Format("Unsupported line {0}", line));
            }

            if (sd.Version == -1) {
                throw new SdpException("Version ('v=') is required");
            }
            if (!sd.HasOrigin) {
                throw new SdpException("Origin ('o=') is required");
            }
            if (sd.Name == null) {
                throw new SdpException("Name ('s=') is required");
            }
            if (!sd.HasTimes) {
                throw new SdpException("Time ('t=') is required");
            }
            return sd;
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            sb.Append("v=");
            sb.Append(Version);
            sb.Append("\r\n");

            sb.Append("o=");
            sb.Append(Origin.UserName);
            sb.Append(' ');
            sb.Append(Origin.SessionID);
            sb.Append(' ');
            sb.Append(Origin.SessionVersion);
            sb.Append(' ');
            sb.Append(Origin.NetworkType);
            sb.Append(' ');
            sb.Append(Origin.AddressType);
            sb.Append(' ');
            sb.Append(Origin.Address);
            sb.Append("\r\n");

            sb.Append("s=");
            sb.Append(Name);
            sb.Append("\r\n");

            if (Information != null) {
                sb.Append("i=");
                sb.Append(Information);
                sb.Append("\r\n");
            }

            if (Uri != null) {
                sb.Append("u=");
                sb.Append(Uri);
                sb.Append("\r\n");
            }

            foreach (string email in EMails) {
                sb.Append("e=");
                sb.Append(email);
                sb.Append("\r\n");
            }
            foreach (string phone in PhoneNumbers) {
                sb.Append("p=");
                sb.Append(phone);
                sb.Append("\r\n");
            }
            if (HasConnection) {
                sb.Append("c=");
                sb.Append(Connection.NetworkType);
                sb.Append(' ');
                sb.Append(Connection.AddressType);
                sb.Append(' ');
                sb.Append(Connection.Address);
                if (Connection.Ttl != 0) {
                    sb.Append('/');
                    sb.Append(Connection.Ttl);
                }
                if (Connection.AddressCount != 0) {
                    sb.Append('/');
                    sb.Append(Connection.AddressCount);
                }
                sb.Append("\r\n");
            }
            foreach (Bandwidth bw in Bandwidths) {
                sb.Append("b=");
                sb.Append(bw.Type);
                sb.Append(' ');
                sb.Append(bw.Value);
                sb.Append("\r\n");
            }
            foreach (Time time in Times) {
                sb.Append("t=");
                sb.Append((decimal)((time.StartTime - Time.Zero).TotalSeconds));
                sb.Append(' ');
                sb.Append((decimal)((time.StopTime - Time.Zero).TotalSeconds));
                sb.Append("\r\n");
            }
            foreach (var attr in Attributes) {
                sb.Append("a=");
                sb.Append(attr.Key);
                if (attr.Value != null) {
                    sb.Append(':');
                    sb.Append(attr.Value);
                }
                sb.Append("\r\n");
            }
            foreach (Media media in Medias) {
                sb.Append("m=");
                sb.Append(media.Type);
                sb.Append(' ');
                sb.Append(media.Port);
                if (media.PortCount != 1) {
                    sb.Append('/');
                    sb.Append(media.PortCount);
                }
                sb.Append(' ');
                sb.Append(media.Protocol);
                foreach (string format in media.Formats) {
                    sb.Append(' ');
                    sb.Append(format);
                }
                sb.Append("\r\n");

                if (media.Information != null) {
                    sb.Append("i=");
                    sb.Append(media.Information);
                    sb.Append("\r\n");
                }

                foreach (Connection conn in media.Connections) {
                    sb.Append("c=");
                    sb.Append(conn.NetworkType);
                    sb.Append(' ');
                    sb.Append(conn.AddressType);
                    sb.Append(' ');
                    sb.Append(conn.Address);
                    if (conn.Ttl != 0) {
                        sb.Append('/');
                        sb.Append(conn.Ttl);
                    }
                    if (conn.AddressCount != 1) {
                        sb.Append('/');
                        sb.Append(conn.AddressCount);
                    }
                    sb.Append("\r\n");
                }
                foreach (Bandwidth bw in media.Bandwidths) {
                    sb.Append("b=");
                    sb.Append(bw.Type);
                    sb.Append(' ');
                    sb.Append(bw.Value);
                    sb.Append("\r\n");
                }
                foreach (var attr in media.Attributes) {
                    sb.Append("a=");
                    sb.Append(attr.Key);
                    if (attr.Value != null) {
                        sb.Append(':');
                        sb.Append(attr.Value);
                    }
                    sb.Append("\r\n");
                }
            }
            return sb.ToString();
        }

        public SessionAnnouncement Announcement { get; internal set; }
    }
}
