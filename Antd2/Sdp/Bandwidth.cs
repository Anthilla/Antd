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

namespace Antd.Sdp {
    public struct Bandwidth {
        public const string TypeConferenceTotal = "CT";
        public const string TypeApplicationSpecific = "AS";

        public static readonly string[] BandwidthTypes = null;

        public Bandwidth(string type, uint value) :
            this() {
            if (string.IsNullOrEmpty(type)) {
                throw new ArgumentException("type");
            }
            Type = type;
            Value = value;
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
                _type = value;
            }
        }
        public uint Value { get; set; }

        internal bool IsValid {
            get {
                if (string.IsNullOrEmpty(Type)) {
                    return false;
                }
                return Grammar.IsValidToken(Type);
            }
        }

        static Bandwidth() {
            BandwidthTypes = new[] { TypeConferenceTotal, TypeApplicationSpecific };
        }
    }
}
