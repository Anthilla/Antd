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
using System.Linq;
using System.Text;

namespace Antd.Sdp {
    public class AttributeDictionary {
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

        public Media Media { get; private set; }

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

        public void Add(string name, string value) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            Grammar.ValidateToken(name);
            if (value != null) {
                Grammar.ValidateByteString(value);
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _values.Add(new KeyValuePair<string, string>(name, value));
        }

        public void Add(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            Grammar.ValidateToken(name);
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _values.Add(new KeyValuePair<string, string>(name, null));
        }

        public void Set(string name, string value) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            Grammar.ValidateToken(name);
            if (value != null) {
                Grammar.ValidateByteString(value);
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (ContainsKey(name)) {
                throw new ArgumentException("An element with the same name already exists");
            }
            Add(name, value);
        }

        public void Set(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            Grammar.ValidateToken(name);
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            Set(name, (string)null);
        }

        public void Set(string name, IEnumerable<string> values) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            Grammar.ValidateToken(name);
            foreach (string value in values) {
                if (value != null) {
                    Grammar.ValidateByteString(value);
                }
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (ContainsKey(name)) {
                throw new ArgumentException("An element with the same name already exists");
            }
            foreach (var value in values) {
                Add(name, value);
            }
        }

        public void SetCategory(string value) {
            Set(Attribute.Category, value);
        }
        public string GetCategory() {
            string category;
            TryGetValue(Attribute.Category, out category);
            return category;
        }
        public string GetLabel() {
            string format;
            TryGetValue(Attribute.Label, out format);
            return format;
        }
        public void SetKeywords(string value) {
            Set(Attribute.Keywords, value);
        }
        public string GetKeywords() {
            string value;
            TryGetValue(Attribute.Keywords, out value);
            return value;
        }
        public void SetMaxPacketTime(TimeSpan value) {
            Set(Attribute.MaxPacketTime, ((int)value.TotalMilliseconds).ToString());
        }
        public TimeSpan? GetMaxPacketTime() {
            string value;
            TryGetValue(Attribute.MaxPacketTime, out value);
            if (value != null) {
                double time = 0;
                if (double.TryParse(value, out time)) {
                    return TimeSpan.FromMilliseconds(time);
                }
            }
            return null;
        }
        public void SetPacketTime(TimeSpan value) {
            Set(Attribute.PacketTime, ((int)value.TotalMilliseconds).ToString());
        }
        public TimeSpan? GetPacketTime() {
            string value;
            TryGetValue(Attribute.PacketTime, out value);
            if (value != null) {
                double time = 0;
                if (double.TryParse(value, out time)) {
                    return TimeSpan.FromMilliseconds(time);
                }
            }
            return null;
        }
        public void AddRtpEncoding(int pt, string encoding, int clockrate, string encodingParameters) {
            if (!string.IsNullOrEmpty(encodingParameters)) {
                Add(Attribute.RtpEncoding, string.Format("{0} {1}/{2}/{3}", pt, encoding, clockrate, encodingParameters));
            }
            else {
                Add(Attribute.RtpEncoding, string.Format("{0} {1}/{2}", pt, encoding, clockrate));
            }
        }
        public void AddRtpEncoding(int pt, string encoding, int clockrate) {
            AddRtpEncoding(pt, encoding, clockrate, null);
        }
        public bool GetRtpEncoding(int pt, out string encoding, out int clockrate, out string encodingParameters) {
            var encodings = GetValues(Attribute.RtpEncoding);
            string ptString = pt.ToString() + " ";
            foreach (string enc in encodings) {
                if (enc.StartsWith(ptString)) {
                    string[] parts = enc.Substring(ptString.Length).Split(new[] { '/' }, 3);
                    if (parts.Length >= 2) {
                        if (!int.TryParse(parts[1], out clockrate)) {
                            break;
                        }
                        encoding = parts[0];
                        if (parts.Length == 3) {
                            encodingParameters = parts[1];
                        }
                        else {
                            encodingParameters = null;
                        }
                        return true;
                    }
                }
            }
            encoding = null;
            clockrate = 0;
            encodingParameters = null;
            return false;
        }
        public bool GetRtpEncoding(int pt, out string encoding, out int clockrate) {
            string encodingParameters;
            return GetRtpEncoding(pt, out encoding, out clockrate, out encodingParameters);
        }
        public void SetSendReceive(bool send, bool receive) {
            Remove(Attribute.SendReceive);
            Remove(Attribute.SendOnly);
            Remove(Attribute.ReceiveOnly);
            if (send && receive) {
                Add(Attribute.SendReceive);
            }
            else if (send) {
                Add(Attribute.SendOnly);
            }
            else if (receive) {
                Add(Attribute.ReceiveOnly);
            }
        }
        public void GetSendReceive(out bool send, out bool receive) {
            send = true;
            receive = true;
            if (ContainsKey(Attribute.SendOnly)) {
                receive = false;
            }
            if (ContainsKey(Attribute.ReceiveOnly)) {
                send = false;
            }
            else if (ContainsKey(Attribute.SendReceive)) { }
            else {
                if ((GetConferenceType() == AttributeValue.ConferenceTypeBroadcast) ||
                    (GetConferenceType() == AttributeValue.ConferenceTypeH332)) {
                    send = false;
                }
            }
        }
        public void SetInactive(bool value) {
            if (value) {
                Set(Attribute.Inactive);
            }
            else {
                Remove(Attribute.Inactive);
            }
        }
        public bool GetInactive() {
            return ContainsKey(Attribute.Inactive);
        }
        public void SetOrientation(string value) {
            Set(Attribute.Orientation, value);
        }
        public string GetOrientation() {
            string value;
            TryGetValue(Attribute.ConferenceType, out value);
            return value;
        }
        public void SetConferenceType(string value) {
            Set(Attribute.Orientation, value);
        }
        public string GetConferenceType() {
            string value;
            TryGetValue(Attribute.ConferenceType, out value);
            return value;
        }
        public void SetFramerate(double value) {
            Set(Attribute.Framerate, value.ToString());
        }
        public double? GetFramerate() {
            string value;
            TryGetValue(Attribute.Framerate, out value);
            if (value != null) {
                double fr = 0;
                if (double.TryParse(value, out fr)) {
                    return fr;
                }
            }
            return null;
        }
        public void SetQuality(int value) {
            if (value < AttributeValue.QualityWorst || value > AttributeValue.QualityBest) {
                throw new ArgumentException("quality should be between 0 and 10");
            }
            Set(Attribute.Quality, value.ToString());
        }
        public int? GetQuality() {
            string value;
            TryGetValue(Attribute.Quality, out value);
            if (value != null) {
                int q = 0;
                if (int.TryParse(value, out q)) {
                    return q;
                }
            }
            return null;
        }
        public void AddFormatParameters(string format, string formatParameters) {
            Add(Attribute.FormatParameters, string.Format("{0} {1}", format, formatParameters));
        }
        public string GetFormatParameters(string format) {
            var formats = GetValues(Attribute.FormatParameters);
            string formatString = format + " ";
            foreach (string frmt in formats) {
                if (frmt.StartsWith(formatString)) {
                    return frmt.Substring(formatString.Length);
                }
            }
            return null;
        }
        public IDictionary<string, string> GetFormatParameterDictionary(string format) {
            string formatParameters = GetFormatParameters(format);
            if (formatParameters == null) {
                return null;
            }
            Dictionary<string, string> retval = new Dictionary<string, string>();
            string[] pairs = formatParameters.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pair in pairs) {
                string[] parts = pair.Split('=');
                if (parts.Length > 2) {
                    return null;
                }
                if (parts.Length == 1) {
                    retval.Add(parts[0], null);
                }
                else {
                    retval.Add(parts[0].Trim(), parts[1].Trim());
                }
            }
            return retval;
        }
        public void AddFormatParameters(string format, IDictionary<string, string> formatParameters) {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in formatParameters) {
                sb.Append(pair.Key);
                if (pair.Value != null) {
                    sb.Append('=');
                    sb.Append(pair.Value);
                }
                sb.Append(';');
            }
            if (sb.Length != 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            AddFormatParameters(format, sb.ToString());
        }

