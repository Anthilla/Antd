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
using System.Collections.ObjectModel;
using System.Linq;

namespace Antd.Sdp {
    class BandwidthCollection : Collection<Bandwidth> {
        public BandwidthCollection(SessionDescription sessionDescription) {
            SessionDescription = sessionDescription;
        }
        public BandwidthCollection(Media media) {
            Media = media;
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

        public uint? GetConferenceTotal() {
            uint? retval;
            TryGetValue(Bandwidth.TypeConferenceTotal, out retval);
            return retval;
        }

        public uint? GetApplicationSpecific() {
            uint? retval;
            TryGetValue(Bandwidth.TypeApplicationSpecific, out retval);
            return retval;
        }

        public void SetConferenceTotal(uint value) {
            Set(Bandwidth.TypeConferenceTotal, value);
        }

        public void SetApplicationSpecific(uint value) {
            Set(Bandwidth.TypeApplicationSpecific, value);
        }

        public bool TryGetValue(string type, out uint? value) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            var bandwidths = this.Where(bw => bw.Type == type).ToList();
            if (bandwidths.Count == 1) {
                value = bandwidths[0].Value;
                return true;
            }
            else {
                value = null;
                return false;
            }
        }

        public bool ContainsType(string type) {
            return this.Where(bw => bw.Type == type).Any();
        }

        public void Set(string type, uint value) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            Bandwidth bandwidth = new Bandwidth(type, value);
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (ContainsType(type)) {
                throw new ArgumentException("An element with the same type already exists");
            }
            Add(bandwidth);
        }

        protected override void InsertItem(int index, Bandwidth item) {
            if (!item.IsValid) {
                throw new ArgumentException("item");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, Bandwidth item) {
            if (!item.IsValid) {
                throw new ArgumentException("item");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.SetItem(index, item);
        }
        protected override void ClearItems() {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index) {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.RemoveItem(index);
        }
    }
}
