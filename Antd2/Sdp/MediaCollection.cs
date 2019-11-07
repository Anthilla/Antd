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
    class MediaCollection : Collection<Media> {
        public MediaCollection(SessionDescription sd) {
            SessionDescription = sd;
        }

        public SessionDescription SessionDescription { get; private set; }
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

        protected override void InsertItem(int index, Media item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (item.SessionDescription != null) {
                throw new ArgumentException("Media is already part of a SessionDescription");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.InsertItem(index, item);
            item.SessionDescription = SessionDescription;
        }
        protected override void ClearItems() {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            foreach (var media in this) {
                media.SessionDescription = null;
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index) {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base[index].SessionDescription = null;
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, Media item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (item.SessionDescription != null) {
                throw new ArgumentException("Media is already part of a SessionDescription");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base[index].SessionDescription = null;
            base.SetItem(index, item);
            item.SessionDescription = SessionDescription;
        }
    }
}