        public bool ContainsKey(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            Grammar.ValidateToken(name);
            foreach (var pair in _values) {
                if (pair.Key.Equals(name)) {
                    return true;
                }
            }
            return false;
        }

        public bool Remove(string name, string value) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            int removed = 0;
            while (_values.Remove(new KeyValuePair<string, string>(name, value))) {
                removed++;
            }
            return removed != 0;
        }

        public bool Remove(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            var attributes = _values.Where(pair => pair.Key.Equals(name)).ToArray();

            foreach (var pair in attributes) {
                _values.Remove(pair);
            }

            return attributes.Length != 0;
        }

        public bool TryGetValue(string name, out string value) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            var attributes = _values.Where(pair => pair.Key.Equals(name)).ToList();
            if (attributes.Count == 1) {
                value = attributes[0].Value;
                return true;
            }
            else {
                value = null;
                return false;
            }
        }

        public ICollection<string> Keys {
            get {
                HashSet<string> names = new HashSet<string>();
                foreach (var attribute in _values) {
                    names.Add(attribute.Key);
                }
                return names;
            }
        }

        public string GetValue(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            string value;
            if (!TryGetValue(name, out value)) {
                throw new InvalidOperationException("No or multiple attributes with that name");
            }
            return value;
        }

        public IList<string> GetValues(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            return _values.Where(pair => pair.Key.Equals(name)).Select(pair => pair.Value).ToList();
        }

        public IList<string> this[string name] {
            get {
                if (name == null) {
                    throw new ArgumentNullException("name");
                }
                return GetValues(name);
            }
        }

        public void Clear() {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _values.Clear();
        }

        public int Count {
            get { return _values.Count(); }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            return _values.GetEnumerator();
        }

        internal AttributeDictionary(SessionDescription sd) {
            SessionDescription = sd;
            Media = null;
        }

        internal AttributeDictionary(Media media) {
            SessionDescription = null;
            Media = media;
        }

        private List<KeyValuePair<string, string>> _values = new List<KeyValuePair<string, string>>();
    }
}
