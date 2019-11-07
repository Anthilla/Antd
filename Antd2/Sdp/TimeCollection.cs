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
    class TimeCollection : Collection<Time> {
        public TimeCollection(SessionDescription sessionDescription) {
            SessionDescription = sessionDescription;
        }

        public SessionDescription SessionDescription { get; private set; }

        public bool IsReadOnly {
            get {
                return SessionDescription.IsReadOnly;
            }
        }

        protected override void InsertItem(int index, Time item) {
            if (!item.IsValid) {
                throw new ArgumentException("item");
            }
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, Time item) {
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
            if (Count == 0) {
                Add(new Time(Time.Zero, Time.Zero));
            }
        }
        protected override void RemoveItem(int index) {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.RemoveItem(index);
            if (Count == 0) {
                Add(new Time(Time.Zero, Time.Zero));
            }
        }
    }
}
