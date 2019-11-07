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
using System.Collections.ObjectModel;

namespace Antd.Sdp {
    class StringCollection : Collection<string> {
        public enum Type {
            Phone,
            EMail,
            Format
        }
        public StringCollection(Type type, SessionDescription sessionDescription) {
            _type = type;
            SessionDescription = sessionDescription;
        }
        public StringCollection(Type type, Media media, IEnumerable<string> formats) {
            if (type != Type.Format) {
                throw new Exception("Invalid Type for this constructor");
            }
            _type = type;
            Media = media;
            foreach (string format in formats) {
                Add(format);
            }
            if (Count == 0) {
                throw new ArgumentException("Formats cannot be empty");
            }
        }
        public SessionDescription SessionDescription { get; private set; }
        public Media Media { get; private set; }
        public bool IsReadOnly {
            get {
                if (SessionDescription != null && SessionDescription.IsReadOnly) {
                    return true;
                }
                if (Media != null && Media.IsReadOnly) {
                    return true;
                }
                return false;
            }
        }
        protected override void InsertItem(int index, string item) {
            if (string.IsNullOrEmpty(item)) {
                throw new ArgumentNullException("item");
            }
            Validate(item);
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, string item) {
            if (string.IsNullOrEmpty(item)) {
                throw new ArgumentNullException("item");
            }
            Validate(item);
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.SetItem(index, item);
        }
        protected override void ClearItems() {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (_type == Type.Format) {
                throw new InvalidCastException("Formats cannot be empty");
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index) {
            if (IsReadOnly) {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (Count == 1) {
                throw new InvalidCastException("Formats cannot be empty");
            }
            base.RemoveItem(index);
        }
        private void Validate(string item) {
            if (_type == Type.EMail) {
                Grammar.ValidateEMail(item);
            }
            else if (_type == Type.Phone) {
                Grammar.ValidatePhoneNumber(item);
            }
            else if (_type == Type.Format) {
                Grammar.ValidateToken(item);
            }
        }
        private Type _type;
    }
}
