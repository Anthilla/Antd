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

namespace Antd.Sdp {
    class ConnectionCollection : Collection<Connection> {
        public ConnectionCollection(Media media) {
            Media = media;
        }

        public Media Media { get; private set; }

        public bool IsReadOnly {
            get {
                return Media.IsReadOnly;
            }
        }
        protected override void InsertItem(int index, Connection item) {
            if (item == null) {
                throw new ArgumentException("item");
            }
            if (item.SessionDescription != null || item.Media != null) {
                throw new ArgumentException("value");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            item.Media = this.Media;
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, Connection item) {
            if (item == null) {
                throw new ArgumentException("item");
            }
            if (item.SessionDescription != null || item.Media != null) {
                throw new ArgumentException("value");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            this[index].Media = null;
            item.Media = this.Media;
            base.SetItem(index, item);
        }
        protected override void ClearItems() {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            foreach (Connection c in this) {
                c.Media = null;
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index) {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            this[index].Media = null;
            base.RemoveItem(index);
        }
    }
}
